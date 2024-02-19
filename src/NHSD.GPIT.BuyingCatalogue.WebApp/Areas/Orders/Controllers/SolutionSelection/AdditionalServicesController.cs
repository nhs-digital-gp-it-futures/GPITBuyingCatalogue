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

        [HttpGet("add")]
        public async Task<IActionResult> SelectAdditionalServices(string internalOrgId, CallOffId callOffId)
        {
            return View(SelectViewName, await GetModel(internalOrgId, callOffId, returnToTaskList: true));
        }

        [HttpPost("add")]
        public async Task<IActionResult> SelectAdditionalServices(string internalOrgId, CallOffId callOffId, SelectServicesModel model)
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

            var newServiceIds = selectedServiceIds.Except(currentServiceIds).ToList();

            if (newServiceIds.Any())
            {
                await orderItemService.AddOrderItems(internalOrgId, callOffId, newServiceIds);
            }

            return RedirectToAction(
                        nameof(TaskListController.TaskList),
                        typeof(TaskListController).ControllerName(),
                        new { internalOrgId, callOffId });
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
                order.GetSolutionOrderItem().CatalogueItemId,
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
                IsAmendment = wrapper.IsAmendment,
                SolutionName = order.OrderType.GetSolutionNameFromOrder(order),
                SolutionId = order.GetSolutionId(),
            };
        }
    }
}
