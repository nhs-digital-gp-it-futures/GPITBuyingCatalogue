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
        private readonly IOrderQuantityService orderQuantityService;
        private readonly IRoutingService routingService;

        public QuantityController(
            IGpPracticeCacheService gpPracticeCache,
            IOrderService orderService,
            IOrderQuantityService orderQuantityService,
            IRoutingService routingService)
        {
            this.gpPracticeCache = gpPracticeCache ?? throw new ArgumentNullException(nameof(gpPracticeCache));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.orderQuantityService = orderQuantityService ?? throw new ArgumentNullException(nameof(orderQuantityService));
            this.routingService = routingService ?? throw new ArgumentNullException(nameof(routingService));
        }

        [HttpGet("quantity/{catalogueItemId}/select")]
        public async Task<IActionResult> SelectQuantity(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            var order = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var orderItem = order.OrderItem(catalogueItemId);

            if (orderItem.OrderItemPrice.ProvisioningType == ProvisioningType.Patient)
            {
                return RedirectToAction(
                    nameof(SelectServiceRecipientQuantity),
                    typeof(QuantityController).ControllerName(),
                    new { internalOrgId, callOffId, catalogueItemId });
            }

            var model = new SelectOrderItemQuantityModel(orderItem)
            {
                BackLink = Url.Action(
                    nameof(PricesController.EditPrice),
                    typeof(PricesController).ControllerName(),
                    new { internalOrgId, callOffId, catalogueItemId }),
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

            var route = routingService.GetRoute(
                RoutingSource.SelectQuantity,
                order,
                new RouteValues(internalOrgId, callOffId));

            return RedirectToAction(route.ActionName, route.ControllerName, route.RouteValues);
        }

        [HttpGet("quantity/{catalogueItemId}/service-recipient/select")]
        public async Task<IActionResult> SelectServiceRecipientQuantity(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            var order = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);

            var model = new SelectServiceRecipientQuantityModel(order.OrderItem(catalogueItemId))
            {
                BackLink = Url.Action(
                    nameof(PricesController.EditPrice),
                    typeof(PricesController).ControllerName(),
                    new { internalOrgId, callOffId, catalogueItemId }),
            };

            await SetPracticeSizes(model);

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

            var route = routingService.GetRoute(
                RoutingSource.SelectQuantity,
                order,
                new RouteValues(internalOrgId, callOffId));

            return RedirectToAction(route.ActionName, route.ControllerName, route.RouteValues);
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
