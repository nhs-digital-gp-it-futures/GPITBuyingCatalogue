using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;

[Authorize("Development")]
[Authorize("Buyer")]
[Area("Competitions")]
[Route("organisation/{internalOrgId}/competitions/{competitionId:int}/results")]
public class CompetitionResultsController : Controller
{
    private readonly ICompetitionsService competitionsService;
    private readonly IManageFiltersService filtersService;
    private readonly IPdfService pdfService;

    public CompetitionResultsController(
        ICompetitionsService competitionsService,
        IManageFiltersService filtersService,
        IPdfService pdfService)
    {
        this.competitionsService = competitionsService ?? throw new ArgumentNullException(nameof(competitionsService));
        this.filtersService = filtersService ?? throw new ArgumentNullException(nameof(filtersService));
        this.pdfService = pdfService ?? throw new ArgumentNullException(nameof(pdfService));
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
            InternalOrgId = internalOrgId,
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
        var nonShortlistedSolutions = await competitionsService.GetNonShortlistedSolutions(internalOrgId, competitionId);
        var filterDetails = await filtersService.GetFilterDetails(competition.OrganisationId, competition.FilterId);

        var model = new ViewResultsModel(competition, filterDetails, nonShortlistedSolutions)
        {
            BackLink = Url.Action(
                nameof(CompetitionsDashboardController.Index),
                typeof(CompetitionsDashboardController).ControllerName(),
                new { internalOrgId }),
            PdfUrl = Url.Action(nameof(DownloadResults), new { internalOrgId, competitionId }),
        };

        return View(model);
    }

    [HttpGet("download")]
    public async Task<IActionResult> DownloadResults(
        string internalOrgId,
        int competitionId)
    {
        var competition = await competitionsService.GetCompetition(internalOrgId, competitionId);

        if (competition == null) return NotFound();

        var uri = Url.Action(
            nameof(CompetitionResultsPdfController.Index),
            typeof(CompetitionResultsPdfController).ControllerName(),
            new { internalOrgId, competitionId, });

        var result = await pdfService.Convert(new(pdfService.BaseUri(), uri));

        var fileName = $"{competition.Name}.pdf";
        return File(result, "application/pdf", fileName);
    }

    [HttpGet("select-winning-solution")]
    public async Task<IActionResult> SelectWinningSolution(
        string internalOrgId,
        int competitionId)
    {
        var competition = await competitionsService.GetCompetitionForResults(internalOrgId, competitionId);
        var winningSolutions = competition.CompetitionSolutions.Where(x => x.IsWinningSolution);

        var model = new SelectWinningSolutionModel(competition.Name, winningSolutions.Select(x => x.Solution))
        {
            BackLink = Url.Action(nameof(ViewResults), new { internalOrgId, competitionId }),
        };

        return View(model);
    }

    [HttpPost("select-winning-solution")]
    public async Task<IActionResult> SelectWinningSolution(
        string internalOrgId,
        int competitionId,
        SelectWinningSolutionModel model)
    {
        _ = internalOrgId;
        _ = competitionId;

        if (!ModelState.IsValid)
            return View(model);

        await Task.Yield();

        return View(model);
    }
}
