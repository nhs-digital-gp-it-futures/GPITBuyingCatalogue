using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;

[Authorize("Buyer")]
[Route("buyer-dashboard")]
public class BuyerDashboardController : Controller
{
    private readonly IOrganisationsService organisationsService;

    public BuyerDashboardController(
        IOrganisationsService organisationsService)
    {
        this.organisationsService =
            organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var organisationId = User.GetPrimaryOrganisationInternalIdentifier();

        var organisation = await organisationsService.GetOrganisationByInternalIdentifier(organisationId);

        var model = new BuyerDashboardModel(
            organisation.Name,
            User.IsAccountManager());

        return View(model);
    }
}
