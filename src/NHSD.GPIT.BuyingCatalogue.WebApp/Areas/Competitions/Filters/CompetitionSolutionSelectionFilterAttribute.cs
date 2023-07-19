using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Filters;

public class CompetitionSolutionSelectionFilterAttribute : ActionFilterAttribute
{
    internal const string InternalOrgIdKey = "internalOrgId";
    internal const string CompetitionIdKey = "competitionId";

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ActionArguments.TryGetValue(InternalOrgIdKey, out var internalOrgId)
            || !context.ActionArguments.TryGetValue(CompetitionIdKey, out var routeCompetitionId)
            || routeCompetitionId is null
            || !int.TryParse(routeCompetitionId.ToString(), out var competitionId))
        {
            await base.OnActionExecutionAsync(context, next);
            return;
        }

        var competitionsService = context.HttpContext.RequestServices.GetRequiredService<ICompetitionsService>();

        var competition = await competitionsService.GetCompetition(internalOrgId.ToString(), competitionId);

        if (competition.IsShortlistAccepted || competition.Completed.HasValue)
        {
            context.Result = new RedirectToActionResult(
                nameof(CompetitionsDashboardController.Index),
                typeof(CompetitionsDashboardController).ControllerName(),
                new { internalOrgId });
        }

        await base.OnActionExecutionAsync(context, next);
    }
}
