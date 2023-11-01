using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf;
using NHSD.GPIT.BuyingCatalogue.Services.Pdf;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ScoringModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;

[Authorize("Development")]
[Authorize("Buyer")]
[Area("Competitions")]
[Route("organisation/{internalOrgId}/competitions/{competitionId:int}/scoring")]
public class CompetitionScoringController : Controller
{
    private readonly ICompetitionsService competitionsService;
    private readonly IPdfService pdfService;

    public CompetitionScoringController(
        ICompetitionsService competitionsService,
        IPdfService pdfService)
    {
        this.competitionsService = competitionsService ?? throw new ArgumentNullException(nameof(competitionsService));
        this.pdfService = pdfService ?? throw new ArgumentNullException(nameof(pdfService));
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
        var competition = await competitionsService.GetCompetitionWithSolutions(internalOrgId, competitionId);

        var model = new InteroperabilityScoringModel(competition)
        {
            BackLink = Url.Action(nameof(Index), new { internalOrgId, competitionId }),
            PdfUrl = Url.Action(nameof(InteropPdf), new { internalOrgId, competitionId }),
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
                await competitionsService.GetCompetitionWithSolutions(internalOrgId, competitionId);

            model.WithInteroperability(competition.NonPriceElements.Interoperability)
                .WithSolutions(competition.CompetitionSolutions, false);

            return View(model);
        }

        var solutionsAndScores = model.SolutionScores.ToDictionary(x => x.SolutionId, x => (x.Score.GetValueOrDefault(), x.Justification));

        await competitionsService.SetSolutionsInteroperabilityScores(internalOrgId, competitionId, solutionsAndScores);

        return RedirectToAction(nameof(Index), new { internalOrgId, competitionId });
    }

    [HttpGet("implementation")]
    public async Task<IActionResult> Implementation(
        string internalOrgId,
        int competitionId)
    {
        var competition =
            await competitionsService.GetCompetitionWithSolutions(internalOrgId, competitionId);

        var model = new ImplementationScoringModel(competition)
        {
            BackLink = Url.Action(nameof(Index), new { internalOrgId, competitionId }),
            PdfUrl = Url.Action(nameof(ImplementationPdf), new { internalOrgId, competitionId }),
        };

        return View(model);
    }

    [HttpPost("implementation")]
    public async Task<IActionResult> Implementation(
        string internalOrgId,
        int competitionId,
        ImplementationScoringModel model)
    {
        if (!ModelState.IsValid)
        {
            var competition =
                await competitionsService.GetCompetitionWithSolutions(internalOrgId, competitionId);

            model.WithSolutions(competition.CompetitionSolutions, false);

            return View(model);
        }

        var solutionsAndScores = model.SolutionScores.ToDictionary(x => x.SolutionId, x => (x.Score.GetValueOrDefault(), x.Justification));

        await competitionsService.SetSolutionsImplementationScores(internalOrgId, competitionId, solutionsAndScores);

        return RedirectToAction(nameof(Index), new { internalOrgId, competitionId });
    }

    [HttpGet("service-level")]
    public async Task<IActionResult> ServiceLevel(
        string internalOrgId,
        int competitionId)
    {
        var competition = await competitionsService.GetCompetitionWithSolutions(internalOrgId, competitionId);

        var model = new ServiceLevelScoringModel(competition)
        {
            BackLink = Url.Action(nameof(Index), new { internalOrgId, competitionId }),
            PdfUrl = Url.Action(nameof(ServiceLevelPdf), new { internalOrgId, competitionId }),
        };

        return View(model);
    }

    [HttpPost("service-level")]
    public async Task<IActionResult> ServiceLevel(
        string internalOrgId,
        int competitionId,
        ServiceLevelScoringModel model)
    {
        if (!ModelState.IsValid)
        {
            var competition = await competitionsService.GetCompetitionWithSolutions(internalOrgId, competitionId);

            model.WithSolutions(competition.CompetitionSolutions, false);

            return View(model);
        }

        var solutionsAndScores = model.SolutionScores.ToDictionary(x => x.SolutionId, x => (x.Score.GetValueOrDefault(), x.Justification));

        await competitionsService.SetSolutionsServiceLevelScores(internalOrgId, competitionId, solutionsAndScores);

        return RedirectToAction(nameof(Index), new { internalOrgId, competitionId });
    }

