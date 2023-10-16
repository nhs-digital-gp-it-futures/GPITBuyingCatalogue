using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers
{
    [Authorize("Buyer")]
    [Area("Orders")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}")]
    public sealed class OrderController : Controller
    {
        public const string ErrorKey = "Order";
        public const string ErrorMessage = "Your order is incomplete. Please go back to the order and check again";

        private readonly IOrderService orderService;
        private readonly IOrderProgressService orderProgressService;
        private readonly IOrganisationsService organisationsService;
        private readonly IImplementationPlanService implementationPlanService;
        private readonly IOrderPdfService pdfService;

        public OrderController(
            IOrderService orderService,
            IOrderProgressService orderProgressService,
            IOrganisationsService organisationsService,
            IImplementationPlanService implementationPlanService,
            IOrderPdfService pdfService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.orderProgressService = orderProgressService ?? throw new ArgumentNullException(nameof(orderProgressService));
            this.organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            this.implementationPlanService = implementationPlanService ?? throw new ArgumentNullException(nameof(implementationPlanService));
            this.pdfService = pdfService ?? throw new ArgumentNullException(nameof(pdfService));
        }

        [HttpGet]
        public async Task<IActionResult> Order(string internalOrgId, CallOffId callOffId)
        {
            var order = (await orderService.GetOrderForTaskListStatuses(callOffId, internalOrgId)).Order;

            if (order.OrderStatus is OrderStatus.Completed or OrderStatus.Terminated)
            {
                return RedirectToAction(
                    nameof(Summary),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            var orderProgress = await orderProgressService.GetOrderProgress(internalOrgId, callOffId);

            var orderModel = new OrderModel(internalOrgId, order, orderProgress)
            {
                DescriptionUrl = Url.Action(
                    nameof(OrderDescriptionController.OrderDescription),
                    typeof(OrderDescriptionController).ControllerName(),
                    new { internalOrgId, order.CallOffId }),
                BackLink = Url.Action(
                    nameof(DashboardController.Organisation),
                    typeof(DashboardController).ControllerName(),
                    new { internalOrgId }),
                BackLinkText = "Go back to dashboard",
            };

            return View(orderModel);
        }

        [HttpGet("~/order/organisation/{internalOrgId}/order/ready-to-start")]
        public async Task<IActionResult> ReadyToStart(string internalOrgId, OrderTypeEnum orderType, OrderTriageValue? option = null)
        {
            var orderTypeValue = new OrderType(orderType);
            string backLink;

            if (orderTypeValue.ToCatalogueItemType != CatalogueItemType.AssociatedService)
            {
                backLink = Url.Action(
                    nameof(OrderTriageController.TriageSelection),
                    typeof(OrderTriageController).ControllerName(),
                    new { internalOrgId, option, selected = true, orderType });
            }
            else
            {
                if (User.GetSecondaryOrganisationInternalIdentifiers().Any())
                {
                    backLink = Url.Action(
                    nameof(OrderTriageController.SelectOrganisation),
                    typeof(OrderTriageController).ControllerName(),
                    new { internalOrgId, orderType });
                }
                else
                {
                    backLink = Url.Action(
                    nameof(OrderTriageController.DetermineAssociatedServiceType),
                    typeof(OrderTriageController).ControllerName(),
                    new { internalOrgId, orderType });
                }
            }

            var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);

            var model = new ReadyToStartModel(organisation)
            {
                Option = option,
                BackLink = backLink,
            };

            return View(model);
        }

        [HttpPost("~/order/organisation/{internalOrgId}/order/ready-to-start")]
        public IActionResult ReadyToStart(string internalOrgId, ReadyToStartModel model, OrderTypeEnum orderType, OrderTriageValue? option = null)
        {
            return RedirectToAction(
                nameof(NewOrder),
                new { internalOrgId, option, orderType });
        }

        [HttpGet("~/order/organisation/{internalOrgId}/order/new-order")]
        public async Task<IActionResult> NewOrder(string internalOrgId, OrderTypeEnum orderType, OrderTriageValue? option = null)
        {
            var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);

            var orderModel = new OrderModel(internalOrgId, null, new OrderProgress(), organisation.Name)
            {
                DescriptionUrl = Url.Action(
                    nameof(OrderDescriptionController.NewOrderDescription),
                    typeof(OrderDescriptionController).ControllerName(),
                    new { internalOrgId, option, orderType }),
                BackLink = Url.Action(nameof(ReadyToStart), new { internalOrgId, option, orderType }),
            };

            return View("Order", orderModel);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> Summary(string internalOrgId, CallOffId callOffId)
        {
            var orderWrapper = await orderService.GetOrderForSummary(callOffId, internalOrgId);
            var order = orderWrapper.Order;
            var hasSubsequentRevisions = await orderService.HasSubsequentRevisions(callOffId);

            var defaultPlan = await implementationPlanService.GetDefaultImplementationPlan();

            var model = new SummaryModel(orderWrapper, internalOrgId, hasSubsequentRevisions, defaultPlan)
            {
                BackLink = GetBackLink(internalOrgId, callOffId, order),
                Title = GetTitle(orderWrapper),
                AdviceText = GetAdvice(orderWrapper, !hasSubsequentRevisions),
            };

            return View(model);
        }

        [HttpPost("summary")]
        public async Task<IActionResult> SummaryComplete(string internalOrgId, CallOffId callOffId)
        {
            var orderWrapper = await orderService.GetOrderForSummary(callOffId, internalOrgId);
            if (!orderWrapper.CanComplete())
            {
                ModelState.AddModelError(ErrorKey, ErrorMessage);
                var hasSubsequentRevisions = await orderService.HasSubsequentRevisions(callOffId);

                var defaultPlan = await implementationPlanService.GetDefaultImplementationPlan();

                var model = new SummaryModel(orderWrapper, internalOrgId, hasSubsequentRevisions, defaultPlan)
                {
                    BackLink = GetBackLink(internalOrgId, callOffId, orderWrapper.Order),
                    Title = GetTitle(orderWrapper),
                    AdviceText = GetAdvice(orderWrapper, !hasSubsequentRevisions),
                };

                return View(model);
            }

            await orderService.CompleteOrder(
                callOffId,
                internalOrgId,
                User.UserId());

            return RedirectToAction(
                nameof(OrderController.Completed),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("completed")]
        public async Task<IActionResult> Completed(string internalOrgId, CallOffId callOffId)
        {
            var order = (await orderService.GetOrderForSummary(callOffId, internalOrgId)).Order;

            return View(new CompletedModel(internalOrgId, order)
            {
                BackLink = Url.Action(
                    nameof(DashboardController.Organisation),
                    typeof(DashboardController).ControllerName(),
                    new { internalOrgId }),
                BackLinkText = "Go back to orders dashboard",
            });
        }

        [HttpGet("download")]
        public async Task<IActionResult> Download(string internalOrgId, CallOffId callOffId)
        {
            var order = (await orderService.GetOrderForSummary(callOffId, internalOrgId)).Order;

            var result = await pdfService.CreateOrderSummaryPdf(order);

            var fileName = order.OrderStatus switch
            {
                OrderStatus.Terminated => $"order-summary-terminated-{callOffId}.pdf",
                OrderStatus.Completed => $"order-summary-completed-{callOffId}.pdf",
                _ => $"order-summary-in-progress-{callOffId}.pdf",
            };

            return File(result.ToArray(), "application/pdf", fileName);
        }

        [HttpGet("amend")]
        public IActionResult AmendOrder(string internalOrgId, CallOffId callOffId)
        {
            return View(new AmendOrderModel(internalOrgId, callOffId)
            {
                BackLink = Url.Action(
                    nameof(Summary),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
            });
        }

        [HttpPost("amend")]
        public async Task<IActionResult> AmendOrder(string internalOrgId, CallOffId callOffId, AmendOrderModel model)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;
            var hasSubsequentRevisions = await orderService.HasSubsequentRevisions(callOffId);
            if (hasSubsequentRevisions || order.ContractExpired)
            {
                return RedirectToAction(
                    nameof(DashboardController.Organisation),
                    typeof(DashboardController).ControllerName(),
                    new { internalOrgId });
            }

            var amendment = await orderService.AmendOrder(internalOrgId, callOffId);

            return RedirectToAction(
                nameof(Order),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, amendment.CallOffId });
        }

        [HttpGet("terminate")]
        public IActionResult TerminateOrder(string internalOrgId, CallOffId callOffId)
        {
            return View(new TerminateOrderModel(internalOrgId, callOffId)
            {
                BackLink = Url.Action(
                    nameof(Summary),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
            });
        }

        [HttpPost("terminate")]
        public async Task<IActionResult> TerminateOrder(string internalOrgId, CallOffId callOffId, TerminateOrderModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var hasSubsequentRevisions = await orderService.HasSubsequentRevisions(callOffId);
            if (hasSubsequentRevisions)
            {
                return RedirectToAction(
                    nameof(DashboardController.Organisation),
                    typeof(DashboardController).ControllerName(),
                    new { internalOrgId });
            }

            await orderService.TerminateOrder(callOffId, internalOrgId, User.UserId(), model.TerminationDate.GetValueOrDefault(), model.Reason);

            return RedirectToAction(
                nameof(Summary),
                typeof(OrderController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        internal static string GetAdvice(OrderWrapper orderWrapper, bool latestOrder)
        {
            var order = orderWrapper.Order;

            return order.OrderStatus switch
            {
                OrderStatus.Terminated => "This contract has been terminated, but you can still view the details.",
                OrderStatus.Completed when order.ContractExpired => $"This order expired on {order.EndDate.DisplayValue}, but you can still view the details.",
                OrderStatus.Completed when order.AssociatedServicesOnly => "This order has already been completed, but you can terminate the contract if needed.",
                OrderStatus.Completed when latestOrder => "This order has already been completed, but you can amend or terminate the contract if needed.",
                OrderStatus.Completed => "There is an amendment currently in progress for this contract.",
                _ => orderWrapper.CanComplete()
                    ? "Review the items you’ve added to your order before completing it."
                    : "This is what's been added to your order so far. You must complete all mandatory steps before you can confirm your order.",
            };
        }

        private static string GetTitle(OrderWrapper orderWrapper)
        {
            var order = orderWrapper.Order;

            return order.OrderStatus switch
            {
                OrderStatus.Terminated => "Terminated contract details",
                OrderStatus.Completed => "Order confirmed",
                _ => orderWrapper.CanComplete()
                    ? "Review order summary"
                    : "Order summary",
            };
        }

        private string GetBackLink(string internalOrgId, CallOffId callOffId, Order order)
        {
            return order.OrderStatus is OrderStatus.Completed or OrderStatus.Terminated
                                ? Url.Action(
                                    nameof(DashboardController.Organisation),
                                    typeof(DashboardController).ControllerName(),
                                    new { internalOrgId })
                                : Url.Action(
                                    nameof(Order),
                                    typeof(OrderController).ControllerName(),
                                    new { internalOrgId, callOffId });
        }
    }
}
