using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Quantity;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection
{
    [Authorize("Buyer")]
    [Area("Order")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}")]
    public class QuantityController : Controller
    {
        private const string OrderItemViewName = "SelectOrderItemQuantity";
        private const string ServiceRecipientViewName = "SelectServiceRecipientQuantity";

        private readonly IGpPracticeCacheService gpPracticeCache;
        private readonly IOrderService orderService;
        private readonly IOrderItemService orderItemService;
        private readonly IOrderQuantityService orderQuantityService;
        private readonly IRoutingService routingService;

        public QuantityController(
            IGpPracticeCacheService gpPracticeCache,
            IOrderService orderService,
            IOrderItemService orderItemService,
            IOrderQuantityService orderQuantityService,
            IRoutingService routingService)
        {
            this.gpPracticeCache = gpPracticeCache ?? throw new ArgumentNullException(nameof(gpPracticeCache));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
            this.orderQuantityService = orderQuantityService ?? throw new ArgumentNullException(nameof(orderQuantityService));
            this.routingService = routingService ?? throw new ArgumentNullException(nameof(routingService));
        }

        [HttpGet("quantity/{catalogueItemId}/select")]
        public async Task<IActionResult> SelectQuantity(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            RoutingSource? source = null)
        {
            var order = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var orderItem = order.OrderItem(catalogueItemId);

            if (orderItem.OrderItemPrice.ProvisioningType == ProvisioningType.Patient)
            {
                return RedirectToAction(
                    nameof(SelectServiceRecipientQuantity),
                    typeof(QuantityController).ControllerName(),
                    new { internalOrgId, callOffId, catalogueItemId, source });
            }

            var route = routingService.GetRoute(
                RoutingPoint.SelectQuantityBackLink,
                order,
                new RouteValues(internalOrgId, callOffId, catalogueItemId) { Source = source });

            var model = new SelectOrderItemQuantityModel(orderItem)
            {
                BackLink = Url.Action(route.ActionName, route.ControllerName, route.RouteValues),
                Source = source,
            };

            return View(OrderItemViewName, model);
        }

        [HttpPost("quantity/{catalogueItemId}/select")]
        public async Task<IActionResult> SelectQuantity(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            SelectOrderItemQuantityModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(OrderItemViewName, model);
            }

            var order = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);

            await orderQuantityService.SetOrderItemQuantity(
                order.Id,
                catalogueItemId,
                int.Parse(model.Quantity));

            await orderItemService.SetOrderItemFunding(callOffId, internalOrgId, catalogueItemId);

            var route = routingService.GetRoute(
                RoutingPoint.SelectQuantity,
                order,
                new RouteValues(internalOrgId, callOffId) { Source = model.Source });

            return RedirectToAction(route.ActionName, route.ControllerName, route.RouteValues);
        }

        [HttpGet("quantity/{catalogueItemId}/service-recipient/select")]
        public async Task<IActionResult> SelectServiceRecipientQuantity(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            RoutingSource? source = null)
        {
            var order = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);

            var route = routingService.GetRoute(
                RoutingPoint.SelectQuantityBackLink,
                order,
                new RouteValues(internalOrgId, callOffId, catalogueItemId) { Source = source });

            var model = new SelectServiceRecipientQuantityModel(order.OrderItem(catalogueItemId))
            {
                BackLink = Url.Action(route.ActionName, route.ControllerName, route.RouteValues),
                Source = source,
            };

            await SetPracticeSizes(model);

            var solution = order.GetSolution();

            if (solution != null
                && solution.CatalogueItemId != catalogueItemId)
            {
                SetPracticeSizesFromSolution(model, solution);
            }

            return View(ServiceRecipientViewName, model);
        }

        [HttpPost("quantity/{catalogueItemId}/service-recipient/select")]
        public async Task<IActionResult> SelectServiceRecipientQuantity(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            SelectServiceRecipientQuantityModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(ServiceRecipientViewName, model);
            }

            var order = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var quantities = model.ServiceRecipients
                .Select(x => new OrderItemRecipientQuantityDto
                {
                    OdsCode = x.OdsCode,
                    Quantity = x.Quantity > 0
                        ? x.Quantity
                        : int.Parse(x.InputQuantity),
                })
                .ToList();

            await orderQuantityService.SetServiceRecipientQuantities(order.Id, catalogueItemId, quantities);

            await orderItemService.SetOrderItemFunding(callOffId, internalOrgId, catalogueItemId);

            var route = routingService.GetRoute(
                RoutingPoint.SelectQuantity,
                order,
                new RouteValues(internalOrgId, callOffId) { Source = model.Source });

            return RedirectToAction(route.ActionName, route.ControllerName, route.RouteValues);
        }

        private static void SetPracticeSizesFromSolution(SelectServiceRecipientQuantityModel model, OrderItem solution)
        {
            foreach (var serviceRecipient in model.ServiceRecipients)
            {
                if (serviceRecipient.Quantity != 0
                    || !string.IsNullOrWhiteSpace(serviceRecipient.InputQuantity))
                {
                    continue;
                }

                var existing = solution.OrderItemRecipients
                    .FirstOrDefault(x => x.OdsCode == serviceRecipient.OdsCode);

                if (existing?.Quantity != null)
                {
                    serviceRecipient.InputQuantity = $"{existing.Quantity}";
                }
            }
        }

        private async Task SetPracticeSizes(SelectServiceRecipientQuantityModel model)
        {
            foreach (var serviceRecipient in model.ServiceRecipients)
            {
                var quantity = await gpPracticeCache.GetNumberOfPatients(serviceRecipient.OdsCode);

                if (quantity.HasValue)
                {
                    serviceRecipient.Quantity = quantity.Value;
                }
            }

            model.ServiceRecipients = model.ServiceRecipients
                .OrderBy(x => x.Quantity == 0 ? 0 : 1)
                .ThenBy(x => x.Name)
                .ToArray();
        }
    }
}
