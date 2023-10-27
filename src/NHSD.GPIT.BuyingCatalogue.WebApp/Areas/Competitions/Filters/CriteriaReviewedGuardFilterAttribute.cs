using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Filters;

public class CriteriaReviewedGuardFilterAttribute : CompetitionFilterBaseAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var competition = await GetCompetitionFromRoute(context);
        if (competition is null)
        {
            await base.OnActionExecutionAsync(context, next);
            return;
        }

        if (competition.HasReviewedCriteria)
        {
            context.Result = new RedirectToActionResult(
                nameof(CompetitionTaskListController.Index),
                typeof(CompetitionTaskListController).ControllerName(),
                new { internalOrgId = competition.Organisation.InternalIdentifier, competitionId = competition.Id });
        }

        await base.OnActionExecutionAsync(context, next);
    }
}
