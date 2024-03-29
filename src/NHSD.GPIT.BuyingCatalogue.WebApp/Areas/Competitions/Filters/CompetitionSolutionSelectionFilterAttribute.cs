using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Filters;

public class CompetitionSolutionSelectionFilterAttribute : CompetitionFilterBaseAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var competition = await GetCompetitionFromRoute(context);
        if (competition is null)
        {
            await base.OnActionExecutionAsync(context, next);
            return;
        }

        if (competition.IsShortlistAccepted || competition.Completed.HasValue)
        {
            context.Result = new RedirectToActionResult(
                nameof(CompetitionsDashboardController.Index),
                typeof(CompetitionsDashboardController).ControllerName(),
                new { internalOrgId = competition.Organisation.InternalIdentifier });
        }

        await base.OnActionExecutionAsync(context, next);
    }
}
