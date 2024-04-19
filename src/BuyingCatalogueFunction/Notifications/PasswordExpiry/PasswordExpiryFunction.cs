using System;
using System.Threading.Tasks;
using BuyingCatalogueFunction.Notifications.Interfaces;
using BuyingCatalogueFunction.Notifications.PasswordExpiry.Interfaces;
using BuyingCatalogueFunction.Notifications.PasswordExpiry.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace BuyingCatalogueFunction.Notifications.PasswordExpiry;

public class PasswordExpiryFunction
{
    private readonly ILogger logger;
    private readonly IPasswordExpiryService passwordExpiryService;
    private readonly IEmailPreferenceService emailPreferenceService;

    public PasswordExpiryFunction(
        ILogger<PasswordExpiryService> logger,
        IPasswordExpiryService passwordExpiryService,
        IEmailPreferenceService emailPreferenceService)
    {
        this.logger = logger;
        this.passwordExpiryService = passwordExpiryService;
        this.emailPreferenceService = emailPreferenceService;
    }

    [Function("PasswordExpiryFunction")]
    public async Task Run([TimerTrigger("0 0 10 * * *")] TimerInfo timerInfo)
    {
        var utcNow = DateTime.UtcNow;

        logger.LogInformation("Password Expiry: Executed at {Date}", utcNow);

        if (timerInfo.ScheduleStatus is not null)
        {
            logger.LogInformation("Password Expiry: Next timer schedule at {Next}", timerInfo.ScheduleStatus.Next);
        }

        await RunFor(utcNow.Date);
    }

    private async Task RunFor(DateTime today)
    {
        logger.LogInformation("Password Expiry: Evaluating Users for {Date}", today);
        var users = await passwordExpiryService.GetUsersNearingPasswordExpiry(today);

        if (users.Count == 0)
        {
            logger.LogInformation("Password Expiry: No users found nearing password expiry for {Date}", today);
            return;
        }

        var defaultEmailPreference =
            await emailPreferenceService.GetDefaultEmailPreference(EmailPreferenceTypeEnum.PasswordExpiry);
        if (defaultEmailPreference is null)
        {
            logger.LogWarning(
                "Password Expiry: {EmailPreferenceType} not found or a ManagedEmailPreference is not configured",
                EmailPreferenceTypeEnum.PasswordExpiry);

            return;
        }

        foreach (var user in users)
        {
            var shouldProcess = await emailPreferenceService.ShouldTriggerForUser(defaultEmailPreference, user.Id);
            if (!shouldProcess) continue;

            try
            {
                await Evaluate(today, user);
            }
            catch (Exception e)
            {
                logger.LogError("Password Expiry: User {UserId}. Check inner exception", user.Id);
            }
        }
    }

    private async Task Evaluate(DateTime today, AspNetUser user)
    {
        var eventToRaise = user.DetermineEventToRaise(today);
        if (eventToRaise == EventTypeEnum.Nothing)
        {
            logger.LogInformation("Password Expiry: User {UserId}. No event to raise", user.Id);
            return;
        }

        logger.LogInformation("Password Expiry: User {UserId}. Raising {EventToRaise}", user.Id, eventToRaise);
        await passwordExpiryService.Raise(today, user, eventToRaise);
    }
}
