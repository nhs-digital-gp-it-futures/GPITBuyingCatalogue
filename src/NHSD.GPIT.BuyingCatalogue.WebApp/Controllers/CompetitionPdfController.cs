using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.CompetitionPdfModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;

[Route("organisation/{internalOrgId}/competitions/{competitionId:int}/pdf")]
[RestrictToLocalhostActionFilter]
public class CompetitionPdfController : Controller
{
    private readonly ICompetitionsService competitionsService;

    public CompetitionPdfController(
        ICompetitionsService competitionsService)
    {
        this.competitionsService = competitionsService ?? throw new ArgumentNullException(nameof(competitionsService));
    }

    [HttpGet("confirm-results-pdf")]
    public async Task<IActionResult> ConfirmResults(
        string internalOrgId,
        int competitionId)
    {
        var competition = await competitionsService.GetCompetitionForResults(internalOrgId, competitionId);

        if (competition == null) return NotFound();

        var model = new PdfConfirmResultsModel(competition);

        return View(model);
    }

    [HttpGet("implementation-scoring-pdf")]
    public Task<IActionResult> ImplementationScoring(
        string internalOrgId,
        int competitionId) => GetScoringView(internalOrgId, competitionId, c => new PdfScoringImplementationModel(c));

    [HttpGet("interop-scoring-pdf")]
    public Task<IActionResult> InteropScoring(
        string internalOrgId,
        int competitionId) => GetScoringView(internalOrgId, competitionId, c => new PdfScoringInteropModel(c));

    [HttpGet("service-level-scoring-pdf")]
    public Task<IActionResult> ServiceLevelScoring(
        string internalOrgId,
        int competitionId) => GetScoringView(internalOrgId, competitionId, c => new PdfScoringServiceLevelModel(c));

    [HttpGet("features-scoring-pdf")]
    public Task<IActionResult> FeaturesScoring(
        string internalOrgId,
        int competitionId) => GetScoringView(internalOrgId, competitionId, c => new PdfFeaturesScoringModel(c));

    internal async Task<IActionResult> GetScoringView<TModel>(
        string internalOrgId,
        int competitionId,
        Func<Competition, TModel> modelFactory,
        [CallerMemberName] string viewName = null)
        where TModel : class
    {
        var competition = await competitionsService.GetCompetitionWithSolutions(internalOrgId, competitionId);

        if (competition == null) return NotFound();

        var model = modelFactory(competition);

        return View(viewName, model);
    }
}
