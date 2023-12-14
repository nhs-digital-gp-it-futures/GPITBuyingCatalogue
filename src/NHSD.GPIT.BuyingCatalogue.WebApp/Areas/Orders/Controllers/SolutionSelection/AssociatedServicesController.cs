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
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.AssociatedServices;
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
        public IActionResult AddAssociatedServices(string internalOrgId, CallOffId callOffId, bool selected = false)
        {
            var model = new AddAssociatedServicesModel
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            };

            if (selected)
            {
                model.AdditionalServicesRequired = YesNoRadioButtonTagHelper.Yes;
            }

            return View(model);
        }

        [HttpPost("add")]
        public IActionResult AddAssociatedServices(string internalOrgId, CallOffId callOffId, AddAssociatedServicesModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var addAssociatedServices = model.AdditionalServicesRequired.EqualsIgnoreCase(YesNoRadioButtonTagHelper.Yes);

            if (addAssociatedServices)
            {
                return RedirectToAction(
                    nameof(SelectAssociatedServices),
                    new { internalOrgId, callOffId, source = RoutingSource.AddAssociatedServices });
            }

            return RedirectToAction(
                nameof(TaskListController.TaskList),
                typeof(TaskListController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("select")]
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
                return RedirectToPriceOrTaskList(internalOrgId, callOffId, catalogueItemId);
            }

            var model = GetSelectServicesModel(internalOrgId, callOffId, wrapper, associatedServices, source);
            return View(SelectViewName, model);
        }

        [HttpPost("select")]
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

            return RedirectToPriceOrTaskList(internalOrgId, callOffId, catalogueItemId);
        }

        [HttpGet("edit")]
        public async Task<IActionResult> EditAssociatedServices(string internalOrgId, CallOffId callOffId)
        {
            var wrapper = await orderService.GetOrderThin(callOffId, internalOrgId);
            var order = wrapper.Order;
            var associatedServices = await associatedServicesService.GetPublishedAssociatedServicesForSolution(
                order.GetSolutionId(),
                order.OrderType.ToPracticeReorganisationType);

            var model = GetSelectServicesModel(internalOrgId, callOffId, wrapper, associatedServices, RoutingSource.TaskList);
            return View(SelectViewName, model);
        }

        [HttpPost("edit")]
        public async Task<IActionResult> EditAssociatedServices(string internalOrgId, CallOffId callOffId, SelectServicesModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(SelectViewName, model);
            }

            var order = (await orderService.GetOrderWithOrderItems(callOffId, internalOrgId)).Order;

            var existingServiceIds = order.GetAssociatedServices()
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
                    nameof(PricesController.SelectPrice),
                    typeof(PricesController).ControllerName(),
                    new { internalOrgId, callOffId, catalogueItemId = newServiceIds.First() });
            }

            var serviceIds = string.Join(Separator, selectedServiceIds);

            return RedirectToAction(
                nameof(ConfirmAssociatedServiceChanges),
                typeof(AssociatedServicesController).ControllerName(),
                new { internalOrgId, callOffId, serviceIds });
        }

        [HttpGet("confirm-changes")]
        public async Task<IActionResult> ConfirmAssociatedServiceChanges(string internalOrgId, CallOffId callOffId, string serviceIds)
        {
            var order = (await orderService.GetOrderWithOrderItems(callOffId, internalOrgId)).Order;
            var associatedServices = await associatedServicesService.GetPublishedAssociatedServicesForSupplier(order.SupplierId);

            var existingServiceIds = order.GetAssociatedServices()
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
                    Description = associatedServices.FirstOrDefault(s => s.Id == x)?.Name,
                });

            var toRemove = existingServiceIds
                .Where(x => !selectedServiceIds.Contains(x))
                .Select(x => new ServiceModel
                {
                    CatalogueItemId = x,
                    Description = associatedServices.FirstOrDefault(s => s.Id == x)?.Name,
                });

            var model = new ConfirmServiceChangesModel(internalOrgId, CatalogueItemType.AssociatedService)
            {
                BackLink = Url.Action(
                    nameof(EditAssociatedServices),
                    typeof(AssociatedServicesController).ControllerName(),
                    new { internalOrgId, callOffId }),
                ToAdd = toAdd.ToList(),
                ToRemove = toRemove.ToList(),
            };

            return View("Services/ConfirmChanges", model);
        }

        [HttpPost("confirm-changes")]
        public async Task<IActionResult> ConfirmAssociatedServiceChanges(string internalOrgId, CallOffId callOffId, ConfirmServiceChangesModel model)
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

            var removingServices = model.ToRemove?.Any() ?? false;
            var addingServices = model.ToAdd?.Any() ?? false;

            if (removingServices)
            {
                var orderId = await orderService.GetOrderId(internalOrgId, callOffId);
                await contractBillingService.DeleteContractBillingItems(orderId, model.ToRemove.Select(x => x.CatalogueItemId));
                await requirementsService.DeleteRequirements(orderId, model.ToRemove.Select(x => x.CatalogueItemId));
                await orderItemService.DeleteOrderItems(internalOrgId, callOffId, model.ToRemove.Select(x => x.CatalogueItemId));
            }

            if (!addingServices)
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

        private async Task AddOrderItems(string internalOrgId, CallOffId callOffId, List<CatalogueItemId> serviceIds)
        {
            if (serviceIds.Any())
            {
                await orderItemService.AddOrderItems(internalOrgId, callOffId, serviceIds);
            }
        }

        private IActionResult RedirectToPriceOrTaskList(string internalOrgId, CallOffId callOffId, CatalogueItemId? catalogueItemId)
        {
            if (catalogueItemId.HasValue)
            {
                return RedirectToAction(
                    nameof(PricesController.SelectPrice),
                    typeof(PricesController).ControllerName(),
                    new { internalOrgId, callOffId, catalogueItemId });
            }
            else
            {
                return RedirectToAction(
                    nameof(TaskListController.TaskList),
                    typeof(TaskListController).ControllerName(),
                    new { internalOrgId, callOffId });
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
                SolutionName = order.OrderType.AssociatedServicesOnly
                    ? wrapper.RolledUp.AssociatedServicesOnlyDetails.Solution.Name
                    : wrapper.RolledUp.GetSolutionOrderItem()?.CatalogueItem.Name,
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
                RoutingSource.AddAssociatedServices => Url.Action(
                    nameof(AddAssociatedServices),
                    typeof(AssociatedServicesController).ControllerName(),
                    new
                    {
                        internalOrgId,
                        callOffId,
                        selected = true,
                    }),

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
