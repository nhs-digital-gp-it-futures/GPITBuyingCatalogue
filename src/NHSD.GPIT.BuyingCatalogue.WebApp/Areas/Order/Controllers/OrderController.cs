using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contacts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{odsCode}/order/{callOffId}")]
    public class OrderController : Controller
    {
        private readonly ILogWrapper<OrderController> logger;
        private readonly IOrderService orderService;
        private readonly IOrderDescriptionService orderDescriptionService;
        private readonly IOrderingPartyService orderingPartyService;
        private readonly IOrganisationsService organisationService;
        private readonly IContactDetailsService contactDetailsService;
        private readonly ICommencementDateService commencementDateService;
        private readonly IFundingSourceService fundingSourceService;

        public OrderController(
            ILogWrapper<OrderController> logger,
            IOrderService orderService,
            IOrderDescriptionService orderDescriptionService,
            IOrderingPartyService orderingPartyService,
            IOrganisationsService organisationService,
            IContactDetailsService contactDetailsService,
            ICommencementDateService commencementDateService,
            IFundingSourceService fundingSourceService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.orderDescriptionService = orderDescriptionService ?? throw new ArgumentNullException(nameof(orderDescriptionService));
            this.orderingPartyService = orderingPartyService ?? throw new ArgumentNullException(nameof(orderingPartyService));
            this.organisationService = organisationService ?? throw new ArgumentNullException(nameof(organisationService));
            this.contactDetailsService = contactDetailsService ?? throw new ArgumentNullException(nameof(contactDetailsService));
            this.commencementDateService = commencementDateService ?? throw new ArgumentNullException(nameof(commencementDateService));
            this.fundingSourceService = fundingSourceService ?? throw new ArgumentNullException(nameof(fundingSourceService));
        }

        [HttpGet]
        public async Task<IActionResult> Order(string odsCode, string callOffId)
        {
            logger.LogInformation($"Taking user to {nameof(OrderController)}.{nameof(Order)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var order = await orderService.GetOrder(callOffId);

            return View(new OrderModel(odsCode, order));
        }

        [HttpGet("summary")]
        public async Task<IActionResult> Summary(string odsCode, string callOffId, string print = "false")
        {
            logger.LogInformation($"Taking user to {nameof(OrderController)}.{nameof(Summary)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}, {nameof(print)} {print}");

            var order = await orderService.GetOrder(callOffId);

            if (print.Equals("true", StringComparison.InvariantCultureIgnoreCase))
            {
                // TODO - The whole summary page needs dynamic content adding, probably best waiting until Additional/Associated services are added
                return View("PrintSummary");
            }

            // TODO - The whole summary page needs dynamic content adding, probably best waiting until Additional/Associated services are added
            return View(new SummaryModel(odsCode, order));
        }

        [HttpGet("delete-order")]
        public async Task<IActionResult> DeleteOrder(string odsCode, string callOffId)
        {
            logger.LogInformation($"Taking user to {nameof(OrderController)}.{nameof(DeleteOrder)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var order = await orderService.GetOrder(callOffId);

            return View(new DeleteOrderModel(odsCode, order));
        }

        [HttpPost("delete-order")]
        public async Task<IActionResult> DeleteOrder(string odsCode, string callOffId, DeleteOrderModel model)
        {
            logger.LogInformation($"Handling post for {nameof(OrderController)}.{nameof(DeleteOrder)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            await orderService.DeleteOrder(callOffId);

            return RedirectToAction(
                actionName: nameof(DashboardController.Organisation),
                controllerName: typeof(DashboardController).ControllerName(),
                routeValues: new { odsCode });
        }

        [HttpGet("delete-order/confirmation")]
        public async Task<IActionResult> DeleteOrderConfirmation(string odsCode, string callOffId)
        {
            logger.LogInformation($"Taking user to {nameof(OrderController)}.{nameof(DeleteOrderConfirmation)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var order = await orderService.GetOrder(callOffId);

            return View(new DeleteConfirmationModel(odsCode, order));
        }

        [HttpGet("description")]
        public async Task<IActionResult> OrderDescription(string odsCode, string callOffId)
        {
            logger.LogInformation($"Taking user to {nameof(OrderController)}.{nameof(OrderDescription)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var order = await orderService.GetOrder(callOffId);

            return View(new OrderDescriptionModel(odsCode, order));
        }

        [HttpPost("description")]
        public async Task<IActionResult> OrderDescription(string odsCode, string callOffId, OrderDescriptionModel model)
        {
            logger.LogInformation($"Handling post for {nameof(OrderController)}.{nameof(OrderDescription)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            if (!ModelState.IsValid)
                return View(model);

            await orderDescriptionService.SetOrderDescription(callOffId, model.Description);

            return RedirectToAction(
                actionName: nameof(Order),
                controllerName: typeof(OrderController).ControllerName(),
                routeValues: new { odsCode, callOffId });
        }

        [HttpGet("ordering-party")]
        public async Task<IActionResult> OrderingParty(string odsCode, string callOffId)
        {
            logger.LogInformation($"Taking user to {nameof(OrderController)}.{nameof(OrderingParty)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var order = await orderService.GetOrder(callOffId);
            var organisation = await organisationService.GetOrganisationByOdsCode(odsCode);

            return View(new OrderingPartyModel(odsCode, order, organisation));
        }

        [HttpPost("ordering-party")]
        public async Task<IActionResult> OrderingParty(string odsCode, string callOffId, OrderingPartyModel model)
        {
            logger.LogInformation($"Handling post for {nameof(OrderController)}.{nameof(OrderingParty)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            if (!ModelState.IsValid)
                return View(model);

            var order = await orderService.GetOrder(callOffId);

            var orderingParty = new OrderingParty
            {
                Name = model.OrganisationName,
                OdsCode = model.OdsCode,
                Address = contactDetailsService.AddOrUpdateAddress(order.OrderingParty.Address, model.Address),
            };

            Contact contact = contactDetailsService.AddOrUpdatePrimaryContact(
                order.OrderingPartyContact,
                model.Contact);

            await orderingPartyService.SetOrderingParty(order, orderingParty, contact);

            return RedirectToAction(
                actionName: nameof(Order),
                controllerName: typeof(OrderController).ControllerName(),
                routeValues: new { odsCode, callOffId });
        }

        [HttpGet("commencement-date")]
        public async Task<IActionResult> CommencementDate(string odsCode, string callOffId)
        {
            logger.LogInformation($"Taking user to {nameof(OrderController)}.{nameof(CommencementDate)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var order = await orderService.GetOrder(callOffId);

            return View(new CommencementDateModel(odsCode, callOffId, order.CommencementDate));
        }

        [HttpPost("commencement-date")]
        public async Task<IActionResult> CommencementDate(string odsCode, string callOffId, CommencementDateModel model)
        {
            logger.LogInformation($"Handling post for {nameof(OrderController)}.{nameof(CommencementDate)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            (var date, var error) = model.ToDateTime();

            if (error != null)
                ModelState.AddModelError("Day", error);

            if (!ModelState.IsValid)
                return View(model);

            await commencementDateService.SetCommencementDate(callOffId, date);

            return RedirectToAction(
                actionName: nameof(Order),
                controllerName: typeof(OrderController).ControllerName(),
                routeValues: new { odsCode, callOffId });
        }

        [HttpGet("funding-source")]
        public async Task<IActionResult> FundingSource(string odsCode, string callOffId)
        {
            logger.LogInformation($"Taking user to {nameof(OrderController)}.{nameof(FundingSource)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            var order = await orderService.GetOrder(callOffId);

            return View(new FundingSourceModel(odsCode, callOffId, order.FundingSourceOnlyGms));
        }

        [HttpPost("funding-source")]
        public async Task<IActionResult> FundingSource(string odsCode, string callOffId, FundingSourceModel model)
        {
            logger.LogInformation($"Handling post for {nameof(OrderController)}.{nameof(FundingSource)} for {nameof(odsCode)} {odsCode}, {nameof(callOffId)} {callOffId}");

            if (!ModelState.IsValid)
                return View(model);

            var onlyGms = model.FundingSourceOnlyGms.EqualsIgnoreCase("Yes") ? true : false;

            await fundingSourceService.SetFundingSource(callOffId, onlyGms);

            return RedirectToAction(
                actionName: nameof(Order),
                controllerName: typeof(OrderController).ControllerName(),
                routeValues: new { odsCode, callOffId });
        }
    }
}
