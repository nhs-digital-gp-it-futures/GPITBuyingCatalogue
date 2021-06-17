using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Dashboard;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order")]
    public class DashboardController : Controller
    {
        private readonly ILogWrapper<OrderController> logger;
        private readonly IOrganisationsService organisationsService;
        private readonly IOrderService orderService;

        public DashboardController(
            ILogWrapper<OrderController> logger,
            IOrganisationsService organisationsService,
            IOrderService orderService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));

            logger.LogInformation($"Taking user to {nameof(DashboardController)}.{nameof(Organisation)} for {nameof(odsCode)} {odsCode}");

            var organisation = await organisationsService.GetOrganisationByOdsCode(odsCode);

            var allOrders = await orderService.GetOrders(organisation.OrganisationId);

            return View(new OrganisationModel(organisation, User, allOrders));
        }

        [HttpGet("organisation/{odsCode}/select")]
        public async Task<IActionResult> SelectOrganisation(string odsCode)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));

            logger.LogInformation($"Taking user to {nameof(DashboardController)}.{nameof(SelectOrganisation)} for {nameof(odsCode)} {odsCode}");

            var odsCodes = new List<string>(User.GetSecondaryOdsCodes())
            {
                User.GetPrimaryOdsCode(),
            };

            var organisations = await organisationsService.GetOrganisationsByOdsCodes(odsCodes.ToArray());

            return View(new SelectOrganisationModel(odsCode, organisations));
        }

        [ValidateAntiForgeryToken]
        [HttpPost("organisation/{odsCode}/select")]
        public IActionResult SelectOrganisation(string odsCode, SelectOrganisationModel model)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            model.ValidateNotNull(nameof(model));

            logger.LogInformation($"Handling post for {nameof(DashboardController)}.{nameof(SelectOrganisation)} for {nameof(odsCode)} {odsCode}");

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
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));

            logger.LogInformation($"Taking user to {nameof(DashboardController)}.{nameof(NewOrder)} for {nameof(odsCode)} {odsCode}");

            return View(new NewOrderModel(odsCode));
        }

        [HttpGet("organisation/{odsCode}/order/neworder/description")]
        public IActionResult NewOrderDescription(string odsCode)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));

            logger.LogInformation($"Taking user to {nameof(DashboardController)}.{nameof(NewOrderDescription)} for {nameof(odsCode)} {odsCode}");

            return View(new NewOrderDescriptionModel(odsCode));
        }

        [ValidateAntiForgeryToken]
        [HttpPost("organisation/{odsCode}/order/neworder/description")]
        public async Task<IActionResult> NewOrderDescription(string odsCode, NewOrderDescriptionModel model)
        {
            odsCode.ValidateNotNullOrWhiteSpace(nameof(odsCode));
            model.ValidateNotNull(nameof(model));

            logger.LogInformation($"Handling post for {nameof(DashboardController)}.{nameof(NewOrderDescription)} for {nameof(odsCode)} {odsCode}");

            if (!ModelState.IsValid)
                return View(model);

            var order = await orderService.CreateOrder(model.Description, model.OdsCode);

            return RedirectToAction(
                actionName: nameof(OrderController.Order),
                controllerName: typeof(OrderController).ControllerName(),
                routeValues: new { odsCode = odsCode, callOffId = order.CallOffId });
        }
    }
}
