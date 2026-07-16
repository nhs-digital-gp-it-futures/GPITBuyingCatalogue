using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BuyingCatalogueFunction.Notifications.InactiveAccount;

public class InactiveAccountsFunction(
    ILogger<InactiveAccountsFunction> logger,
    IInactiveAccountsService inactiveAccountsService)
{
    private readonly ILogger logger = logger;
    private readonly IInactiveAccountsService inactiveAccountsService = inactiveAccountsService;

    [Function("InactiveAccountsFunction")]
    public async Task Run([TimerTrigger("0 0 7 * * *", RunOnStartup = true)] TimerInfo timerInfo)
    {
        logger.LogInformation("Inactive Accounts: Executed at {Date}", DateTime.UtcNow);

        if (timerInfo.ScheduleStatus is not null)
        {
            logger.LogInformation("Inactive Accounts: Next timer schedule at {Next}", timerInfo.ScheduleStatus.Next);
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
            logger.LogInformation("Inactive Accounts: No inactive users found");
            return;
        }

        foreach (var user in users)
        {
            try
            {
                await inactiveAccountsService.Raise(user, utcToday);
            }
            catch (Exception)
            {
                logger.LogError("Inactive Accounts: User {UserId}. Check inner exception", user.Id);
            }
        }
    }
}
