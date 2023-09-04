using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;

[Authorize("Development")]
[Authorize("Buyer")]
[Area("Competitions")]
[Route("organisation/{internalOrgId}/competitions/{competitionId:int}/results")]
public class CompetitionResultsController : Controller
{
    private readonly ICompetitionsService competitionsService;

    public CompetitionResultsController(
        ICompetitionsService competitionsService)
    {
        this.competitionsService = competitionsService;
    }

    [HttpGet("confirm")]
    public async Task<IActionResult> Confirm(
        string internalOrgId,
        int competitionId)
    {
        var competition = await competitionsService.GetCompetitionForResults(internalOrgId, competitionId);

        var model = new ConfirmResultsModel(competition)
        {
            BackLink = Url.Action(
                nameof(CompetitionTaskListController.Index),
                typeof(CompetitionTaskListController).ControllerName(),
                new { internalOrgId, competitionId }),
        };

        return View(model);
    }

    [HttpPost("confirm")]
    public async Task<IActionResult> Confirm(
        string internalOrgId,
        int competitionId,
        ConfirmResultsModel model)
    {
        _ = model;

        await competitionsService.CompleteCompetition(internalOrgId, competitionId);

        return RedirectToAction(
            nameof(ViewResults),
            new { internalOrgId, competitionId });
    }

    [HttpGet("view")]
    public async Task<IActionResult> ViewResults(
        string internalOrgId,
        int competitionId)
    {
        var competition = await competitionsService.GetCompetitionForResults(internalOrgId, competitionId);

        var model = new ViewResultsModel(competition)
        {
            BackLink = Url.Action(
                nameof(CompetitionsDashboardController.Index),
                typeof(CompetitionsDashboardController).ControllerName(),
                new { internalOrgId }),
        };

        return View(model);
    }
}
