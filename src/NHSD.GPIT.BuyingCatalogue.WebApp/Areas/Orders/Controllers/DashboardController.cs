using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Dashboard;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Shared;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.SuggestionSearch;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers
{
    [Authorize("Buyer")]
    [Area("Orders")]
    [Route("order")]
    public sealed class DashboardController : Controller
    {
        private readonly IOrganisationsService organisationsService;
        private readonly IOrderService orderService;

        public DashboardController(
            IOrganisationsService organisationsService,
            IOrderService orderService)
        {
            this.organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        public IActionResult Index()
        {
            var internalOrgId = User.GetPrimaryOrganisationInternalIdentifier();

            return RedirectToAction(
                nameof(Organisation),
                typeof(DashboardController).ControllerName(),
                new { internalOrgId });
        }

        [HttpGet("organisation/{internalOrgId}")]
        public async Task<IActionResult> Organisation(
            string internalOrgId,
            [FromQuery] string page = "",
            [FromQuery] string search = "")
        {
            var options = new PageOptions(page, pageSize: 10);
            var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);
            var ordersCount = (await orderService.GetOrdersBySearchTerm(organisation.Id, search)).Count;
            (PagedList<Order> orders, IEnumerable<CallOffId> orderIds) = await orderService.GetPagedOrders(organisation.Id, options, search);

            var model = new OrganisationModel(organisation, User, orders.Items)
            {
                Options = orders.Options,
                OrderIds = orderIds,
                OrdersCount = ordersCount,
            };

            return View(model);
        }

        [HttpGet("organisation/{internalOrgId}/select")]
        public async Task<IActionResult> SelectOrganisation(string internalOrgId)
        {
            var internalOrgIds = new List<string>(User.GetSecondaryOrganisationInternalIdentifiers())
            {
                User.GetPrimaryOrganisationInternalIdentifier(),
            };

            var organisations = await organisationsService.GetOrganisationsByInternalIdentifiers(internalOrgIds.ToArray());

            var model = new SelectOrganisationModel(internalOrgId, organisations)
            {
                BackLink = Url.Action(nameof(Organisation), new { internalOrgId }),
            };

            return View(model);
        }

        [HttpPost("organisation/{internalOrgId}/select")]
        public IActionResult SelectOrganisation(string internalOrgId, SelectOrganisationModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            return RedirectToAction(
                nameof(Organisation),
                typeof(DashboardController).ControllerName(),
                new { internalOrgId = model.SelectedOrganisation });
        }

        [HttpGet("organisation/{internalOrgId}/search-suggestions")]
        public async Task<IActionResult> FilterSearchSuggestions(
            string internalOrgId,
            [FromQuery] string search)
        {
            var currentPageUrl = new UriBuilder(HttpContext.Request.Headers.Referer.ToString());

            var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);
            var results = await orderService.GetOrdersBySearchTerm(organisation.Id, search);

            return Json(results.Select(r =>
                new SuggestionSearchResult
                {
                    Title = r.Title,
                    Category = r.Category,
                    Url = currentPageUrl.AppendQueryParameterToUrl(nameof(search), r.Category).ToString(),
                }));
        }
    }
}
