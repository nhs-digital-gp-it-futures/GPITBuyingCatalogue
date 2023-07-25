using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ScoringModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;

[Authorize("Development")]
[Authorize("Buyer")]
[Area("Competitions")]
[Route("organisation/{internalOrgId}/competitions/{competitionId:int}/scoring")]
public class CompetitionScoringController : Controller
{
    private readonly ICompetitionsService competitionsService;

    public CompetitionScoringController(
        ICompetitionsService competitionsService)
    {
        this.competitionsService = competitionsService ?? throw new ArgumentNullException(nameof(competitionsService));
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string internalOrgId,
        int competitionId)
    {
        var competition = await competitionsService.GetCompetitionWithNonPriceElements(internalOrgId, competitionId);

        var model = new NonPriceElementScoresDashboardModel(competition)
        {
            BackLink = Url.Action(
                nameof(CompetitionTaskListController.Index),
                typeof(CompetitionTaskListController).ControllerName(),
                new { internalOrgId, competitionId }),
        };

        return View(model);
    }

    [HttpGet("interoperability")]
    public async Task<IActionResult> Interoperability(
        string internalOrgId,
        int competitionId)
    {
        var competition = await competitionsService.GetCompetitionWithSolutionsInterop(internalOrgId, competitionId);

        var model = new InteroperabilityScoringModel(competition)
        {
            BackLink = Url.Action(nameof(Index), new { internalOrgId, competitionId }),
        };

        return View(model);
    }

    [HttpPost("interoperability")]
    public async Task<IActionResult> Interoperability(
        string internalOrgId,
        int competitionId,
        InteroperabilityScoringModel model)
    {
        if (!ModelState.IsValid)
        {
            var competition =
                await competitionsService.GetCompetitionWithSolutionsInterop(internalOrgId, competitionId);

            model.WithInteroperability(competition.NonPriceElements.Interoperability)
                .WithSolutions(competition.CompetitionSolutions);

            return View(model);
        }

        var solutionsAndScores = model.SolutionScores.ToDictionary(x => x.SolutionId, x => x.Score.GetValueOrDefault());

        await competitionsService.SetSolutionsInteroperabilityScores(internalOrgId, competitionId, solutionsAndScores);

        return RedirectToAction(nameof(Index), new { internalOrgId, competitionId });
    }
}
