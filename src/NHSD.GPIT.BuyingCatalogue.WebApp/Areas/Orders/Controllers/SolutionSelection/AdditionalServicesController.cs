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
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Shared;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Services;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection
{
    [Authorize("Buyer")]
    [Area("Orders")]
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
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;

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

            var orderWrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var order = orderWrapper.Order;

            var route = routingService.GetRoute(
                RoutingPoint.SelectAdditionalServices,
                orderWrapper,
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

            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);

            var currentServiceIds = GetCurrentServiceIds(callOffId, wrapper);

            var selectedServiceIds = model.Services?
                .Where(x => x.IsSelected)
                .Select(x => x.CatalogueItemId)
                .ToList() ?? new List<CatalogueItemId>();

            if (currentServiceIds.All(selectedServiceIds.Contains))
            {
                var newServiceIds = selectedServiceIds.Except(currentServiceIds).ToList();

                if (!newServiceIds.Any())
                {
                    return RedirectToAction(
                        nameof(TaskListController.TaskList),
                        typeof(TaskListController).ControllerName(),
                        new { internalOrgId, callOffId });
                }

                await orderItemService.AddOrderItems(internalOrgId, callOffId, newServiceIds);

                return RedirectToAction(
                    nameof(PricesController.SelectPrice),
                    typeof(PricesController).ControllerName(),
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
            var wrapper = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
            var solution = wrapper.RolledUp.GetSolution();

            var additionalServices = await additionalServicesService.GetAdditionalServicesBySolutionId(
                solution.CatalogueItemId,
                publishedOnly: true);

            var currentServiceIds = GetCurrentServiceIds(callOffId, wrapper);

            var selectedServiceIds = serviceIds?.Split(Separator)
                .Select(CatalogueItemId.ParseExact)
                .ToList() ?? new List<CatalogueItemId>();

            var toAdd = selectedServiceIds
                .Where(x => !currentServiceIds.Contains(x))
                .Select(x => new ServiceModel
                {
                    CatalogueItemId = x,
                    Description = additionalServices.FirstOrDefault(s => s.Id == x)?.Name,
                });

            var toRemove = currentServiceIds
                .Where(x => !selectedServiceIds.Contains(x))
                .Select(x => new ServiceModel
                {
                    CatalogueItemId = x,
                    Description = additionalServices.FirstOrDefault(s => s.Id == x)?.Name,
                });

            var model = new ConfirmServiceChangesModel(internalOrgId, CatalogueItemType.AdditionalService)
            {
                BackLink = Url.Action(
                    nameof(EditAdditionalServices),
                    typeof(AdditionalServicesController).ControllerName(),
                    new { internalOrgId, callOffId }),
                ToAdd = toAdd.ToList(),
                ToRemove = toRemove.ToList(),
            };

            return View("Services/ConfirmChanges", model);
        }

        [HttpPost("confirm-changes")]
        public async Task<IActionResult> ConfirmAdditionalServiceChanges(string internalOrgId, CallOffId callOffId, ConfirmServiceChangesModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Services/ConfirmChanges", model);
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
                nameof(PricesController.SelectPrice),
                typeof(PricesController).ControllerName(),
                new { internalOrgId, callOffId, model.ToAdd.First().CatalogueItemId });
        }

        private static List<CatalogueItemId> GetCurrentServiceIds(CallOffId callOffId, OrderWrapper wrapper)
        {
            if (!callOffId.IsAmendment)
            {
                return wrapper.Order
                    .GetAdditionalServices()
                    .Select(x => x.CatalogueItemId)
                    .ToList();
            }

            var existingServiceIds = wrapper.Previous?
                .GetAdditionalServices()
                .Select(x => x.CatalogueItemId) ?? new List<CatalogueItemId>();

            return wrapper.Order
                .GetAdditionalServices()
                .Select(x => x.CatalogueItemId)
                .Except(existingServiceIds)
                .ToList();
        }

        private async Task<SelectServicesModel> GetModel(string internalOrgId, CallOffId callOffId, bool returnToTaskList = false)
        {
            const CatalogueItemType catalogueItemType = CatalogueItemType.AdditionalService;

            var wrapper = await orderService.GetOrderThin(callOffId, internalOrgId);
            var order = wrapper.RolledUp;

            var additionalServices = await additionalServicesService.GetAdditionalServicesBySolutionId(
                order.GetSolution().CatalogueItemId,
                publishedOnly: true);

            var backLink = returnToTaskList
                ? Url.Action(nameof(TaskListController.TaskList), typeof(TaskListController).ControllerName(), new { internalOrgId, callOffId })
                : Url.Action(nameof(OrderController.Order), typeof(OrderController).ControllerName(), new { internalOrgId, callOffId });

            return new SelectServicesModel(
                wrapper.Previous?.GetServices(catalogueItemType) ?? Enumerable.Empty<CatalogueItem>(),
                order.GetServices(catalogueItemType),
                additionalServices)
            {
                BackLink = backLink,
                InternalOrgId = internalOrgId,
                AssociatedServicesOnly = order.AssociatedServicesOnly,
                IsAmendment = wrapper.IsAmendment,
                SolutionName = order.AssociatedServicesOnly
                    ? wrapper.RolledUp.Solution.Name
                    : wrapper.RolledUp.GetSolution()?.CatalogueItem.Name,
                SolutionId = order.GetSolution().CatalogueItemId,
            };
        }
    }
}
