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
                actionName: nameof(Organisation),
                controllerName: typeof(DashboardController).ControllerName(),
                routeValues: new { odsCode });
        }

        [HttpGet("organisation/{odsCode}")]
        public async Task<IActionResult> Organisation(string odsCode)
        {
            var organisation = await organisationsService.GetOrganisationByOdsCode(odsCode);

            var allOrders = await orderService.GetOrders(organisation.OrganisationId);

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
                actionName: nameof(Organisation),
                controllerName: typeof(DashboardController).ControllerName(),
                routeValues: new { odsCode = model.SelectedOrganisation });
        }

        [HttpGet("organisation/{odsCode}/order/neworder")]
        public IActionResult NewOrder(string odsCode)
        {
            return View(new NewOrderModel(odsCode));
        }

        [HttpGet("organisation/{odsCode}/order/neworder/description")]
        public IActionResult NewOrderDescription(string odsCode)
        {
            return View(new NewOrderDescriptionModel(odsCode));
        }

        [HttpPost("organisation/{odsCode}/order/neworder/description")]
        public async Task<IActionResult> NewOrderDescription(string odsCode, NewOrderDescriptionModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var order = await orderService.CreateOrder(model.Description, model.OdsCode);

            return RedirectToAction(
                actionName: nameof(OrderController.Order),
                controllerName: typeof(OrderController).ControllerName(),
                routeValues: new { odsCode, order.CallOffId });
        }
    }
}
