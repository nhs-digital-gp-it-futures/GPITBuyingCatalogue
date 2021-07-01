using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contacts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.TaskList;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{odsCode}/order/{callOffId}")]
    public sealed class OrderController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IOrderDescriptionService orderDescriptionService;
        private readonly IOrderingPartyService orderingPartyService;
        private readonly IOrganisationsService organisationService;
        private readonly IContactDetailsService contactDetailsService;
        private readonly ICommencementDateService commencementDateService;
        private readonly IFundingSourceService fundingSourceService;
        private readonly ITaskListService taskListService;

        // TODO: too many dependencies, i.e. too many responsibilities
        public OrderController(
            IOrderService orderService,
            IOrderDescriptionService orderDescriptionService,
            IOrderingPartyService orderingPartyService,
            IOrganisationsService organisationService,
            IContactDetailsService contactDetailsService,
            ICommencementDateService commencementDateService,
            IFundingSourceService fundingSourceService,
            ITaskListService taskListService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.orderDescriptionService = orderDescriptionService ?? throw new ArgumentNullException(nameof(orderDescriptionService));
            this.orderingPartyService = orderingPartyService ?? throw new ArgumentNullException(nameof(orderingPartyService));
            this.organisationService = organisationService ?? throw new ArgumentNullException(nameof(organisationService));
            this.contactDetailsService = contactDetailsService ?? throw new ArgumentNullException(nameof(contactDetailsService));
            this.commencementDateService = commencementDateService ?? throw new ArgumentNullException(nameof(commencementDateService));
            this.fundingSourceService = fundingSourceService ?? throw new ArgumentNullException(nameof(fundingSourceService));
            this.taskListService = taskListService ?? throw new ArgumentNullException(nameof(taskListService));
        }

        [HttpGet]
        public async Task<IActionResult> Order(string odsCode, CallOffId callOffId)
        {
            var order = await orderService.GetOrder(callOffId);

            var sectionStatuses = taskListService.GetTaskListStatusModelForOrder(order);

            var orderModel = new OrderModel(odsCode, order, sectionStatuses)
            {
                DescriptionUrl = Url.Action(
                                    nameof(OrderController.OrderDescription),
                                    typeof(OrderController).ControllerName(),
                                    new { odsCode, order.CallOffId }),
            };

            return View(orderModel);
        }

        [HttpGet("complete-order")]
        public async Task<IActionResult> CompleteOrder(string odsCode, CallOffId callOffId)
        {
            var order = await orderService.GetOrder(callOffId);

            var model = new CompleteOrderModel(odsCode, callOffId, order)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { odsCode, callOffId }),
            };

            return View(model);
        }

        [HttpPost("complete-order")]
        public async Task<IActionResult> CompleteOrder(string odsCode, CallOffId callOffId, CompleteOrderModel model)
        {
            var order = await orderService.GetOrder(callOffId);

            if (!order.CanComplete())
            {
                ModelState.AddModelError("Order", "Your order is incomplete. Please go back to the order and check again");
                return View(model);
            }

            await orderService.CompleteOrder(callOffId);

            return RedirectToAction(
                nameof(OrderController.CompletedOrderConfirmation),
                typeof(OrderController).ControllerName(),
                new { odsCode, callOffId });
        }

        [HttpGet("complete-order/order-confirmation")]
        public IActionResult CompletedOrderConfirmation(string odsCode, CallOffId callOffId)
        {
            var model = new CompletedOrderConfirmationModel(odsCode, callOffId)
            {
                BackLink = Url.Action(
                    nameof(DashboardController.Organisation),
                    typeof(DashboardController).ControllerName(),
                    new { odsCode }),
            };

            return View(model);
        }

        [HttpGet("~/order/organisation/{odsCode}/order/neworder")]
        public IActionResult NewOrder(string odsCode)
        {
            var orderModel = new OrderModel(odsCode, null, new OrderTaskList())
            {
                DescriptionUrl = Url.Action(
                                    nameof(OrderController.NewOrderDescription),
                                    typeof(OrderController).ControllerName(),
                                    new { odsCode }),
            };

            return View("Order", orderModel);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> Summary(string odsCode, CallOffId callOffId, string print = "false")
        {
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
        public async Task<IActionResult> DeleteOrder(string odsCode, CallOffId callOffId)
        {
            var order = await orderService.GetOrder(callOffId);

            return View(new DeleteOrderModel(odsCode, order));
        }

        [HttpPost("delete-order")]
        public async Task<IActionResult> DeleteOrder(string odsCode, CallOffId callOffId, DeleteOrderModel model)
        {
            await orderService.DeleteOrder(callOffId);

            return RedirectToAction(
                nameof(DashboardController.Organisation),
                typeof(DashboardController).ControllerName(),
                new { odsCode });
        }

        [HttpGet("delete-order/confirmation")]
        public async Task<IActionResult> DeleteOrderConfirmation(string odsCode, CallOffId callOffId)
        {
            var order = await orderService.GetOrder(callOffId);

            return View(new DeleteConfirmationModel(odsCode, order));
        }

        [HttpGet("description")]
        public async Task<IActionResult> OrderDescription(string odsCode, CallOffId callOffId)
        {
            var order = await orderService.GetOrder(callOffId);

            var descriptionModel = new OrderDescriptionModel(odsCode, order)
            {
                BackLink = Url.Action(
                            nameof(OrderController.Order),
                            typeof(OrderController).ControllerName(),
                            new { odsCode, callOffId }),
            };

            return View(descriptionModel);
        }

        [HttpPost("description")]
        public async Task<IActionResult> OrderDescription(string odsCode, CallOffId callOffId, OrderDescriptionModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await orderDescriptionService.SetOrderDescription(callOffId, model.Description);

            return RedirectToAction(
                nameof(Order),
                typeof(OrderController).ControllerName(),
                new { odsCode, callOffId });
        }

        [HttpGet("~/organisation/{odsCode}/order/neworder/description")]
        public IActionResult NewOrderDescription(string odsCode)
        {
            var descriptionModel = new OrderDescriptionModel(odsCode, null)
            {
                BackLink = Url.Action(
                            nameof(OrderController.NewOrder),
                            typeof(OrderController).ControllerName(),
                            new { odsCode }),
            };

            return View("OrderDescription", descriptionModel);
        }

        [HttpPost("~/organisation/{odsCode}/order/neworder/description")]
        public async Task<IActionResult> NewOrderDescription(string odsCode, OrderDescriptionModel model)
        {
            if (!ModelState.IsValid)
                return View("OrderDescription", model);

            var order = await orderService.CreateOrder(model.Description, model.OdsCode);

            return RedirectToAction(
                nameof(OrderController.Order),
                typeof(OrderController).ControllerName(),
                new { odsCode, order.CallOffId });
        }

        [HttpGet("ordering-party")]
        public async Task<IActionResult> OrderingParty(string odsCode, CallOffId callOffId)
        {
            var order = await orderService.GetOrder(callOffId);
            var organisation = await organisationService.GetOrganisationByOdsCode(odsCode);

            return View(new OrderingPartyModel(odsCode, order, organisation));
        }

        [HttpPost("ordering-party")]
        public async Task<IActionResult> OrderingParty(string odsCode, CallOffId callOffId, OrderingPartyModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var order = await orderService.GetOrder(callOffId);

            var contact = contactDetailsService.AddOrUpdatePrimaryContact(
                order.OrderingPartyContact,
                model.Contact);

            // TODO: Update naming (only sets contact now)
            await orderingPartyService.SetOrderingParty(order, null, contact);

            return RedirectToAction(
                nameof(Order),
                typeof(OrderController).ControllerName(),
                new { odsCode, callOffId });
        }

        [HttpGet("commencement-date")]
        public async Task<IActionResult> CommencementDate(string odsCode, CallOffId callOffId)
        {
            var order = await orderService.GetOrder(callOffId);

            return View(new CommencementDateModel(odsCode, callOffId, order.CommencementDate));
        }

        [HttpPost("commencement-date")]
        public async Task<IActionResult> CommencementDate(string odsCode, CallOffId callOffId, CommencementDateModel model)
        {
            (DateTime? date, var error) = model.ToDateTime();

            if (error != null)
                ModelState.AddModelError("Day", error);

            if (!ModelState.IsValid)
                return View(model);

            await commencementDateService.SetCommencementDate(callOffId, date);

            return RedirectToAction(
                nameof(Order),
                typeof(OrderController).ControllerName(),
                new { odsCode, callOffId });
        }

        [HttpGet("funding-source")]
        public async Task<IActionResult> FundingSource(string odsCode, CallOffId callOffId)
        {
            var order = await orderService.GetOrder(callOffId);

            return View(new FundingSourceModel(odsCode, callOffId, order.FundingSourceOnlyGms));
        }

        [HttpPost("funding-source")]
        public async Task<IActionResult> FundingSource(string odsCode, CallOffId callOffId, FundingSourceModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var onlyGms = model.FundingSourceOnlyGms.EqualsIgnoreCase("Yes");

            await fundingSourceService.SetFundingSource(callOffId, onlyGms);

            return RedirectToAction(
                nameof(Order),
                typeof(OrderController).ControllerName(),
                new { odsCode, callOffId });
        }
    }
}
