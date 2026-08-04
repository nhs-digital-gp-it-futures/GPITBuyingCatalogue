using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace BuyingCatalogueFunction.Notifications.InactiveAccount;

public partial class InactiveAccountsService(
    BuyingCatalogueDbContext dbContext,
    IOptions<QueueOptions> options,
    QueueServiceClient queueServiceClient,
    ILogger<InactiveAccountsService> logger) : IInactiveAccountsService
{
    private readonly IOptions<QueueOptions> options = options ?? throw new ArgumentNullException(nameof(options));
    private readonly QueueServiceClient queueServiceClient = queueServiceClient ?? throw new ArgumentNullException(nameof(queueServiceClient));
    private readonly BuyingCatalogueDbContext dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    private readonly ILogger<InactiveAccountsService> logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private static List<(int Threshold, InactiveAccountEventType Event)> InactivityThresholdsMap =>
    [
        (30, InactiveAccountEventType.InactivityEnteredFirstExpiryThreshold),
        (14, InactiveAccountEventType.InactivityEnteredSecondExpiryThreshold),
        (7, InactiveAccountEventType.InactivityEnteredThirdExpiryThreshold),
        (1, InactiveAccountEventType.InactivityEnteredForthExpiryThreshold),
        (0, InactiveAccountEventType.InactivityEnteredExpiredThreshold),
    ];

    public async Task<ICollection<AspNetUser>> GetInactiveAccounts(DateOnly utcToday)
    {
        // We will begin sending notifications to users when they
        // reach 30 days from 6 months of inactivity
        var inactivityThresholdDate = GetInactivityStartThresholdDate(utcToday);

        return await dbContext.Users
            .AsNoTracking()
            .Include(x => x.LoginEvents)
            .Include(x => x.Events)
            .Where(u => !u.Disabled 
                &&
                !(
                    u.Events.Any(y => y.EventTypeId == (int)EventTypeEnum.InactivityEnteredFirstExpiryThreshold) &&
                    u.Events.Any(y => y.EventTypeId == (int)EventTypeEnum.InactivityEnteredSecondExpiryThreshold) &&
                    u.Events.Any(y => y.EventTypeId == (int)EventTypeEnum.InactivityEnteredThirdExpiryThreshold) &&
                    u.Events.Any(y => y.EventTypeId == (int)EventTypeEnum.InactivityEnteredForthExpiryThreshold) &&
                    u.Events.Any(y => y.EventTypeId == (int)EventTypeEnum.InactivityEnteredExpiredThreshold)
                )
                &&  
                (
                    (
                        u.ReactivationDate == null &&
                        (
                            u.LoginEvents.Count > 0 && DateOnly.FromDateTime(u.LoginEvents.Max(le => le.Date)) <= inactivityThresholdDate ||
                            u.LoginEvents.Count == 0 && DateOnly.FromDateTime(u.Created) <= inactivityThresholdDate
                        )
                    ) || 
                    u.ReactivationDate != null && DateOnly.FromDateTime(u.ReactivationDate.Value) <= inactivityThresholdDate
                )
             )
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task Raise(
        AspNetUser user,
        DateOnly utcToday,
        EmailPreferenceType defaultEmailPreference,
        bool shouldNotify)
    {
        dbContext.Attach(user);

        var eventType = DetermineEventToRaise(user, utcToday);

        if (eventType == InactiveAccountEventType.Nothing)
        {
            return;
        }

        if (eventType == InactiveAccountEventType.InactivityEnteredExpiredThreshold)
        {
            await DeactivateUserAccount(user.Id);
        }

        user.Events.Add(new AspNetUserEvent((int)eventType));

        if (shouldNotify)
        {
            var notification = await CreateNotification(user, eventType);
            await DispatchNotification(user, notification, eventType);
        }

        await dbContext.SaveChangesAsync();
    }

    private static DateOnly GetInactivityStartThresholdDate(DateOnly utcToday)
    {
        return GetInactivityEndThresholdDate(utcToday).AddDays(30);
    }

    private static DateOnly GetInactivityEndThresholdDate(DateOnly utcToday)
    {
        return utcToday.AddMonths(-6);
    }

    private static InactiveAccountEventType DetermineEventToRaise(AspNetUser user, DateOnly utcToday)
    {
        // we shouldn't trigger this condition when deployed but adding it for completeness
        if (GetInactivityStartThresholdDate(utcToday) < DateOnly.FromDateTime(user.LastLoginDate()))
        {
            return InactiveAccountEventType.Nothing;
        }

        var endThreshold = GetInactivityEndThresholdDate(utcToday);
        var lastLoginDate = user.LastLoginDate();
        var timeSpanSinceLastLogin = lastLoginDate - endThreshold.ToDateTime(TimeOnly.MinValue);

        if (timeSpanSinceLastLogin.Days <= 0)
        {
            return InactiveAccountEventType.InactivityEnteredExpiredThreshold;
        }

        var eventToRaise = InactivityThresholdsMap
            .Where(x => timeSpanSinceLastLogin.Days == x.Threshold)
            .OrderBy(x => x.Threshold)
            .Select(x => x.Event)
            .FirstOrDefault();

        return user.Events.Any(x => x.EventTypeId == (int)eventToRaise)
            ? InactiveAccountEventType.Nothing
            : eventToRaise;
    }

    private async Task<EmailNotification> CreateNotification(AspNetUser user, InactiveAccountEventType eventType)
    {
        var notification = new EmailNotification { To = user.Email };

        if (eventType == InactiveAccountEventType.InactivityEnteredExpiredThreshold)
        {
            await DeactivateUserAccount(user.Id);
            notification.JsonFrom(new AccountDeactivationEmailModel());

            LogUserDeactivationNotice(logger, user.Id);
        }
        else
        {
            var daysFromThreshold = InactivityThresholdsMap.First(x => x.Event == eventType).Threshold;
            notification.JsonFrom(new InactiveAccountEmailModel 
            { 
                DaysFromThreshold = daysFromThreshold
            });

            LogUserInactivityNotice(logger, user.Id, daysFromThreshold);
        }

        dbContext.Add(notification);

        return notification;
    }

    private async Task DispatchNotification(
        AspNetUser user,
        EmailNotification notification,
        InactiveAccountEventType eventType)
    {
        var queueName = options.Value.SendEmailNotifications;
        var client = queueServiceClient.GetQueueClient(queueName);

        try
        {
            await client.SendMessageAsync(Convert.ToBase64String(Encoding.UTF8.GetBytes(notification.Id.ToString())));
        }
        catch (Exception e)
        {
            LogNotificationQueueDispatchError(
                logger,
                e,
                user.Id,
                eventType,
                queueName);

            throw;
        }
    }

    private async Task DeactivateUserAccount(int userId)
    {
        var user = await dbContext.AspNetUsers
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            LogNoUserFoundWarning(logger, userId);
            return;
        }

        user.Disabled = true;
        user.DeactivationReason = AccountDeactivationReason.Inactivity;
        await dbContext.SaveChangesAsync();
    }

    [LoggerMessage(
        EventId = 100,
        Level = LogLevel.Information,
        Message = "Deactiving inactive user account for {UserId} due to inactivity")]
    private static partial void LogUserDeactivationNotice(ILogger logger, int userId);
        
    [LoggerMessage(
        EventId = 200,
        Level = LogLevel.Information,
        Message = "Notifying user {UserId} that their account will be deactiviated in {DaysFromThreshold} due to inactivity")]
    private static partial void LogUserInactivityNotice(ILogger logger, int userId, int daysFromThreshold);
        
    [LoggerMessage(
        EventId = 300,
        Level = LogLevel.Error,
        Message = "{UserId}, {EventType} - Notifications saved but problem dispatching to queue {Queue}")]
    private static partial void LogNotificationQueueDispatchError(
        ILogger logger,
        Exception exception,
        int userId,
        InactiveAccountEventType eventType,
        string queue);

    [LoggerMessage(
        EventId = 400,
        Level = LogLevel.Warning,
        Message = "No user found with id {UserId} to deactivate")]
    private static partial void LogNoUserFoundWarning(ILogger logger, int userId);
}
