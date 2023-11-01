using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.CompetitionFeaturesScoringPdf;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.CompetitionScoringInteropPdf;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;

[Route("organisation/{internalOrgId}/competitions/{competitionId:int}/interop/pdf")]
[RestrictToLocalhostActionFilter]
public class CompetitionScoringInteropPdfController : Controller
{
    private readonly ICompetitionsService competitionsService;

    public CompetitionScoringInteropPdfController(
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

        if (competition == null) return NotFound();

        var model = new PdfScoringInteropModel(competition);

        return View(model);
    }
}
