using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace BuyingCatalogueFunction.Notifications
{
    public class OrderEnteredFirstExpiryThresholdFunction
    {
        private readonly ILogger<OrderEnteredFirstExpiryThresholdFunction> logger;
        private readonly BuyingCatalogueDbContext dbContext;
        private readonly QueueServiceClient queueServiceClient;

        public OrderEnteredFirstExpiryThresholdFunction(
            ILogger<OrderEnteredFirstExpiryThresholdFunction> logger,
            BuyingCatalogueDbContext dbContext,
            QueueServiceClient queueServiceClient)
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.queueServiceClient = queueServiceClient;
        }

        // {second} {minute} {hour} {day} {month} {day-of-week}
        // 0        */5      *      *     *       * // Every 5 minutes for testing...
        [Function(nameof(OrderEnteredFirstExpiryThresholdFunction))]
        public async Task Run([TimerTrigger("0 */5 * * * *")] MyInfo myTimer)
        {
            Event defaultEmailPreference = GetDefaultEmailPreference(EventTypeEnum.OrderEnteredFirstExpiryThreshold);

            if (defaultEmailPreference?.ManagedEmailPreference != null)
            {
                var today = new DateTime(2027, 01, 01); //  System.DateTime.UtcNow.Date;
                List<Order> orders = await GetOrdersToProcess(today, EventTypeEnum.OrderEnteredFirstExpiryThreshold);

                if (!orders.Any())
                {
                    logger.LogInformation($"No {EventTypeEnum.OrderEnteredFirstExpiryThreshold} orders to process for {today}");
                    return;
                }

                foreach (var order in orders)
                {
                    try
                    {
                        List<AspNetUser> users = await GetUsersForOrganisation(order.OrderingPartyId);
                        var notifications = await SaveUserNotifications(defaultEmailPreference, order, users);
                        await DisaptchNotifications(order, notifications);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, $"Problem - skipping order {order.Id} processing {EventTypeEnum.OrderEnteredFirstExpiryThreshold}");
                    }
                }
            }
            else
            {
                logger.LogWarning($"Event not found or a ManagedEmailPreference is not configuired for {EventTypeEnum.OrderEnteredFirstExpiryThreshold}");
            }
        }

        private async Task<List<Notification>> SaveUserNotifications(Event defaultEmailPreference, Order order, List<AspNetUser> users)
        {
            var notifications = new List<Notification>();
            var txOptions = new TransactionOptions()
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
            };

            using var scope = new TransactionScope(TransactionScopeOption.Required, txOptions, TransactionScopeAsyncFlowOption.Enabled);
            // Add order event for OrderEnteredFirstExpiryThreshold
            // so on subsequent checks we know we've already process this
            // created the relevant notification records
            order.OrderEvents.Add(new OrderEvent() { EventId = (int)EventTypeEnum.OrderEnteredFirstExpiryThreshold });

            foreach (var user in users)
            {
                if (ShouldSendBasedOnUserPreferences(defaultEmailPreference, user))
                {
                    notifications.Add(CreateNotificationForUser(user));
                }
            }
            dbContext.AddRange(notifications);
            await dbContext.SaveChangesAsync();
            scope.Complete();

            return notifications;
        }

        private static bool ShouldSendBasedOnUserPreferences(Event defaultEmailPreference, AspNetUser _)
        {
            if (defaultEmailPreference.ManagedEmailPreference.DefaultEnabled)
            {
                // TODO:  opt out - so send unless the user has explicitly opted out
            }
            else
            {
                // TODO: opt in - so ONLY send if the user has explicitly opted in
            }

            // TODO
            return defaultEmailPreference.ManagedEmailPreference.DefaultEnabled;
        }

        private async Task<List<AspNetUser>> GetUsersForOrganisation(int organisationId)
        {
            return await dbContext
                .Users
                .Where(u => u.PrimaryOrganisationId == organisationId)
                .ToListAsync();
        }

        private Event GetDefaultEmailPreference(EventTypeEnum eventType)
        {
            return dbContext
                .Events
                .Include(e => e.ManagedEmailPreference)
                .FirstOrDefault(u => u.Id == (int)eventType);
        }

        private static Notification CreateNotificationForUser(AspNetUser user)
        {
            var notification = new Notification()
            {
                To = user.Email,
            };

            notification.JsonFrom(new OrderEnteredFirstExpiryThresholdEmailContent());
            return notification;
        }

        private async Task DisaptchNotifications(Order order, List<Notification> notifications)
        {
            try
            {
                // now hand the messages off to the messaging system.
                // if something goes wrong here
                // we can subsequently identifiy unprocessed notification rows and try sending them again.
                // processing of notifications should be idempotent so that it is safe to retry messages
                // In the case of gov.nofify we can be somewhat idempotent within the retention period
                // by using and checking for our reference.
                var client = queueServiceClient.GetQueueClient("notifications");
                foreach (var notification in notifications)
                    await client.SendMessageAsync(
                        Convert.ToBase64String(Encoding.UTF8.GetBytes(notification.Id.ToString())));
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Problem dispatching {EventTypeEnum.OrderEnteredFirstExpiryThreshold} with order {order.Id}");
                throw;
            }
        }

        private async Task<List<Order>> GetOrdersToProcess(DateTime today, EventTypeEnum eventType)
        {
            return await dbContext.Orders
                .Where(o =>
                    o.CommencementDate.HasValue
                    && o.MaximumTerm.HasValue
                    // Make sure "OrderEnteredFirstExpiryThreshold" event for this order doesn't already exist
                    && !o.OrderEvents.Any(e => e.EventId == (int)eventType)
                    && today < o.CommencementDate.Value.AddMonths(o.MaximumTerm.Value).AddDays(-1)
                    && (o.MaximumTerm.Value >= 3 && EF.Functions.DateDiffDay(today, o.CommencementDate.Value.AddMonths(o.MaximumTerm.Value).AddDays(-1)) <= 90
                    || o.MaximumTerm.Value < 3 && EF.Functions.DateDiffDay(today, o.CommencementDate.Value.AddMonths(o.MaximumTerm.Value).AddDays(-1)) <= 30))
                .ToListAsync();
        }
    }

    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
