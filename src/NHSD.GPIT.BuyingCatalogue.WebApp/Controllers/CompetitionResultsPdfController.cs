using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.CompetitionResultsPdf;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;

[Route("organisation/{internalOrgId}/competitions/{competitionId:int}/results/pdf")]
[RestrictToLocalhostActionFilter]
public class CompetitionResultsPdfController : Controller
{
    private readonly ICompetitionsService competitionsService;
    private readonly IManageFiltersService filtersService;

    public CompetitionResultsPdfController(
        ICompetitionsService competitionsService,
        IManageFiltersService filtersService)
    {
        this.competitionsService = competitionsService ?? throw new ArgumentNullException(nameof(competitionsService));
        this.filtersService = filtersService ?? throw new ArgumentNullException(nameof(filtersService));
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string internalOrgId,
        int competitionId)
    {
        var competition = await competitionsService.GetCompetitionForResults(internalOrgId, competitionId);
        var nonShortlistedSolutions = await competitionsService.GetNonShortlistedSolutions(internalOrgId, competitionId);
        var filterDetails = await filtersService.GetFilterDetails(competition.OrganisationId, competition.FilterId);

        if (competition == null) return NotFound();

        var model = new PdfViewResultsModel(competition, filterDetails, nonShortlistedSolutions);

        return View(model);
    }
}
