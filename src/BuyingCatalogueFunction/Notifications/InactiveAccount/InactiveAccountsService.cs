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

public class InactiveAccountsService(
    BuyingCatalogueDbContext dbContext,
    IOptions<QueueOptions> options,
    QueueServiceClient queueServiceClient,
    ILogger<InactiveAccountsService> logger) : IInactiveAccountsService
{
    private readonly IOptions<QueueOptions> options = options ?? throw new ArgumentNullException(nameof(options));
    private readonly QueueServiceClient queueServiceClient = queueServiceClient ?? throw new ArgumentNullException(nameof(queueServiceClient));
    private readonly BuyingCatalogueDbContext dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    private readonly ILogger<InactiveAccountsService> logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private static List<(int Threshold, InactiveAccountEventTypeEnum Event)> InactivityThresholdsMap =>
    [
        (30, InactiveAccountEventTypeEnum.InactivityEnteredFirstExpiryThreshold),
        (14, InactiveAccountEventTypeEnum.InactivityEnteredSecondExpiryThreshold),
        (7, InactiveAccountEventTypeEnum.InactivityEnteredThirdExpiryThreshold),
        (1, InactiveAccountEventTypeEnum.InactivityEnteredForthExpiryThreshold),
        (0, InactiveAccountEventTypeEnum.InactivityEnteredFifthExpiryThreshold),
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
                && !(u.Events.Any(y => y.EventTypeId == (int)EventTypeEnum.InactivityEnteredFirstExpiryThreshold) &&
                     u.Events.Any(y => y.EventTypeId == (int)EventTypeEnum.InactivityEnteredSecondExpiryThreshold) &&
                     u.Events.Any(y => y.EventTypeId == (int)EventTypeEnum.InactivityEnteredThirdExpiryThreshold) &&
                     u.Events.Any(y => y.EventTypeId == (int)EventTypeEnum.InactivityEnteredForthExpiryThreshold) &&
                     u.Events.Any(y => y.EventTypeId == (int)EventTypeEnum.InactivityEnteredFifthExpiryThreshold))
                && ((u.LoginEvents.Count > 0 && DateOnly.FromDateTime(u.LoginEvents.Max(le => le.Date)) <= inactivityThresholdDate)
                || (u.LoginEvents.Count == 0 && DateOnly.FromDateTime(u.Created) <= inactivityThresholdDate)))
            .ToListAsync();
    }

    public async Task Raise(AspNetUser user, DateOnly utcToday)
    {
        dbContext.Attach(user);

        var eventType = DetermineEventToRaise(user, utcToday);

        if (eventType == InactiveAccountEventTypeEnum.Nothing)
        {
            return;
        }

        var notification = await CreateNotification(user, eventType);

        await DispatchNotification(user, notification, eventType);
    }

    private static DateOnly GetInactivityStartThresholdDate(DateOnly utcToday)
    {
        return GetInactivityEndThresholdDate(utcToday).AddDays(30);
    }

    private static DateOnly GetInactivityEndThresholdDate(DateOnly utcToday)
    {
        return utcToday.AddMonths(-6);
    }

    private static InactiveAccountEventTypeEnum DetermineEventToRaise(AspNetUser user, DateOnly utcToday)
    {
        // we shouldn't trigger this condition when deployed but adding it for completeness
        if (GetInactivityStartThresholdDate(utcToday) < DateOnly.FromDateTime(user.LastLoginDate()))
        {
            return InactiveAccountEventTypeEnum.Nothing;
        }

        var endThreshold = GetInactivityEndThresholdDate(utcToday);
        var lastLoginDate = user.LastLoginDate();
        var timeSpanSinceLastLogin = lastLoginDate - endThreshold.ToDateTime(TimeOnly.MinValue);

        if (timeSpanSinceLastLogin.Days <= 0)
        {
            return InactiveAccountEventTypeEnum.InactivityEnteredFifthExpiryThreshold;
        }

        var eventToRaise = InactivityThresholdsMap.OrderBy(x => x.Threshold)
            .Where(x => timeSpanSinceLastLogin.Days == x.Threshold)
            .Select(x => x.Event)
            .FirstOrDefault();

        return user.Events.Any(x => x.EventTypeId == (int)eventToRaise)
            ? InactiveAccountEventTypeEnum.Nothing
            : eventToRaise;
    }

    private async Task<EmailNotification> CreateNotification(AspNetUser user, InactiveAccountEventTypeEnum eventType)
    {
        user.Events.Add(new AspNetUserEvent((int)eventType));

        var notification = new EmailNotification { To = user.Email };

        if (eventType == InactiveAccountEventTypeEnum.InactivityEnteredFifthExpiryThreshold)
        {
            await DeactivateUserAccount(user.Id);
            notification.JsonFrom(new AccountDeactivationEmailModel());
            logger.LogInformation("Deactiving inactive user account for {UserId} due to inactivity", user.Id);
        }
        else
        {
            var daysFromThreshold = InactivityThresholdsMap.First(x => x.Event == eventType).Threshold;
            notification.JsonFrom(new InactiveAccountEmailModel 
            { 
                DaysFromThreshold = daysFromThreshold
            });

            logger.LogInformation("Notifying user {UserId} that their account will be deactiviated in {DaysFromThreshold} due to inactivity", 
                user.Id,
                daysFromThreshold);
        }

        dbContext.Add(notification);
        await dbContext.SaveChangesAsync();

        return notification;
    }

    private async Task DispatchNotification(
        AspNetUser user,
        EmailNotification notification,
        InactiveAccountEventTypeEnum eventType)
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
                "{UserId}, {EventType} - Notifications saved but problem dispatching to queue {Queue}",
                user.Id, eventType, queueName);
            throw;
        }
    }

    private async Task DeactivateUserAccount(int userId)
    {
        var user = await dbContext.AspNetUsers
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            logger.LogWarning("No user found with id {userId} to deactivate", userId);
        }

        user.Disabled = true;
        user.DeactivationReason = AccountDeactivationReasonEnum.Inactivity;
        await dbContext.SaveChangesAsync();
    }
}
