using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests;

public static class ActionFilterExtensions
{
    /// <summary>
    /// Calls OnActionExecutionAsync and passes in a stubbed <see cref="ActionExecutionDelegate"/>.
    /// </summary>
    /// <param name="actionFilter">The action filter.</param>
    /// <param name="context">The <see cref="ActionExecutingContext"/>.</param>
    /// <param name="executedContext">The <see cref="ActionExecutedContext"/>.</param>
    /// <returns>A boolean indicating whether the <see cref="ActionExecutionDelegate"/> was called.</returns>
    public static async Task<bool> TestOnActionExecutionAsync(this IAsyncActionFilter actionFilter, ActionExecutingContext context, ActionExecutedContext executedContext)
    {
        bool called = false;

        await actionFilter.OnActionExecutionAsync(context, NextDelegate);

        return called;

        Task<ActionExecutedContext> NextDelegate()
        {
            called = true;
            return Task.FromResult(executedContext);
        }
    }
}
