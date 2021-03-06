using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection
{
    [Authorize("Buyer")]
    [Area("Order")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/additional-services")]
    public class AdditionalServicesController : Controller
    {
        private const string SelectViewName = "SelectAdditionalServices";
        private const char Separator = ',';

        private readonly IAdditionalServicesService additionalServicesService;
        private readonly IOrderItemService orderItemService;
        private readonly IOrderService orderService;
        private readonly IRoutingService routingService;

        public AdditionalServicesController(
            IAdditionalServicesService additionalServicesService,
            IOrderItemService orderItemService,
            IOrderService orderService,
            IRoutingService routingService)
        {
            this.additionalServicesService = additionalServicesService ?? throw new ArgumentNullException(nameof(additionalServicesService));
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.routingService = routingService ?? throw new ArgumentNullException(nameof(routingService));
        }

        [HttpGet("select")]
        public async Task<IActionResult> SelectAdditionalServices(string internalOrgId, CallOffId callOffId)
        {
            var order = await orderService.GetOrderThin(callOffId, internalOrgId);

            if (order.GetSolution() == null)
            {
                return RedirectToAction(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            return View(await GetModel(internalOrgId, callOffId));
        }

        [HttpPost("select")]
        public async Task<IActionResult> SelectAdditionalServices(string internalOrgId, CallOffId callOffId, SelectServicesModel model)
        {
            var serviceIds = model.Services?
                .Where(x => x.IsSelected)
                .Select(x => x.CatalogueItemId)
                .ToList();

            if (serviceIds?.Any() ?? false)
            {
                await orderItemService.AddOrderItems(internalOrgId, callOffId, serviceIds);
            }

            var order = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);

            var route = routingService.GetRoute(
                RoutingPoint.SelectAdditionalServices,
                order,
                new RouteValues(internalOrgId, callOffId));

            return RedirectToAction(route.ActionName, route.ControllerName, route.RouteValues);
        }

        [HttpGet("edit")]
        public async Task<IActionResult> EditAdditionalServices(string internalOrgId, CallOffId callOffId)
        {
            return View(SelectViewName, await GetModel(internalOrgId, callOffId, returnToTaskList: true));
        }

        [HttpPost("edit")]
        public async Task<IActionResult> EditAdditionalServices(string internalOrgId, CallOffId callOffId, SelectServicesModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(SelectViewName, await GetModel(internalOrgId, callOffId, returnToTaskList: true));
            }

            var order = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);

            var existingServiceIds = order.GetAdditionalServices()
                .Select(x => x.CatalogueItemId)
                .ToList();

            var selectedServiceIds = model.Services?
                .Where(x => x.IsSelected)
                .Select(x => x.CatalogueItemId)
                .ToList() ?? new List<CatalogueItemId>();

            if (existingServiceIds.All(x => selectedServiceIds.Contains(x)))
            {
                var newServiceIds = selectedServiceIds.Except(existingServiceIds).ToList();

                if (!newServiceIds.Any())
                {
                    return RedirectToAction(
                        nameof(TaskListController.TaskList),
                        typeof(TaskListController).ControllerName(),
                        new { internalOrgId, callOffId });
                }

                await orderItemService.AddOrderItems(internalOrgId, callOffId, newServiceIds);

                return RedirectToAction(
                    nameof(ServiceRecipientsController.AddServiceRecipients),
                    typeof(ServiceRecipientsController).ControllerName(),
                    new { internalOrgId, callOffId, catalogueItemId = newServiceIds.First() });
            }

            var serviceIds = string.Join(Separator, selectedServiceIds);

            return RedirectToAction(
                nameof(ConfirmAdditionalServiceChanges),
                typeof(AdditionalServicesController).ControllerName(),
                new { internalOrgId, callOffId, serviceIds });
        }

        [HttpGet("confirm-changes")]
        public async Task<IActionResult> ConfirmAdditionalServiceChanges(string internalOrgId, CallOffId callOffId, string serviceIds)
        {
            var order = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var additionalServices = await additionalServicesService.GetAdditionalServicesBySolutionId(order.GetSolution().CatalogueItemId, publishedOnly: true);

            var existingServiceIds = order.GetAdditionalServices()
                .Select(x => x.CatalogueItemId)
                .ToList();

            var selectedServiceIds = serviceIds?.Split(Separator)
                .Select(CatalogueItemId.ParseExact)
                .ToList() ?? new List<CatalogueItemId>();

            var toAdd = selectedServiceIds
                .Where(x => !existingServiceIds.Contains(x))
                .Select(x => new ServiceModel
                {
                    CatalogueItemId = x,
                    Description = additionalServices.FirstOrDefault(s => s.Id == x)?.Name,
                });

            var toRemove = existingServiceIds
                .Where(x => !selectedServiceIds.Contains(x))
                .Select(x => new ServiceModel
                {
                    CatalogueItemId = x,
                    Description = additionalServices.FirstOrDefault(s => s.Id == x)?.Name,
                });

            var model = new ConfirmServiceChangesModel(internalOrgId, callOffId, CatalogueItemType.AdditionalService)
            {
                BackLink = Url.Action(
                    nameof(EditAdditionalServices),
                    typeof(AdditionalServicesController).ControllerName(),
                    new { internalOrgId, callOffId }),
                ToAdd = toAdd.ToList(),
                ToRemove = toRemove.ToList(),
            };

            return View("ConfirmChanges", model);
        }

        [HttpPost("confirm-changes")]
        public async Task<IActionResult> ConfirmAdditionalServiceChanges(string internalOrgId, CallOffId callOffId, ConfirmServiceChangesModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("ConfirmChanges", model);
            }

            if (model.ConfirmChanges is false)
            {
                return RedirectToAction(
                    nameof(TaskListController.TaskList),
                    typeof(TaskListController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            if (model.ToRemove?.Any() ?? false)
            {
                await orderItemService.DeleteOrderItems(internalOrgId, callOffId, model.ToRemove.Select(x => x.CatalogueItemId));
            }

            if (!(model.ToAdd?.Any() ?? false))
            {
                return RedirectToAction(
                    nameof(TaskListController.TaskList),
                    typeof(TaskListController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            await orderItemService.AddOrderItems(internalOrgId, callOffId, model.ToAdd.Select(x => x.CatalogueItemId));

            return RedirectToAction(
                nameof(ServiceRecipientsController.AddServiceRecipients),
                typeof(ServiceRecipientsController).ControllerName(),
                new { internalOrgId, callOffId, model.ToAdd.First().CatalogueItemId });
        }

        private async Task<SelectServicesModel> GetModel(string internalOrgId, CallOffId callOffId, bool returnToTaskList = false)
        {
            var order = await orderService.GetOrderThin(callOffId, internalOrgId);
            var additionalServices = await additionalServicesService.GetAdditionalServicesBySolutionId(order.GetSolution().CatalogueItemId, publishedOnly: true);

            var backLink = returnToTaskList
                ? Url.Action(nameof(TaskListController.TaskList), typeof(TaskListController).ControllerName(), new { internalOrgId, callOffId })
                : Url.Action(nameof(OrderController.Order), typeof(OrderController).ControllerName(), new { internalOrgId, callOffId });

            return new SelectServicesModel(order, additionalServices, CatalogueItemType.AdditionalService)
            {
                BackLink = backLink,
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                AssociatedServicesOnly = order.AssociatedServicesOnly,
            };
        }
    }
}
