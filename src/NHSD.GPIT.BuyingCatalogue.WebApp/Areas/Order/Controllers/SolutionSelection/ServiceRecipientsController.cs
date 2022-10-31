﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.ServiceRecipients;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection
{
    [Authorize("Buyer")]
    [Area("Order")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/item/{catalogueItemId}/service-recipients")]
    public class ServiceRecipientsController : Controller
    {
        private readonly IOdsService odsService;
        private readonly IOrderItemRecipientService orderItemRecipientService;
        private readonly IOrderService orderService;
        private readonly IRoutingService routingService;

        public ServiceRecipientsController(
            IOdsService odsService,
            IOrderItemRecipientService orderItemRecipientService,
            IOrderService orderService,
            IRoutingService routingService)
        {
            this.odsService = odsService ?? throw new ArgumentNullException(nameof(odsService));
            this.orderItemRecipientService = orderItemRecipientService ?? throw new ArgumentNullException(nameof(orderItemRecipientService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.routingService = routingService ?? throw new ArgumentNullException(nameof(routingService));
        }

        [HttpGet("add")]
        public async Task<IActionResult> AddServiceRecipients(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            SelectionMode? selectionMode = null,
            RoutingSource? source = null,
            string importedRecipients = null)
        {
            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = wrapper.Order;
            var orderItem = order.OrderItem(catalogueItemId);
            var serviceRecipients = await GetServiceRecipients(internalOrgId, catalogueItemId, wrapper);

            var route = routingService.GetRoute(
                RoutingPoint.SelectServiceRecipientsBackLink,
                order,
                new RouteValues(internalOrgId, callOffId, catalogueItemId) { Source = source, });

            var splitImportedRecipients = importedRecipients?.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var model = new SelectRecipientsModel(orderItem, serviceRecipients, selectionMode, splitImportedRecipients)
            {
                BackLink = Url.Action(route.ActionName, route.ControllerName, route.RouteValues),
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                CatalogueItemId = catalogueItemId,
                Source = source,
                AssociatedServicesOnly = order.AssociatedServicesOnly,
            };

            var baseOrderItem = model.AssociatedServicesOnly
                ? order.GetAssociatedServices().FirstOrDefault()
                : order.GetSolution();

            if (splitImportedRecipients is null)
                model.PreSelectRecipients(baseOrderItem);

            return View("SelectRecipients", model);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddServiceRecipients(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            SelectRecipientsModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("SelectRecipients", model);
            }

            var order = (await orderService.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId)).Order;

            await orderItemRecipientService.AddOrderItemRecipients(
                order.Id,
                catalogueItemId,
                model.GetSelectedItems());

            var route = routingService.GetRoute(
                RoutingPoint.SelectServiceRecipients,
                order,
                new RouteValues(internalOrgId, callOffId, catalogueItemId) { Source = model.Source });

            return RedirectToAction(route.ActionName, route.ControllerName, route.RouteValues);
        }

        [HttpGet("edit")]
        public async Task<IActionResult> EditServiceRecipients(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            SelectionMode? selectionMode = null,
            RoutingSource? source = null,
            string importedRecipients = null)
        {
            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = wrapper.Order;
            var orderItem = wrapper.OrderItem(catalogueItemId);
            var serviceRecipients = await GetServiceRecipients(internalOrgId, catalogueItemId, wrapper);

            var route = routingService.GetRoute(
                RoutingPoint.SelectServiceRecipientsBackLink,
                order,
                new RouteValues(internalOrgId, callOffId, catalogueItemId) { Source = source, });

            var splitImportedRecipients = importedRecipients?.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var model = new SelectRecipientsModel(orderItem, serviceRecipients, selectionMode, splitImportedRecipients)
            {
                BackLink = Url.Action(route.ActionName, route.ControllerName, route.RouteValues),
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                CatalogueItemId = catalogueItemId,
                Source = source,
                AssociatedServicesOnly = order.AssociatedServicesOnly,
                IsAdding = false,
            };

            return View("SelectRecipients", model);
        }

        [HttpPost("edit")]
        public async Task<IActionResult> EditServiceRecipients(
            string internalOrgId,
            CallOffId callOffId,
            CatalogueItemId catalogueItemId,
            SelectRecipientsModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("SelectRecipients", model);
            }

            var order = (await orderService.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId)).Order;

            await orderItemRecipientService.UpdateOrderItemRecipients(
                order.Id,
                catalogueItemId,
                model.GetSelectedItems());

            var route = routingService.GetRoute(
                RoutingPoint.SelectServiceRecipients,
                order,
                new RouteValues(internalOrgId, callOffId, catalogueItemId) { Source = model.Source });

            return RedirectToAction(route.ActionName, route.ControllerName, route.RouteValues);
        }

        private async Task<List<ServiceRecipientModel>> GetServiceRecipients(string internalOrgId, CatalogueItemId catalogueItemId, OrderWrapper wrapper)
        {
            var recipients = await odsService.GetServiceRecipientsByParentInternalIdentifier(internalOrgId);

            var output = recipients
                .OrderBy(x => x.Name)
                .Select(x => new ServiceRecipientModel
                {
                    Name = x.Name,
                    OdsCode = x.OrgId,
                })
                .ToList();

            output.RemoveAll(x => wrapper.HasPreviousServiceRecipientFor(catalogueItemId, x.OdsCode));

            return output;
        }
    }
}
