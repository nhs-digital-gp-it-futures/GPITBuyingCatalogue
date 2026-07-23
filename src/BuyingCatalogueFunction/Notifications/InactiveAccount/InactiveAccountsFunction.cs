using System;
using System.Threading.Tasks;
using BuyingCatalogueFunction.Notifications.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;

namespace BuyingCatalogueFunction.Notifications.InactiveAccount;

public partial class InactiveAccountsFunction(
    ILogger<InactiveAccountsFunction> logger,
    IInactiveAccountsService inactiveAccountsService,
    IEmailPreferenceService emailPreferenceService)
{
    private readonly ILogger<InactiveAccountsFunction> logger = logger;
    private readonly IInactiveAccountsService inactiveAccountsService = inactiveAccountsService;
    private readonly IEmailPreferenceService emailPreferenceService = emailPreferenceService;

    [Function("InactiveAccountsFunction")]
    public async Task Run([TimerTrigger("0 0 7 * * *", RunOnStartup = true)] TimerInfo timerInfo)
    {
        LogExecuteDateTime(logger, DateTime.UtcNow);

        if (timerInfo.ScheduleStatus is not null)
        {
            LogNextScheduledDateTime(logger, timerInfo.ScheduleStatus.Next);
        }

        await Run();
    }

    private async Task Run()
    {
        LogEvaluatingInactiveUsers(logger);

        var utcToday = DateOnly.FromDateTime(DateTime.UtcNow);
        var users = await inactiveAccountsService.GetInactiveAccounts(utcToday);

        if (users.Count == 0)
        {
            LogNoInactiveUsers(logger);
            return;
        }

        var defaultEmailPreference = await emailPreferenceService
            .GetDefaultEmailPreference(EmailPreferenceTypeEnum.InactiveAccount);

        if (defaultEmailPreference is null)
        {
            LogManagedEmailPreferenceNotConfigured(logger, EmailPreferenceTypeEnum.InactiveAccount);
        }

        foreach (var user in users)
        {
            try
            {
                var shouldNotify = await emailPreferenceService.ShouldTriggerForUser(defaultEmailPreference, user.Id);
                await inactiveAccountsService.Raise(user, utcToday, defaultEmailPreference, shouldNotify);
            }
            catch (Exception e)
            {
                LogInactityNotificationError(logger, e, user.Id);
            }
        }
    }

    [LoggerMessage(
        EventId = 100,
        Level = LogLevel.Information,
        Message = "Inactive Accounts: Executed at {Date}")]
    private static partial void LogExecuteDateTime(ILogger logger, DateTime date);

    [LoggerMessage(
        EventId = 200,
        Level = LogLevel.Information,
        Message = "Inactive Accounts: Next timer schedule at {Next}")]
    private static partial void LogNextScheduledDateTime(ILogger logger, DateTime next);

    [LoggerMessage(
        EventId = 300,
        Level = LogLevel.Information,
        Message = "Inactive Accounts: No inactive users found")]
    private static partial void LogNoInactiveUsers(ILogger logger);

    [LoggerMessage(
        EventId = 400,
        Level = LogLevel.Information,
        Message = "Inactive Accounts: Evaluating Inactive Users")]
    private static partial void LogEvaluatingInactiveUsers(ILogger logger);

    [LoggerMessage(
        EventId = 500,
        Level = LogLevel.Warning,
        Message = "Inactive Accounts: {EmailPreferenceType} not found or a ManagedEmailPreference is not configured")]
    private static partial void LogManagedEmailPreferenceNotConfigured(
        ILogger logger, 
        EmailPreferenceTypeEnum emailPreferenceType);

    [LoggerMessage(
        EventId = 600,
        Level = LogLevel.Error,
        Message = "Inactive Accounts: Exception raising a deactivation notice for User {UserId}")]
    private static partial void LogInactityNotificationError(
        ILogger logger,
        Exception exception,
        int userId);
}
