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
using NHSD.GPIT.BuyingCatalogue.WebApp.Extensions;
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

            var organisationIds = new List<string>(User.GetSecondaryOrganisationInternalIdentifiers())
            {
                User.GetPrimaryOrganisationInternalIdentifier(),
            };

            var organisations = await organisationsService.GetOrganisationsByInternalIdentifiers(organisationIds.ToArray());
            var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);

            (PagedList<Order> orders, _) = await orderService.GetPagedOrders(organisation.Id, options, search);

            var model = new OrganisationModel(organisation, organisations, orders.Items)
            {
                Options = orders.Options,
            };

            return View(model);
        }

        [HttpPost("organisation/{internalOrgId}")]
        public IActionResult Organisation(
            string internalOrgId,
            OrganisationModel model,
            [FromQuery] string page = "",
            [FromQuery] string search = "")
        {
            _ = internalOrgId;

            if (!ModelState.IsValid)
                return View(model);

            return RedirectToAction(
                nameof(Organisation),
                new { internalOrgId = model.SelectedOrganisationId, page, search });
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
                new HtmlEncodedSuggestionSearchResult(
                    r.Title,
                    r.Category,
                    currentPageUrl.AppendQueryParameterToUrl(nameof(search), r.Category).Uri.PathAndQuery)));
        }
    }
}
