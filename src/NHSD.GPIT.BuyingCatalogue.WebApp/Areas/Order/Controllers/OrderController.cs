using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.TaskList;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
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
        private readonly ITaskListService taskListService;

        public OrderController(
            IOrderService orderService,
            ITaskListService taskListService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.taskListService = taskListService ?? throw new ArgumentNullException(nameof(taskListService));
        }

        [HttpGet]
        public async Task<IActionResult> Order(string odsCode, CallOffId callOffId)
        {
            var order = await orderService.GetOrder(callOffId);

            if (order.OrderStatus == OrderStatus.Complete)
            {
                return RedirectToAction(
                    nameof(Summary),
                    typeof(OrderController).ControllerName(),
                    new { odsCode, callOffId });
            }

            var sectionStatuses = taskListService.GetTaskListStatusModelForOrder(order);

            var orderModel = new OrderModel(odsCode, order, sectionStatuses)
            {
                DescriptionUrl = Url.Action(
                    nameof(OrderDescriptionController.OrderDescription),
                    typeof(OrderDescriptionController).ControllerName(),
                    new { odsCode, order.CallOffId }),
                BackLink = Url.Action(
                    nameof(DashboardController.Organisation),
                    typeof(DashboardController).ControllerName(),
                    new { odsCode }),
            };

            return View(orderModel);
        }

        [HttpGet("~/order/organisation/{odsCode}/order/neworder")]
        public IActionResult NewOrder(string odsCode)
        {
            var orderModel = new OrderModel(odsCode, null, new OrderTaskList())
            {
                DescriptionUrl = Url.Action(
                    nameof(OrderDescriptionController.NewOrderDescription),
                    typeof(OrderDescriptionController).ControllerName(),
                    new { odsCode }),
                BackLink = Url.Action(
                    nameof(DashboardController.Organisation),
                    typeof(DashboardController).ControllerName(),
                    new { odsCode }),
            };

            return View("Order", orderModel);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> Summary(string odsCode, CallOffId callOffId, string print = "false")
        {
            var order = await orderService.GetOrder(callOffId);

            var model = new SummaryModel(odsCode, order)
            {
                BackLink = order.OrderStatus == OrderStatus.Complete
                    ? Url.Action(
                        nameof(DashboardController.Organisation),
                        typeof(DashboardController).ControllerName(),
                        new { odsCode })
                    : Url.Action(
                        nameof(Order),
                        typeof(OrderController).ControllerName(),
                        new { odsCode, callOffId }),

                Title = order.OrderStatus switch
                {
                    OrderStatus.Complete => $"Order confirmed for {callOffId}",
                    _ => order.CanComplete()
                        ? $"Review order summary for {callOffId}"
                        : $"Order summary for {callOffId}",
                },

                AdviceText = order.OrderStatus switch
                {
                    OrderStatus.Complete => "This order has been confirmed and can no longer be changed. You can use the button to get a copy of the order summary.",
                    _ => order.CanComplete()
                        ? "Review your order summary and confirm the content is correct. Once confirmed, you'll be unable to make changes."
                        : "This is what's been added to your order so far. You must complete all mandatory steps before you can confirm your order.",
                },
            };

            return print.Equals("true", StringComparison.OrdinalIgnoreCase)
                ? View("PrintSummary", model)
                : View(model);
        }

        [HttpPost("summary")]
        public async Task<IActionResult> Summary(string odsCode, CallOffId callOffId)
        {
            var order = await orderService.GetOrder(callOffId);

            if (!order.CanComplete())
            {
                var model = new SummaryModel(odsCode, order);
                ModelState.AddModelError("Order", "Your order is incomplete. Please go back to the order and check again");
                return View(model);
            }

            await orderService.CompleteOrder(callOffId);

            return RedirectToAction();
        }
    }
}
