using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels.OrderingInformationModels;
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
    private readonly IServiceRecipientImportService serviceRecipientImportService;

    public CompetitionResultsController(
        ICompetitionsService competitionsService,
        IManageFiltersService filtersService,
        IPdfService pdfService,
        IServiceRecipientImportService serviceRecipientImportService)
    {
        this.competitionsService = competitionsService ?? throw new ArgumentNullException(nameof(competitionsService));
        this.filtersService = filtersService ?? throw new ArgumentNullException(nameof(filtersService));
        this.pdfService = pdfService ?? throw new ArgumentNullException(nameof(pdfService));
        this.serviceRecipientImportService = serviceRecipientImportService
            ?? throw new ArgumentNullException(nameof(serviceRecipientImportService));
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
            PdfUrl = Url.Action(nameof(DownloadConfirmResults), new { internalOrgId, competitionId }),
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
        return View("ViewResults", model);
    }

    [HttpGet("direct-award")]
    public async Task<IActionResult> DirectAward(
       string internalOrgId,
       int competitionId)
    {
        var competition = await competitionsService.GetCompetitionForResults(internalOrgId, competitionId);
        var nonShortlistedSolutions = await competitionsService.GetNonShortlistedSolutions(internalOrgId, competitionId);
        var filterDetails = await filtersService.GetFilterDetails(competition.OrganisationId, competition.FilterId);

        var model = new FilteredDirectAwardModel(competition, filterDetails, nonShortlistedSolutions)
        {
            BackLink = Url.Action(
                nameof(CompetitionsDashboardController.Index),
                typeof(CompetitionsDashboardController).ControllerName(),
                new { internalOrgId }),
            PdfUrl = Url.Action(nameof(DownloadResults), new { internalOrgId, competitionId }),
        };
        return View("FilteredDirectAward", model);
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

        var fileName = $"competition-results-{competition.Id}.pdf";
        return File(result, "application/pdf", fileName);
    }

    [HttpGet("downloadConfirm")]
    public async Task<IActionResult> DownloadConfirmResults(
        string internalOrgId,
        int competitionId)
    {
        var competition = await competitionsService.GetCompetitionForResults(internalOrgId, competitionId);

        if (competition == null) return NotFound();

        var uri = Url.Action(
            nameof(CompetitionPdfController.ConfirmResults),
            typeof(CompetitionPdfController).ControllerName(),
            new { internalOrgId, competitionId, });

        var result = await pdfService.Convert(new(pdfService.BaseUri(), uri));

        const string fileName = "review-scoring.pdf";
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
    public IActionResult SelectWinningSolution(
        string internalOrgId,
        int competitionId,
        SelectWinningSolutionModel model)
    {
        _ = internalOrgId;
        _ = competitionId;

        if (!ModelState.IsValid)
            return View(model);

        return RedirectToAction(
            nameof(OrderingInformation),
            new { internalOrgId, competitionId, solutionId = model.SolutionId });
    }

    [HttpGet("ordering-information")]
    public async Task<IActionResult> OrderingInformation(
        string internalOrgId,
        int competitionId,
        CatalogueItemId? solutionId = null)
    {
        bool WinningSolutionSelector(CompetitionSolution competitionSolution)
        {
            return solutionId is not null
                ? competitionSolution.SolutionId == solutionId
                : competitionSolution.IsWinningSolution;
        }

        var competition = await competitionsService.GetCompetitionForResults(internalOrgId, competitionId);
        var solution = competition.CompetitionSolutions.FirstOrDefault(WinningSolutionSelector);

        if (solution is null || !solution.IsWinningSolution)
            return RedirectToAction(nameof(ViewResults), new { internalOrgId, competitionId });

        var model = new OrderingInformationModel(competition, solution)
        {
            BackLink = Url.Action(
                solutionId is not null ? nameof(SelectWinningSolution) : nameof(ViewResults),
                new { internalOrgId, competitionId }),
        };

        return View(model);
    }

    [HttpGet("recipients-csv")]
    public async Task<IActionResult> RecipientsCsv(
        string internalOrgId,
        int competitionId)
    {
        var competition = await competitionsService.GetCompetitionWithRecipients(internalOrgId, competitionId);
        var recipients = competition.Recipients.Select(
            x => new ServiceRecipientImportModel { Organisation = x.Name, OdsCode = x.Id, });

        using var stream = new MemoryStream();
        await serviceRecipientImportService.CreateServiceRecipientTemplate(stream, recipients);
        stream.Position = 0;

        return File(stream.ToArray(), "application/octet-stream", "service_recipient_export.csv");
    }
}
