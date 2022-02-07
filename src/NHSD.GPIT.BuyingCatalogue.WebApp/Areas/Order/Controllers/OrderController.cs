using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.TaskList;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderTriage;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Shared;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize]
    [Area("Order")]
    [Route("order/organisation/{odsCode}/order/{callOffId}")]
    public sealed class OrderController : Controller
    {
        private readonly IOrderService orderService;
        private readonly ITaskListService taskListService;
        private readonly IOrganisationsService organisationsService;
        private readonly IPdfService pdfService;

        public OrderController(
            IOrderService orderService,
            ITaskListService taskListService,
            IOrganisationsService organisationsService,
            IPdfService pdfService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.taskListService = taskListService ?? throw new ArgumentNullException(nameof(taskListService));
            this.organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            this.pdfService = pdfService ?? throw new ArgumentNullException(nameof(pdfService));
        }

        [HttpGet]
        public async Task<IActionResult> Order(string odsCode, CallOffId callOffId)
        {
            var order = await orderService.GetOrderThin(callOffId, odsCode);

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
                BackLinkText = "Go back to dashboard",
            };

            return View(orderModel);
        }

        [HttpGet("~/order/organisation/{odsCode}/order/ready-to-start")]
        public IActionResult ReadyToStart(string odsCode, TriageOption? option = null)
        {
            var model = new ReadyToStartModel()
            {
                BackLink = Url.Action(
                    nameof(OrderTriageController.TriageSelection),
                    typeof(OrderTriageController).ControllerName(),
                    new { odsCode, option, selected = true }),
            };

            return View(model);
        }

        [HttpPost("~/order/organisation/{odsCode}/order/ready-to-start")]
        public IActionResult ReadyToStart(string odsCode, ReadyToStartModel model, TriageOption? option = null)
        {
            if (User.GetSecondaryOdsCodes().Any())
                return RedirectToAction(nameof(SelectOrganisation), new { odsCode, option });

            return RedirectToAction(
                nameof(NewOrder),
                typeof(OrderController).ControllerName(),
                new { odsCode, option });
        }

        [HttpGet("~/order/organisation/{odsCode}/order/proxy-select")]
        public async Task<IActionResult> SelectOrganisation(string odsCode, TriageOption? option = null)
        {
            var odsCodes = new List<string>(User.GetSecondaryOdsCodes())
            {
                User.GetPrimaryOdsCode(),
            };

            var organisations = await organisationsService.GetOrganisationsByOdsCodes(odsCodes.ToArray());

            var model = new SelectOrganisationModel(odsCode, organisations)
            {
                BackLink = Url.Action(nameof(ReadyToStart), new { odsCode, option }),
                Title = "Which organisation are you ordering for?",
            };

            return View(model);
        }

        [HttpPost("~/order/organisation/{odsCode}/order/proxy-select")]
        public IActionResult SelectOrganisation(string odsCode, SelectOrganisationModel model, TriageOption? option = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            return RedirectToAction(nameof(NewOrder), new { odsCode = model.SelectedOrganisation, option });
        }

        [HttpGet("~/order/organisation/{odsCode}/order/neworder")]
        public async Task<IActionResult> NewOrder(string odsCode, TriageOption? option = null)
        {
            var organisation = await organisationsService.GetOrganisationByOdsCode(odsCode);

            var orderModel = new OrderModel(odsCode, null, new OrderTaskList(), organisation.Name)
            {
                DescriptionUrl = Url.Action(
                    nameof(OrderDescriptionController.NewOrderDescription),
                    typeof(OrderDescriptionController).ControllerName(),
                    new { odsCode, option }),
                BackLink = Url.Action(
                    nameof(OrderController.ReadyToStart),
                    typeof(OrderController).ControllerName(),
                    new { odsCode, option }),
            };

            return View("Order", orderModel);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> Summary(string odsCode, CallOffId callOffId)
        {
            var order = await orderService.GetOrderForSummary(callOffId, odsCode);

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

            return View(model);
        }

        [HttpPost("summary")]
        public async Task<IActionResult> Summary(string odsCode, CallOffId callOffId, SummaryModel model)
        {
            var order = await orderService.GetOrderForSummary(callOffId, odsCode);

            if (!order.CanComplete())
            {
                model.Order = order;
                ModelState.AddModelError("Order", "Your order is incomplete. Please go back to the order and check again");
                return View(model);
            }

            await orderService.CompleteOrder(callOffId, odsCode);

            return RedirectToAction();
        }

        [HttpGet("download")]
        public async Task<IActionResult> Download(string odsCode, CallOffId callOffId)
        {
            var order = await orderService.GetOrderForSummary(callOffId, odsCode);

            string url = Url.Action(
                        nameof(OrderSummaryController.Index),
                        typeof(OrderSummaryController).ControllerName(),
                        new { odsCode, callOffId });

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = $"{Request.Scheme}://{Request.Host}{url}";
            }
            else
            {
                url = $"https://127.0.0.1{url}";
            }

            var result = pdfService.Convert(new Uri(url));

            var fileName = order.OrderStatus == OrderStatus.Complete ? $"order-summary-completed-{callOffId}.pdf" : $"order-summary-in-progress-{callOffId}.pdf";

            return File(result, "application/pdf", fileName);
        }
    }
}
