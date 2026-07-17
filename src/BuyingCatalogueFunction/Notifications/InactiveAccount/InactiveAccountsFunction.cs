using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BuyingCatalogueFunction.Notifications.InactiveAccount;

public partial class InactiveAccountsFunction(
    ILogger<InactiveAccountsFunction> logger,
    IInactiveAccountsService inactiveAccountsService)
{
    private readonly ILogger logger = logger;
    private readonly IInactiveAccountsService inactiveAccountsService = inactiveAccountsService;

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
        logger.LogInformation("Inactive Accounts: Evaluating Inactive Users");
        var utcToday = DateOnly.FromDateTime(DateTime.UtcNow);
        var users = await inactiveAccountsService.GetInactiveAccounts(utcToday);

        if (users.Count == 0)
        {
            LogNoInactiveUsers(logger);
            return;
        }

        foreach (var user in users)
        {
            try
            {
                await inactiveAccountsService.Raise(user, utcToday);
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
        Level = LogLevel.Error,
        Message = "Inactive Accounts: Exception raising a deactivation notice for User {UserId}")]
    private static partial void LogInactityNotificationError(
        ILogger logger,
        Exception exception,
        int userId);
}
