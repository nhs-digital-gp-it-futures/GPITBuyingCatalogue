using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Quantity;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Quantities;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection
{
    [Authorize("Buyer")]
    [Area("Orders")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}")]
    public class QuantityController : Controller
    {
        private const string OrderItemViewName = "QuantitySelection/SelectOrderItemQuantity";
        private const string ServiceRecipientViewName = "QuantitySelection/SelectServiceRecipientQuantity";

        private readonly IGpPracticeService gpPracticeService;
        private readonly IOrderService orderService;
        private readonly IOrderQuantityService orderQuantityService;
        private readonly IRoutingService routingService;
        private readonly IOrderItemService orderItemService;

        public QuantityController(
            IGpPracticeService gpPracticeService,
            IOrderService orderService,
            IOrderQuantityService orderQuantityService,
            IRoutingService routingService,
            IOrderItemService orderItemService)
        {
            this.gpPracticeService = gpPracticeService ?? throw new ArgumentNullException(nameof(gpPracticeService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.orderQuantityService = orderQuantityService ?? throw new ArgumentNullException(nameof(orderQuantityService));
            this.routingService = routingService ?? throw new ArgumentNullException(nameof(routingService));
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
        }

        [HttpGet("quantity/{catalogueItemId}/select")]
        public async Task<IActionResult> SelectQuantity(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            RoutingSource? source = null)
        {
            var orderWrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = orderWrapper.Order;
            var orderItem = order.OrderItem(catalogueItemId);
            if (orderItem is null) return BadRequest();

            if (((IPrice)orderItem?.OrderItemPrice)?.IsPerServiceRecipient() ?? false)
            {
                return RedirectToAction(
                    nameof(SelectServiceRecipientQuantity),
                    typeof(QuantityController).ControllerName(),
                    new { internalOrgId, callOffId, catalogueItemId, source });
            }

            var route = routingService.GetRoute(
                RoutingPoint.SelectQuantityBackLink,
                orderWrapper,
                new RouteValues(internalOrgId, callOffId, catalogueItemId) { Source = source });

            var model = new SelectOrderItemQuantityModel(orderItem.CatalogueItem, orderItem.OrderItemPrice, orderItem.Quantity)
            {
                BackLink = Url.Action(route.ActionName, route.ControllerName, route.RouteValues),
                IsAmendment = callOffId.IsAmendment,
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

            var orderWrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = orderWrapper.Order;

            await orderQuantityService.SetOrderItemQuantity(
                order.Id,
                catalogueItemId,
                int.Parse(model.Quantity));

            await orderItemService.DetectChangesInFundingAndDelete(callOffId, internalOrgId, catalogueItemId);

            var route = routingService.GetRoute(
                RoutingPoint.SelectQuantity,
                orderWrapper,
                new RouteValues(internalOrgId, callOffId, catalogueItemId) { Source = model.Source });

            return RedirectToAction(route.ActionName, route.ControllerName, route.RouteValues);
        }

        [HttpGet("quantity/{catalogueItemId}/service-recipient/select")]
        public async Task<IActionResult> SelectServiceRecipientQuantity(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            RoutingSource? source = null)
        {
            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = wrapper.Order;
            var orderItem = order.OrderItem(catalogueItemId);

            var route = routingService.GetRoute(
                RoutingPoint.SelectQuantityBackLink,
                wrapper,
                new RouteValues(internalOrgId, callOffId, catalogueItemId) { Source = source });

            var recipients = wrapper.DetermineOrderRecipients(orderItem.CatalogueItemId).Select(
                x => new ServiceRecipientDto(x.OdsCode, x.OdsOrganisation?.Name, x.GetQuantityForItem(orderItem.CatalogueItemId)));

            var previousRecipients = wrapper.Previous?.OrderRecipients?.Select(
                x => new ServiceRecipientDto(x.OdsCode, x.OdsOrganisation?.Name, x.GetQuantityForItem(orderItem.CatalogueItemId)));

            var model = new SelectServiceRecipientQuantityModel(
                orderItem.CatalogueItem,
                orderItem.OrderItemPrice,
                recipients,
                previousRecipients)
            {
                BackLink = Url.Action(route.ActionName, route.ControllerName, route.RouteValues), Source = source,
            };

            if (orderItem.OrderItemPrice.ProvisioningType != ProvisioningType.Patient)
            {
                return View(ServiceRecipientViewName, model);
            }

            var solution = order.GetSolution();

            if (solution?.OrderItemPrice?.ProvisioningType is ProvisioningType.Patient
                && solution.CatalogueItemId != catalogueItemId)
            {
                await SetPracticeSizes(model, solution,  wrapper.DetermineOrderRecipients(solution.CatalogueItemId));
            }
            else
            {
                await SetPracticeSizes(model);
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

            var orderWrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = orderWrapper.Order;
            var quantities = model.ServiceRecipients
                .Select(x => new OrderItemRecipientQuantityDto
                {
                    OdsCode = x.OdsCode,
                    Quantity = string.IsNullOrWhiteSpace(x.InputQuantity)
                        ? x.Quantity
                        : int.Parse(x.InputQuantity),
                })
                .ToList();

            await orderQuantityService.SetServiceRecipientQuantities(order.Id, catalogueItemId, quantities);

            await orderItemService.DetectChangesInFundingAndDelete(callOffId, internalOrgId, catalogueItemId);

            var route = routingService.GetRoute(
                RoutingPoint.SelectQuantity,
                orderWrapper,
                new RouteValues(internalOrgId, callOffId, catalogueItemId) { Source = model.Source });

            return RedirectToAction(route.ActionName, route.ControllerName, route.RouteValues);
        }

        [HttpGet("quantity/{catalogueItemId}/view")]
        public async Task<IActionResult> ViewOrderItemQuantity(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId)
        {
            var order = (await orderService.GetOrderWithOrderItems(callOffId, internalOrgId)).Previous;
            var orderItem = order.OrderItem(catalogueItemId);

            if (((IPrice)orderItem?.OrderItemPrice)?.IsPerServiceRecipient() ?? false)
            {
                return RedirectToAction(
                    nameof(ViewServiceRecipientQuantity),
                    typeof(QuantityController).ControllerName(),
                    new { internalOrgId, callOffId, catalogueItemId });
            }

            var model = new ViewOrderItemQuantityModel(orderItem)
            {
                BackLink = Url.Action(
                    nameof(TaskListController.TaskList),
                    typeof(TaskListController).ControllerName(),
                    new { internalOrgId, callOffId }),
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            return View(model);
        }

        [HttpGet("quantity/{catalogueItemId}/service-recipient/view")]
        public async Task<IActionResult> ViewServiceRecipientQuantity(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId)
        {
            var order = (await orderService.GetOrderWithOrderItems(callOffId, internalOrgId)).Previous;
            var recipients = order.OrderRecipients;
            var orderItem = order.OrderItem(catalogueItemId);

            var model = new ViewServiceRecipientQuantityModel(orderItem, recipients)
            {
                BackLink = Url.Action(
                    nameof(TaskListController.TaskList),
                    typeof(TaskListController).ControllerName(),
                    new { internalOrgId, callOffId }),
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            return View(model);
        }

        private async Task SetPracticeSizes(SelectServiceRecipientQuantityModel model, OrderItem solution = null, ICollection<OrderRecipient> recipients = null)
        {
            var odsCodes = model.ServiceRecipients.Where(x => x.Quantity == 0).Select(x => x.OdsCode).ToArray();
            var practiceSizes =
                (await gpPracticeService.GetNumberOfPatients(odsCodes)).ToDictionary(
                    x => x.OdsCode,
                    x => x.NumberOfPatients);

            foreach (var serviceRecipient in model.ServiceRecipients)
            {
                if (serviceRecipient.Quantity > 0)
                {
                    continue;
                }

                var existing = recipients
                    ?.FirstOrDefault(x => x.OdsCode == serviceRecipient.OdsCode)
                    ?.GetQuantityForItem(solution.CatalogueItemId);

                if (existing.HasValue)
                {
                    serviceRecipient.InputQuantity = $"{existing.Value}";
                }
                else
                {
                    if (practiceSizes.TryGetValue(serviceRecipient.OdsCode, out var quantity))
                    {
                        serviceRecipient.InputQuantity = $"{quantity}";
                    }
                }
            }

            model.ServiceRecipients = model.ServiceRecipients
                .OrderBy(x => x.Quantity == 0 ? 0 : 1)
                .ThenBy(x => x.Name)
                .ToArray();
        }
    }
}
