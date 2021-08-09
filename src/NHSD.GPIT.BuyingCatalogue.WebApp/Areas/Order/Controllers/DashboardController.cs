using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Dashboard;

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
        public async Task<IActionResult> Organisation(string odsCode)
        {
            var organisation = await organisationsService.GetOrganisationByOdsCode(odsCode);

            var allOrders = await orderService.GetOrders(organisation.Id);

            return View(new OrganisationModel(organisation, User, allOrders));
        }

        [HttpGet("organisation/{odsCode}/select")]
        public async Task<IActionResult> SelectOrganisation(string odsCode)
        {
            var odsCodes = new List<string>(User.GetSecondaryOdsCodes())
            {
                User.GetPrimaryOdsCode(),
            };

            var organisations = await organisationsService.GetOrganisationsByOdsCodes(odsCodes.ToArray());

            return View(new SelectOrganisationModel(odsCode, organisations));
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
    }
}
