using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.DashboardModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;

[Authorize("Development")]
[Authorize("Buyer")]
[Area("Competitions")]
[Route("organisation/{internalOrgId}/competitions")]
public class CompetitionsDashboardController : Controller
{
    private readonly IOrganisationsService organisationsService;
    private readonly ICompetitionsService competitionsService;

    public CompetitionsDashboardController(
        IOrganisationsService organisationsService,
        ICompetitionsService competitionsService)
    {
        this.organisationsService =
            organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
        this.competitionsService = competitionsService ?? throw new ArgumentNullException(nameof(competitionsService));
    }

    [HttpGet]
    public async Task<IActionResult> Index(string internalOrgId)
    {
        var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);

        var competitions = await competitionsService.GetCompetitions(organisation.Id);

        var model = new CompetitionDashboardModel(internalOrgId, organisation.Name, competitions.ToList());

        return View(model);
    }

    [HttpGet("before-you-start")]
    public IActionResult BeforeYouStart(string internalOrgId)
    {
        var model = new NavBaseModel { BackLink = Url.Action(nameof(Index), new { internalOrgId }) };

        return View(model);
    }

    [HttpPost("before-you-start")]
    public IActionResult BeforeYouStart(string internalOrgId, NavBaseModel model)
    {
        _ = model;

        return RedirectToAction(nameof(Index), new { internalOrgId });
    }
}
