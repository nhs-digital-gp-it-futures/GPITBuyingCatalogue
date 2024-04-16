using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using BuyingCatalogueFunction.Notifications.PasswordExpiry.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace BuyingCatalogueFunction.Notifications.PasswordExpiry.Services;

public class PasswordExpiryService : IPasswordExpiryService
{
    private readonly IOptions<QueueOptions> options;
    private readonly QueueServiceClient queueServiceClient;
    private readonly BuyingCatalogueDbContext dbContext;
    private readonly ILogger<PasswordExpiryService> logger;

    public PasswordExpiryService(
        BuyingCatalogueDbContext dbContext,
        IOptions<QueueOptions> options,
        QueueServiceClient queueServiceClient,
        ILogger<PasswordExpiryService> logger)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.queueServiceClient = queueServiceClient ?? throw new ArgumentNullException(nameof(queueServiceClient));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ICollection<AspNetUser>> GetUsersNearingPasswordExpiry(DateTime today)
    {
        var query = dbContext.Users.Include(x => x.Events).Where(
            x => !x.Disabled &&
                 !(x.Events.Any(y => y.EventTypeId == (int)EventTypeEnum.PasswordEnteredFirstExpiryThreshold) &&
                   x.Events.Any(y => y.EventTypeId == (int)EventTypeEnum.PasswordEnteredSecondExpiryThreshold) &&
                   x.Events.Any(y => y.EventTypeId == (int)EventTypeEnum.PasswordEnteredThirdExpiryThreshold)));

        query = AddThresholdFilters(today, query);

        return await query.ToListAsync();
    }

    public async Task Raise(DateTime date, AspNetUser user, EventTypeEnum eventType)
    {
        dbContext.Attach(user);

        var notification = await CreateNotification(date, user, eventType);

        await DispatchNotification(user, notification, eventType);
    }

    private async Task<EmailNotification> CreateNotification(DateTime date, AspNetUser user, EventTypeEnum eventType)
    {
        user.Events.Add(new AspNetUserEvent((int)eventType));

        var notification = new EmailNotification { To = user.Email };

        notification.JsonFrom(new PasswordDueToExpireEmailModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            DaysRemaining = user.RemainingPasswordExpiryDays(date),
        });

        dbContext.Add(notification);
        await dbContext.SaveChangesAsync();

        return notification;
    }

    private async Task DispatchNotification(
        AspNetUser user,
        EmailNotification notification,
        EventTypeEnum eventType)
    {
        var queueName = options.Value.SendEmailNotifications;
        var client = queueServiceClient.GetQueueClient(queueName);

        try
        {
            await client.SendMessageAsync(Convert.ToBase64String(Encoding.UTF8.GetBytes(notification.Id.ToString())));
        }
        catch (Exception e)
        {
            logger.LogError(
                e,
                "Password Expiry: {UserId}, {EventType} - Notifications saved but problem dispatching to queue {Queue}",
                user.Id, eventType, queueName);
            throw;
        }
    }

    [ExcludeFromCodeCoverage]
    private IQueryable<AspNetUser> AddThresholdFilters(DateTime today, IQueryable<AspNetUser> query)
    {
        var highestThreshold = AspNetUser.ExpiryThresholds.ThresholdsMap
            .OrderByDescending(x => x.Threshold)
            .Select(x => x.Threshold)
            .First();

        if (dbContext.Database.IsSqlServer())
            query = query
                .Where(x =>
                    EF.Functions.DateDiffDay(today, x.PasswordUpdatedDate.AddYears(1)) <= highestThreshold
                    && EF.Functions.DateDiffDay(today, x.PasswordUpdatedDate.AddYears(1)) > 0);

        return query;
    }
}
