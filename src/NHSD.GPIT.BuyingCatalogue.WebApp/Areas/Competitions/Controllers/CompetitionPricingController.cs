using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.PricingModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;

[Authorize("Development")]
[Authorize("Buyer")]
[Area("Competitions")]
[Route("organisation/{internalOrgId}/competitions/{competitionId:int}/pricing")]
public class CompetitionPricingController : Controller
{
    private readonly ICompetitionsService competitionsService;

    public CompetitionPricingController(
        ICompetitionsService competitionsService)
    {
        this.competitionsService = competitionsService ?? throw new ArgumentNullException(nameof(competitionsService));
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string internalOrgId,
        int competitionId)
    {
        var competition = await competitionsService.GetCompetitionWithSolutions(internalOrgId, competitionId);

        var model = new PricingDashboardModel(competition)
        {
            BackLink = Url.Action(
                nameof(CompetitionTaskListController.Index),
                typeof(CompetitionTaskListController).ControllerName(),
                new { internalOrgId, competitionId }),
            InternalOrgId = internalOrgId,
        };

        return View(model);
    }

    [HttpGet("{catalogueItemId}")]
    public async Task<IActionResult> Hub(
        string internalOrgId,
        int competitionId,
        CatalogueItemId catalogueItemId)
    {
        var competition = await competitionsService.GetCompetitionWithSolutions(internalOrgId, competitionId);
        var solution = competition.CompetitionSolutions.FirstOrDefault(x => x.SolutionId == catalogueItemId);

        var model = new CompetitionSolutionHubModel(solution, competition.CompetitionRecipients)
        {
            BackLink = Url.Action(nameof(Index), new { internalOrgId, competitionId }),
        };

        return View(model);
    }
}
