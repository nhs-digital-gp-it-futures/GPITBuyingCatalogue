using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Services;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection
{
    [Authorize("Buyer")]
    [Area("Orders")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/associated-services")]
    public class AssociatedServicesController : Controller
    {
        private const string SelectViewName = "Services/SelectAssociatedServices";
        private const char Separator = ',';

        private readonly IAssociatedServicesService associatedServicesService;
        private readonly IOrderItemService orderItemService;
        private readonly IOrderService orderService;
        private readonly IOrderQuantityService orderQuantityService;
        private readonly IContractBillingService contractBillingService;
        private readonly IRequirementsService requirementsService;

        public AssociatedServicesController(
            IAssociatedServicesService associatedServicesService,
            IOrderItemService orderItemService,
            IOrderService orderService,
            IOrderQuantityService orderQuantityService,
            IContractBillingService contractBillingService,
            IRequirementsService requirementsService)
        {
            this.associatedServicesService = associatedServicesService ?? throw new ArgumentNullException(nameof(associatedServicesService));
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.orderQuantityService = orderQuantityService ?? throw new ArgumentNullException(nameof(orderQuantityService));
            this.contractBillingService = contractBillingService ?? throw new ArgumentNullException(nameof(contractBillingService));
            this.requirementsService = requirementsService ?? throw new ArgumentNullException(nameof(requirementsService));
        }

        [HttpGet("add")]
        public async Task<IActionResult> SelectAssociatedServices(string internalOrgId, CallOffId callOffId, RoutingSource? source = null)
        {
            var wrapper = await orderService.GetOrderThin(callOffId, internalOrgId);
            var order = wrapper.Order;
            var associatedServices = await associatedServicesService.GetPublishedAssociatedServicesForSolution(
                order.GetSolutionId(),
                order.OrderType.ToPracticeReorganisationType);

            if (order.OrderType.MergerOrSplit && associatedServices.Count == 1)
            {
                var catalogueItemId = associatedServices.Select(s => s.Id).First();
                await AddOrderItems(internalOrgId, callOffId, new[] { catalogueItemId }.ToList());
                await orderQuantityService.SetServiceRecipientQuantitiesToSameValue(order.Id, catalogueItemId, 1);
                return RedirectToAction(
                    nameof(TaskListController.TaskList),
                    typeof(TaskListController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            var model = GetSelectServicesModel(internalOrgId, callOffId, wrapper, associatedServices, source);
            return View(SelectViewName, model);
        }

        [HttpPost("add")]
        public async Task<IActionResult> SelectAssociatedServices(string internalOrgId, CallOffId callOffId, SelectServicesModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(SelectViewName, model);
            }

            var serviceIds = model.Services
                .Where(x => x.IsSelected)
                .Select(x => x.CatalogueItemId)
                .ToList();

            await AddOrderItems(internalOrgId, callOffId, serviceIds);
            CatalogueItemId? catalogueItemId = serviceIds.Any()
                ? serviceIds.First()
                : null;

            return RedirectToAction(
                    nameof(TaskListController.TaskList),
                    typeof(TaskListController).ControllerName(),
                    new { internalOrgId, callOffId });
        }

        private async Task AddOrderItems(string internalOrgId, CallOffId callOffId, List<CatalogueItemId> serviceIds)
        {
            if (serviceIds.Any())
            {
                await orderItemService.AddOrderItems(internalOrgId, callOffId, serviceIds);
            }
        }

        private SelectServicesModel GetSelectServicesModel(
            string internalOrgId,
            CallOffId callOffId,
            OrderWrapper wrapper,
            List<CatalogueItem> associatedServices,
            RoutingSource? source = RoutingSource.Dashboard)
        {
            var order = wrapper.Order;

            return new SelectServicesModel(
                order.GetServices(CatalogueItemType.AssociatedService),
                associatedServices)
            {
                SolutionId = order.GetSolutionId(),
                BackLink = GetBackLink(internalOrgId, callOffId, source),
                InternalOrgId = internalOrgId,
                AssociatedServicesOnly = order.OrderType.AssociatedServicesOnly,
                SolutionName = order.OrderType.GetSolutionNameFromOrder(wrapper.RolledUp),
            };
        }

        private string GetBackLink(string internalOrgId, CallOffId callOffId, RoutingSource? source)
        {
            var defaultRouteValues = new
            {
                internalOrgId,
                callOffId,
            };

            return source switch
            {
                RoutingSource.EditSolution => Url.Action(
                    nameof(CatalogueSolutionsController.EditSolutionAssociatedServicesOnly),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    defaultRouteValues),

                RoutingSource.SelectSolution => Url.Action(
                    nameof(CatalogueSolutionsController.SelectSolutionAssociatedServicesOnly),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new
                    {
                        internalOrgId,
                        callOffId,
                        source = RoutingSource.SelectAssociatedServices,
                    }),

                RoutingSource.TaskList => Url.Action(
                    nameof(TaskListController.TaskList),
                    typeof(TaskListController).ControllerName(),
                    defaultRouteValues),

                _ => Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    defaultRouteValues),
            };
        }
    }
}
