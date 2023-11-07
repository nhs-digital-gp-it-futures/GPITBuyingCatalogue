using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.CompetitionConfirmResultsPdf;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;

[Route("organisation/{internalOrgId}/competitions/{competitionId:int}/confirm-results/pdf")]
[RestrictToLocalhostActionFilter]
public class CompetitionConfirmResultsPdfController : Controller
{
    private readonly ICompetitionsService competitionsService;

    public CompetitionConfirmResultsPdfController(
        ICompetitionsService competitionsService)
    {
        this.competitionsService = competitionsService ?? throw new ArgumentNullException(nameof(competitionsService));
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string internalOrgId,
        int competitionId)
    {
        var competition = await competitionsService.GetCompetitionForResults(internalOrgId, competitionId);

        if (competition == null) return NotFound();

        var model = new PdfConfirmResultsModel(competition);

        return View(model);
    }
}