    [HttpGet("features")]
    public async Task<IActionResult> Features(
        string internalOrgId,
        int competitionId)
    {
        var competition = await competitionsService.GetCompetitionWithSolutions(internalOrgId, competitionId);

        var model = new FeaturesScoringModel(competition)
        {
            BackLink = Url.Action(nameof(Index), new { internalOrgId, competitionId }),
            PdfUrl = Url.Action(nameof(FeaturePdf), new { internalOrgId, competitionId }),
        };

        return View(model);
    }

    [HttpPost("features")]
    public async Task<IActionResult> Features(
        string internalOrgId,
        int competitionId,
        FeaturesScoringModel model)
    {
        if (!ModelState.IsValid)
        {
            var competition = await competitionsService.GetCompetitionWithSolutions(internalOrgId, competitionId);

            model.WithSolutions(competition.CompetitionSolutions, false);

            return View(model);
        }

        var solutionsAndScores = model.SolutionScores.ToDictionary(x => x.SolutionId, x => (x.Score.GetValueOrDefault(), x.Justification));

        await competitionsService.SetSolutionsFeaturesScores(internalOrgId, competitionId, solutionsAndScores);

        return RedirectToAction(nameof(Index), new { internalOrgId, competitionId });
    }

    [HttpGet("featurePdf")]
    public async Task<IActionResult> FeaturePdf(
        string internalOrgId,
        int competitionId)
    {
        var competition = await competitionsService.GetCompetition(internalOrgId, competitionId);

        if (competition == null) return NotFound();

        var uri = Url.Action(
            nameof(CompetitionFeaturesScoringPdfController.Index),
            typeof(CompetitionFeaturesScoringPdfController).ControllerName(),
            new { internalOrgId, competitionId, });

        var result = await pdfService.Convert(new(pdfService.BaseUri(), uri));

        var fileName = "compare-features.pdf";
        return File(result, "application/pdf", fileName);
    }

    [HttpGet("implementationPdf")]
    public async Task<IActionResult> ImplementationPdf(
        string internalOrgId,
        int competitionId)
    {
        var competition = await competitionsService.GetCompetition(internalOrgId, competitionId);

        if (competition == null) return NotFound();

        var uri = Url.Action(
            nameof(CompetitionScoringImplementationPdfController.Index),
            typeof(CompetitionScoringImplementationPdfController).ControllerName(),
            new { internalOrgId, competitionId, });

        var result = await pdfService.Convert(new(pdfService.BaseUri(), uri));

        var fileName = "compare-implementation.pdf";
        return File(result, "application/pdf", fileName);
    }

    [HttpGet("interopPdf")]
    public async Task<IActionResult> InteropPdf(
        string internalOrgId,
        int competitionId)
    {
        var competition = await competitionsService.GetCompetition(internalOrgId, competitionId);

        if (competition == null) return NotFound();

        var uri = Url.Action(
            nameof(CompetitionScoringInteropPdfController.Index),
            typeof(CompetitionScoringInteropPdfController).ControllerName(),
            new { internalOrgId, competitionId, });

        var result = await pdfService.Convert(new(pdfService.BaseUri(), uri));

        var fileName = "compare-interoperability.pdf";
        return File(result, "application/pdf", fileName);
    }

    [HttpGet("serviceLevelPdf")]
    public async Task<IActionResult> ServiceLevelPdf(
        string internalOrgId,
        int competitionId)
    {
        var competition = await competitionsService.GetCompetition(internalOrgId, competitionId);

        if (competition == null) return NotFound();

        var uri = Url.Action(
            nameof(CompetitionScoringServiceLevelPdfController.Index),
            typeof(CompetitionScoringServiceLevelPdfController).ControllerName(),
            new { internalOrgId, competitionId, });

        var result = await pdfService.Convert(new(pdfService.BaseUri(), uri));

        var fileName = "compare-sla.pdf";
        return File(result, "application/pdf", fileName);
    }
}
