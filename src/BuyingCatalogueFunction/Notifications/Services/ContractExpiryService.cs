using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using BuyingCatalogueFunction.Notifications.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace BuyingCatalogueFunction.Notifications.Services
{
    public class ContractExpiryService : IContractExpiryService
    {
        private readonly ILogger<ContractExpiryService> logger;
        private readonly QueueOptions options;
        private readonly BuyingCatalogueDbContext dbContext;
        private readonly QueueServiceClient queueServiceClient;

        public ContractExpiryService(
            ILogger<ContractExpiryService> logger,
            IOptions<QueueOptions> options,
            BuyingCatalogueDbContext dbContext,
            QueueServiceClient queueServiceClient)
        {
            this.dbContext = dbContext;
            this.options = options.Value;
            this.queueServiceClient = queueServiceClient;
            this.logger = logger;
        }

        public async Task<List<Order>> GetOrdersNearingExpiry(DateTime today)
        {
            var query = dbContext.Orders
                .Include(o => o.ContractOrderNumber)
                .ThenInclude(o => o.OrderEvents)
                .Where(o =>
                    o.Completed.HasValue
                    && !o.IsTerminated
                    && !o.IsDeleted
                    && o.CommencementDate.HasValue
                    && o.MaximumTerm.HasValue
                    && !(o.ContractOrderNumber.OrderEvents.Any(e => e.EventTypeId == (int)EventTypeEnum.OrderEnteredFirstExpiryThreshold)
                        && o.ContractOrderNumber.OrderEvents.Any(e => e.EventTypeId == (int)EventTypeEnum.OrderEnteredSecondExpiryThreshold))
                    && today < o.CommencementDate.Value.AddMonths(o.MaximumTerm.Value).AddDays(-1));

            query = AddThresholdFilters(today, query);

            return (await query.ToListAsync())
                .GroupBy(x => x.OrderNumber)
                .SelectMany(x => x.OrderByDescending(y => y.Revision).Take(1))
                .ToList();
        }

        public async Task<EmailPreferenceType> GetDefaultEmailPreference(EventTypeEnum eventType)
        {
            return (await dbContext
                .EventTypes
                .Include(e => e.EmailPreferenceType)
                .FirstOrDefaultAsync(u => u.Id == (int)eventType))?.EmailPreferenceType;
        }

        public async Task RaiseExpiry(DateTime date, Order order, EventTypeEnum eventType, EmailPreferenceType emailPreference)
        {
            dbContext.Attach(order);

            List<AspNetUser> users = await GetUsersForOrganisation(order.OrderingPartyId);
            var notifications = await SaveUserNotifications(date, order, eventType, users, emailPreference);
            await DisaptchNotifications(order, notifications, eventType);
        }

        private async Task DisaptchNotifications(Order order,
            List<EmailNotification> notifications,
            EventTypeEnum eventType)
        {
            var client = queueServiceClient.GetQueueClient(options.SendEmailNotifications);
            var tasks = notifications
                .Select(n => client.SendMessageAsync(Convert.ToBase64String(Encoding.UTF8.GetBytes(n.Id.ToString()))));

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                logger.LogError(
                    e,
                    "Order {CallOffId}. {EventType} - Notifications saved but problem dispatching to queue {Queue}",
                    order.CallOffId,
                    eventType,
                    options.SendEmailNotifications);
                throw;
            }
        }

        private async Task<List<EmailNotification>> SaveUserNotifications(
            DateTime date,
            Order order,
            EventTypeEnum eventType,
            List<AspNetUser> users,
            EmailPreferenceType emailPreferenceType)
        {
            var notifications = new List<EmailNotification>();

            order.ContractOrderNumber.OrderEvents.Add(new OrderEvent() { EventTypeId = (int)eventType });

            foreach (var user in users)
            {
                if (await ShouldSendBasedOnUserPreferences(emailPreferenceType, user))
                {
                    notifications.Add(CreateNotificationForUser(user, order.CallOffId, order.EndDate.RemainingDays(date)));
                }
            }
            dbContext.AddRange(notifications);
            await dbContext.SaveChangesAsync();

            return notifications;
        }

        private async Task<bool> ShouldSendBasedOnUserPreferences(
            EmailPreferenceType emailPreferenceType,
            AspNetUser user)
        {
            var userPreference = await dbContext
                .UserEmailPreferences
                .FirstOrDefaultAsync(u => u.EmailPreferenceTypeId == emailPreferenceType.Id
                    && u.UserId == user.Id);

            return userPreference?.Enabled ?? emailPreferenceType.DefaultEnabled;
        }

        private async Task<List<AspNetUser>> GetUsersForOrganisation(int organisationId)
        {
            return await dbContext
                .Users
                .Where(u => u.PrimaryOrganisationId == organisationId
                    && !u.Disabled)
                .ToListAsync();
        }

        private static EmailNotification CreateNotificationForUser(AspNetUser user, CallOffId callOffId, int daysRemaining)
        {
            var notification = new EmailNotification()
            {
                To = user.Email,
            };

            notification.JsonFrom(new ContractDueToExpireEmailModel()
            {
                CallOffId = callOffId.ToString(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                DaysRemaining = daysRemaining,
            });

            return notification;
        }

        [ExcludeFromCodeCoverage]
        private IQueryable<Order> AddThresholdFilters(DateTime today, IQueryable<Order> query)
        {
            if (dbContext.Database.IsSqlServer())
                query = query
                    .Where(o =>
                        o.MaximumTerm.Value >= 3 && (EF.Functions.DateDiffDay(today, o.CommencementDate.Value.AddMonths(o.MaximumTerm.Value).AddDays(-1)) <= 90)
                        || o.MaximumTerm.Value < 3 && (EF.Functions.DateDiffDay(today, o.CommencementDate.Value.AddMonths(o.MaximumTerm.Value).AddDays(-1)) <= 30));

            return query;
        }
    }
}
