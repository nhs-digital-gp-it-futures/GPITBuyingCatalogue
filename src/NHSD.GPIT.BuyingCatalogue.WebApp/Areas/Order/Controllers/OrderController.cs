using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Pdf;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers
{
    [Authorize("Buyer")]
    [Area("Order")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}")]
    public sealed class OrderController : Controller
    {
        private readonly IOrderService orderService;
        private readonly IOrderTaskListService orderTaskListService;
        private readonly IOrganisationsService organisationsService;
        private readonly IPdfService pdfService;
        private readonly PdfSettings pdfSettings;

        public OrderController(
            IOrderService orderService,
            IOrderTaskListService orderTaskListService,
            IOrganisationsService organisationsService,
            IPdfService pdfService,
            PdfSettings pdfSettings)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.orderTaskListService = orderTaskListService ?? throw new ArgumentNullException(nameof(orderTaskListService));
            this.organisationsService = organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
            this.pdfService = pdfService ?? throw new ArgumentNullException(nameof(pdfService));
            this.pdfSettings = pdfSettings ?? throw new ArgumentNullException(nameof(pdfSettings));
        }

        [HttpGet]
        public async Task<IActionResult> Order(string internalOrgId, CallOffId callOffId)
        {
            var wrapper = await orderService.GetOrderForTaskListStatuses(callOffId, internalOrgId);

            if (wrapper.Order.OrderStatus == OrderStatus.Completed)
            {
                return RedirectToAction(
                    nameof(Summary),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            var taskListModel = orderTaskListService.GetTaskListStatuses(wrapper);

            var orderModel = new OrderModel(internalOrgId, wrapper.Order, taskListModel)
            {
                DescriptionUrl = Url.Action(
                    nameof(OrderDescriptionController.OrderDescription),
                    typeof(OrderDescriptionController).ControllerName(),
                    new { internalOrgId, wrapper.Order.CallOffId }),
                BackLink = Url.Action(
                    nameof(DashboardController.Organisation),
                    typeof(DashboardController).ControllerName(),
                    new { internalOrgId }),
                BackLinkText = "Go back to dashboard",
            };

            return View(orderModel);
        }

        [HttpGet("~/order/organisation/{internalOrgId}/order/ready-to-start")]
        public async Task<IActionResult> ReadyToStart(string internalOrgId, OrderTriageValue? option = null, CatalogueItemType? orderType = null)
        {
            string backLink;

            if (orderType != CatalogueItemType.AssociatedService)
            {
                backLink = Url.Action(
                    nameof(OrderTriageController.TriageSelection),
                    typeof(OrderTriageController).ControllerName(),
                    new { internalOrgId, option, selected = true, orderType });
            }
            else
            {
                var actionName = User.GetSecondaryOrganisationInternalIdentifiers().Any()
                    ? nameof(OrderTriageController.SelectOrganisation)
                    : nameof(OrderTriageController.OrderItemType);

                backLink = Url.Action(
                    actionName,
                    typeof(OrderTriageController).ControllerName(),
                    new { internalOrgId, orderType });
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
        public IActionResult ReadyToStart(string internalOrgId, ReadyToStartModel model, OrderTriageValue? option = null, CatalogueItemType? orderType = null)
        {
            return RedirectToAction(
                nameof(NewOrder),
                new { internalOrgId, option, orderType });
        }

        [HttpGet("~/order/organisation/{internalOrgId}/order/new-order")]
        public async Task<IActionResult> NewOrder(string internalOrgId, OrderTriageValue? option = null, CatalogueItemType? orderType = null)
        {
            var organisation = await organisationsService.GetOrganisationByInternalIdentifier(internalOrgId);

            var orderModel = new OrderModel(internalOrgId, null, new OrderTaskListModel(), organisation.Name)
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
            var order = await orderService.GetOrderForSummary(callOffId, internalOrgId);

            var model = new SummaryModel(internalOrgId, order)
            {
                BackLink = order.OrderStatus == OrderStatus.Completed
                    ? Url.Action(
                        nameof(DashboardController.Organisation),
                        typeof(DashboardController).ControllerName(),
                        new { internalOrgId })
                    : Url.Action(
                        nameof(Order),
                        typeof(OrderController).ControllerName(),
                        new { internalOrgId, callOffId }),

                Title = order.OrderStatus switch
                {
                    OrderStatus.Completed => "Order confirmed",
                    _ => order.CanComplete()
                        ? "Review and complete your order summary"
                        : "Order summary",
                },

                AdviceText = order.OrderStatus switch
                {
                    OrderStatus.Completed => "This order has been confirmed and can no longer be changed.",
                    _ => order.CanComplete()
                        ? "Review your order summary before completing it. Once the order summary is completed, you'll be unable to make changes."
                        : "This is what's been added to your order so far. You must complete all mandatory steps before you can confirm your order.",
                },
            };

            return View(model);
        }

        [HttpGet("completed")]
        public async Task<IActionResult> Completed(string internalOrgId, CallOffId callOffId)
        {
            var order = await orderService.GetOrderForSummary(callOffId, internalOrgId);

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
            var order = await orderService.GetOrderForSummary(callOffId, internalOrgId);

            var result = pdfService.Convert(OrderSummaryUri(internalOrgId, callOffId));

            var fileName = order.OrderStatus == OrderStatus.Completed
                ? $"order-summary-completed-{callOffId}.pdf"
                : $"order-summary-in-progress-{callOffId}.pdf";

            return File(result, "application/pdf", fileName);
        }

        private Uri OrderSummaryUri(string internalOrgId, CallOffId callOffId)
        {
            var uri = Url.Action(
                nameof(OrderSummaryController.Index),
                typeof(OrderSummaryController).ControllerName(),
                new { internalOrgId, callOffId });

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                uri = $"{Request.Scheme}://{Request.Host}{uri}";
            }
            else
            {
                uri = pdfSettings.UseSslForPdf
                    ? $"https://localhost{uri}"
                    : $"http://localhost{uri}";
            }

            return new Uri(uri);
        }
    }
}
