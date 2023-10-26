using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts
{
    [Authorize("Buyer")]
    [Area("Orders")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/delivery-dates")]
    public class DeliveryDatesController : Controller
    {
        public const string DateFormat = "yyyyMMdd";

        private readonly IDeliveryDateService deliveryDateService;
        private readonly IOrderService orderService;
        private readonly IRoutingService routingService;

        public DeliveryDatesController(
            IDeliveryDateService deliveryDateService,
            IOrderService orderService,
            IRoutingService routingService)
        {
            this.deliveryDateService = deliveryDateService ?? throw new ArgumentNullException(nameof(deliveryDateService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.routingService = routingService ?? throw new ArgumentNullException(nameof(routingService));
        }

        [HttpGet("select")]
        public async Task<IActionResult> SelectDate(string internalOrgId, CallOffId callOffId, string returnUrl = null)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;

            var model = new SelectDateModel(internalOrgId, callOffId, order)
            {
                BackLink = returnUrl ?? Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpPost("select")]
        public async Task<IActionResult> SelectDate(string internalOrgId, CallOffId callOffId, SelectDateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;

            if (order.DeliveryDate == null
                || order.DeliveryDate.Value == model.Date!.Value)
            {
                if (order.DeliveryDate == null)
                {
                    await deliveryDateService.SetDeliveryDate(internalOrgId, callOffId, model.Date!.Value);
                }

                return RedirectToAction(
                    nameof(EditDates),
                    typeof(DeliveryDatesController).ControllerName(),
                    new { internalOrgId, callOffId, CatalogueItemId = order.GetOrderItemIds().First() });
            }

            var deliveryDate = model.Date?.ToString(DateFormat);

            return RedirectToAction(
                nameof(ConfirmChanges),
                typeof(DeliveryDatesController).ControllerName(),
                new { internalOrgId, callOffId, deliveryDate });
        }

        [HttpGet("confirm")]
        public async Task<IActionResult> ConfirmChanges(string internalOrgId, CallOffId callOffId, string deliveryDate)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;
            var newDate = DateTime.ParseExact(deliveryDate, DateFormat, CultureInfo.InvariantCulture);

            var model = new ConfirmChangesModel(internalOrgId, callOffId, order.DeliveryDate!.Value, newDate)
            {
                BackLink = Url.Action(
                    nameof(SelectDate),
                    typeof(DeliveryDatesController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmChanges(string internalOrgId, CallOffId callOffId, ConfirmChangesModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.ConfirmChanges == false)
            {
                return RedirectToAction(
                    nameof(Review),
                    typeof(DeliveryDatesController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            await deliveryDateService.SetDeliveryDate(internalOrgId, callOffId, model.NewDeliveryDate);

            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;
            var catalogueItemId = order.GetOrderItemIds().First();

            return RedirectToAction(
                nameof(EditDates),
                typeof(DeliveryDatesController).ControllerName(),
                new { model.InternalOrgId, model.CallOffId, catalogueItemId });
        }

        [HttpGet("{catalogueItemId}/edit")]
        public async Task<IActionResult> EditDates(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId, RoutingSource? source = null)
        {
            var orderWrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);

            var route = routingService.GetRoute(
                RoutingPoint.EditDeliveryDatesBackLink,
                orderWrapper,
                new RouteValues(internalOrgId, callOffId, catalogueItemId) { Source = source });

            var model = new EditDatesModel(orderWrapper, catalogueItemId, source)
            {
                BackLink = Url.Action(route.ActionName, route.ControllerName, route.RouteValues),
            };

            return View(model);
        }

        [HttpPost("{catalogueItemId}/edit")]
        public async Task<IActionResult> EditDates(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId, EditDatesModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var orderWrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = orderWrapper.Order;

            var deliveryDates = model.Recipients
                .Select(x => new RecipientDeliveryDateDto(x.OdsCode, x.Date!.Value))
                .ToList();

            await deliveryDateService.SetDeliveryDates(order.Id, catalogueItemId, deliveryDates);

            var route = routingService.GetRoute(
                RoutingPoint.EditDeliveryDates,
                orderWrapper,
                new RouteValues(internalOrgId, callOffId, catalogueItemId) { Source = model.Source });

            return RedirectToAction(route.ActionName, route.ControllerName, route.RouteValues);
        }

        [HttpGet("{catalogueItemId}/match")]
        public async Task<IActionResult> MatchDates(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            var order = (await orderService.GetOrderWithOrderItems(callOffId, internalOrgId)).Order;
            var catalogueItem = order.OrderItem(catalogueItemId).CatalogueItem;
            var previousCatalogueItemId = order.GetPreviousOrderItemId(catalogueItemId)!.Value;

            var model = new MatchDatesModel(internalOrgId, callOffId, catalogueItem)
            {
                BackLink = Url.Action(
                    nameof(EditDates),
                    typeof(DeliveryDatesController).ControllerName(),
                    new { internalOrgId, callOffId, catalogueItemId = previousCatalogueItemId }),
            };

            return View(model);
        }

        [HttpPost("{catalogueItemId}/match")]
        public async Task<IActionResult> MatchDates(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId, MatchDatesModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = wrapper.Order;

            if (model.MatchDates == true)
            {
                var solutionId = order.GetSolutionId();

                if (solutionId != null)
                {
                    await deliveryDateService.MatchDeliveryDates(order.Id, solutionId.Value, catalogueItemId);
                }
            }
            else
            {
                var recipients = wrapper.DetermineOrderRecipients(catalogueItemId)
                    .Select(x => new RecipientDeliveryDateDto(x.OdsCode, order.DeliveryDate!.Value))
                    .ToList();

                await deliveryDateService.SetDeliveryDates(order.Id, catalogueItemId, recipients);
            }

            return RedirectToAction(
                nameof(EditDates),
                typeof(DeliveryDatesController).ControllerName(),
                new { internalOrgId, callOffId, catalogueItemId });
        }

        [HttpGet("review")]
        public async Task<IActionResult> Review(string internalOrgId, CallOffId callOffId)
        {
            var order = (await orderService.GetOrderWithOrderItems(callOffId, internalOrgId)).Order;

            var model = new ReviewModel(order)
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }
    }
}
