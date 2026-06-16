using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
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
        public async Task<IActionResult> SelectDate(string internalOrgId, CallOffId callOffId, string returnUrl = null, bool? setAllPDD = null)
        {
            var orderWrapper = await orderService.GetOrderThin(callOffId, internalOrgId);

            var model = new SelectDateModel(internalOrgId, callOffId, orderWrapper.Order, setAllPDD, orderWrapper.Previous?.DeliveryDate)
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

            if (order.DeliveryDate.HasValue && order.DeliveryDate.Value != model.Date!.Value)
            {
                var deliveryDate = model.Date?.ToString(DateFormat);

                return RedirectToAction(
                    nameof(ConfirmChanges),
                    typeof(DeliveryDatesController).ControllerName(),
                    new { internalOrgId, callOffId, deliveryDate, model.ApplyToAll });
            }

            if (model.ApplyToAll == true)
            {
                await deliveryDateService.SetAllDeliveryDates(internalOrgId, callOffId, model.Date!.Value);

                return RedirectToAction(
                    nameof(Review),
                    typeof(DeliveryDatesController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            if (order.DeliveryDate == null)
            {
                await deliveryDateService.SetDeliveryDate(internalOrgId, callOffId, model.Date!.Value);
            }
            else
            {
                await deliveryDateService.ResetRecipientDeliveryDates(order.Id);
            }

            int? orderItemId = order.GetOrderItemIds().Count > 0 ? order.GetOrderItemIds()[0] : null;

            return RedirectToAction(
                nameof(EditDates),
                typeof(DeliveryDatesController).ControllerName(),
                new { internalOrgId, callOffId, OrderItemId = orderItemId });
        }

        [HttpGet("confirm")]
        public async Task<IActionResult> ConfirmChanges(string internalOrgId, CallOffId callOffId, string deliveryDate, bool applyToAll)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;
            var newDate = DateTime.ParseExact(deliveryDate, DateFormat, CultureInfo.InvariantCulture);

            var model = new ConfirmChangesModel(internalOrgId, callOffId, order.DeliveryDate!.Value, newDate, applyToAll)
            {
                BackLink = Url.Action(
                    nameof(SelectDate),
                    typeof(DeliveryDatesController).ControllerName(),
                    new { internalOrgId, callOffId, applyToAll }),
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

            if (model.ConfirmChanges == true)
            {
                if (!model.ApplyToAll)
                {
                    var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;

                    await deliveryDateService.SetDeliveryDate(internalOrgId, callOffId, model.NewDeliveryDate);
                    await deliveryDateService.ResetRecipientDeliveryDates(order.Id);

                    int? orderItemId = order.GetOrderItemIds().Count > 0 ? order.GetOrderItemIds()[0] : null;

                    return RedirectToAction(
                        nameof(EditDates),
                        typeof(DeliveryDatesController).ControllerName(),
                        new { model.InternalOrgId, model.CallOffId, orderItemId });
                }

                await deliveryDateService.SetAllDeliveryDates(internalOrgId, callOffId, model.NewDeliveryDate);
            }

            return RedirectToAction(
                nameof(Review),
                typeof(DeliveryDatesController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("{orderItemId}/edit")]
        public async Task<IActionResult> EditDates(string internalOrgId, CallOffId callOffId, int orderItemId, RoutingSource? source = null)
        {
            var orderWrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var orderItem = orderWrapper.Order.OrderItem(orderItemId);
            var item = orderItem.Parent?.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService
                ? orderItem.Parent
                : orderItem;

            // If there are no new recipients for this item (e.g. the original solution in an amend)
            if (orderWrapper.DetermineOrderRecipients(item) is null or { Count: 0 })
            {
                RoutingResult next = routingService.GetRoute(
                    RoutingPoint.EditDeliveryDates,
                    orderWrapper,
                    new RouteValues(internalOrgId, callOffId, orderItem.CatalogueItemId) { Source = source, OrderItemId = orderItemId });

                return RedirectToAction(next.ActionName, next.ControllerName, next.RouteValues);
            }

            var route = routingService.GetRoute(
                RoutingPoint.EditDeliveryDatesBackLink,
                orderWrapper,
                new RouteValues(internalOrgId, callOffId, orderItem.CatalogueItemId) { Source = source, OrderItemId = orderItemId });

            var model = new EditDatesModel(orderWrapper, orderItemId, source)
            {
                BackLink = Url.Action(route.ActionName, route.ControllerName, route.RouteValues),
            };

            return View(model);
        }

        [HttpPost("{orderItemId}/edit")]
        public async Task<IActionResult> EditDates(string internalOrgId, CallOffId callOffId, int orderItemId, EditDatesModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var orderWrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = orderWrapper.Order;
            var orderItem = order.OrderItem(orderItemId);

            var recipients = model.Recipients.SelectMany(x => x.Value).ToList();

            var deliveryDates = recipients
                .Select(x => new RecipientDeliveryDateDto(x.OdsCode, x.Date!.Value))
                .ToList();

            await deliveryDateService.SetDeliveryDates(order.Id, orderItem, deliveryDates);

            var route = routingService.GetRoute(
                RoutingPoint.EditDeliveryDates,
                orderWrapper,
                new RouteValues(internalOrgId, callOffId, orderItem.CatalogueItemId) { Source = model.Source, OrderItemId = orderItemId });

            return RedirectToAction(route.ActionName, route.ControllerName, route.RouteValues);
        }

        [HttpGet("{orderItemId}/match")]
        public async Task<IActionResult> MatchDates(string internalOrgId, CallOffId callOffId, int orderItemId)
        {
            var order = (await orderService.GetOrderWithOrderItems(callOffId, internalOrgId)).Order;
            var orderItem = order.OrderItem(orderItemId);
            var catalogueItem = orderItem.CatalogueItem;
            var previousCatalogueItemId = order.GetPreviousOrderItemId(orderItemId)!.Value;

            var model = new MatchDatesModel(internalOrgId, callOffId, catalogueItem)
            {
                BackLink = Url.Action(
                    nameof(EditDates),
                    typeof(DeliveryDatesController).ControllerName(),
                    new { internalOrgId, callOffId, orderItemId = previousCatalogueItemId }),
            };

            return View(model);
        }

        [HttpPost("{orderItemId}/match")]
        public async Task<IActionResult> MatchDates(string internalOrgId, CallOffId callOffId, int orderItemId, MatchDatesModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = wrapper.Order;
            var solutionOrderItem = order.GetSolutionOrderItem();
            var orderItem = order.OrderItem(orderItemId);

            var recipients = wrapper.DetermineOrderRecipients(orderItem);
            List<RecipientDeliveryDateDto> dates = model.MatchDates == true && solutionOrderItem is not null
                ? recipients
                    .Select(x => new RecipientDeliveryDateDto(
                        x.RecipientOdsCode,
                        x.GetDeliveryDateForItem(solutionOrderItem.Id).GetValueOrDefault()))
                    .ToList()
                : recipients
                    .Select(x => new RecipientDeliveryDateDto(x.RecipientOdsCode, order.DeliveryDate!.Value))
                    .ToList();

            await deliveryDateService.SetDeliveryDates(order.Id, orderItem, dates);

            if (orderItem.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService
                && orderItem.Services.Count > 0)
            {
                foreach (var service in orderItem.Services)
                {
                    await deliveryDateService.SetDeliveryDates(order.Id, service, dates);
                }
            }

            return RedirectToAction(
                nameof(EditDates),
                typeof(DeliveryDatesController).ControllerName(),
                new { internalOrgId, callOffId, orderItemId });
        }

        [HttpGet("review")]
        public async Task<IActionResult> Review(string internalOrgId, CallOffId callOffId)
        {
            var orderWrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);

            var model = new ReviewModel(orderWrapper)
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
