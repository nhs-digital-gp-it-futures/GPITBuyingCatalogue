using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Dashboard;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Shared;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Autocomplete;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
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
            if (!User.IsBuyer())
                return View("NotBuyer");

            var odsCode = User.GetPrimaryOdsCode();

            return RedirectToAction(
                nameof(Organisation),
                typeof(DashboardController).ControllerName(),
                new { odsCode });
        }

        [HttpGet("organisation/{odsCode}")]
        public async Task<IActionResult> Organisation(
            string odsCode,
            [FromQuery] string page = "",
            [FromQuery] string search = "")
        {
            const int PageSize = 10;
            var options = new PageOptions(page, PageSize);

            var organisation = await organisationsService.GetOrganisationByOdsCode(odsCode);

            var orders = await orderService.GetPagedOrders(organisation.Id, options, search);

            var model = new OrganisationModel(organisation, User, orders.Items)
            {
                Options = orders.Options,
            };

            return View(model);
        }

        [HttpGet("organisation/{odsCode}/select")]
        public async Task<IActionResult> SelectOrganisation(string odsCode)
        {
            var odsCodes = new List<string>(User.GetSecondaryOdsCodes())
            {
                User.GetPrimaryOdsCode(),
            };

            var organisations = await organisationsService.GetOrganisationsByOdsCodes(odsCodes.ToArray());

            var model = new SelectOrganisationModel(odsCode, organisations)
            {
                BackLink = Url.Action(nameof(Organisation), new { odsCode }),
            };

            return View(model);
        }

        [HttpPost("organisation/{odsCode}/select")]
        public IActionResult SelectOrganisation(string odsCode, SelectOrganisationModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            return RedirectToAction(
                nameof(Organisation),
                typeof(DashboardController).ControllerName(),
                new { odsCode = model.SelectedOrganisation });
        }

        [HttpGet("organisation/{odsCode}/search-suggestions")]
        public async Task<IActionResult> FilterSearchSuggestions(
            string odsCode,
            [FromQuery] string search)
        {
            var currentPageUrl = new UriBuilder(HttpContext.Request.Headers.Referer.ToString());

            var organisation = await organisationsService.GetOrganisationByOdsCode(odsCode);
            var results = await orderService.GetOrdersBySearchTerm(organisation.Id, search);

            return Json(results.Select(r =>
                new AutocompleteResult
                {
                    Title = r.Title,
                    Category = r.Category,
                    Url = currentPageUrl.AppendQueryParameterToUrl(nameof(search), r.Title).ToString(),
                }));
        }
    }
}
