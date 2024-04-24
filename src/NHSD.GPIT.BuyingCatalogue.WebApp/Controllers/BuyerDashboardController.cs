using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;

[Authorize("Buyer")]
[Route("buyer-dashboard")]
public class BuyerDashboardController : Controller
{
    private readonly IOrganisationsService organisationsService;
    private readonly IOrderService orderService;
    private readonly IManageFiltersService manageFiltersService;
    private readonly ICompetitionsService competitionsService;

    public BuyerDashboardController(
        IOrganisationsService organisationsService,
        IOrderService orderService,
        IManageFiltersService manageFiltersService,
        ICompetitionsService competitionsService)
    {
        this.organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
        this.manageFiltersService = manageFiltersService ?? throw new ArgumentNullException(nameof(manageFiltersService));
        this.competitionsService = competitionsService ?? throw new ArgumentNullException(nameof(competitionsService));
        this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
    }

    [HttpGet("{internalOrgId}")]
    public async Task<IActionResult> Index(
        string internalOrgId)
    {
        var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);

        var filters = await manageFiltersService.GetFilters(organisation.Id);
        var competitions = await competitionsService.GetCompetitionsDashboard(internalOrgId);
        var orders = await orderService.GetOrders(organisation.Id);

        var model = new BuyerDashboardModel(organisation, orders, competitions.ToList(), filters);

        return View(model);
    }
}
