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
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ActionArguments.TryGetValue("competitionId", out var routeCompetitionId) || !int.TryParse(routeCompetitionId!.ToString(), out var competitionId))
            return;

        var organisationService = context.HttpContext.RequestServices.GetRequiredService<IOrganisationsService>();
        var competitionsService = context.HttpContext.RequestServices.GetRequiredService<ICompetitionsService>();

        var internalOrgId = context.HttpContext.User.GetPrimaryOrganisationInternalIdentifier();

        var organisation = await organisationService.GetOrganisationByInternalIdentifier(internalOrgId);
        var competition = await competitionsService.GetCompetition(organisation.Id, competitionId);

        if (competition.IsShortlistLocked || competition.Completed.HasValue)
        {
            context.Result = new RedirectToActionResult(
                nameof(CompetitionsDashboardController.Index),
                typeof(CompetitionsDashboardController).ControllerName(),
                new { internalOrgId });
        }

        await base.OnActionExecutionAsync(context, next);
    }
}
