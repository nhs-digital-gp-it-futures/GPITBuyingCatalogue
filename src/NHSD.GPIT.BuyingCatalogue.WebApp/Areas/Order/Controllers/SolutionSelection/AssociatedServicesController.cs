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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.UI.Components.TagHelpers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection
{
    [Authorize("Buyer")]
    [Area("Order")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/associated-services")]
    public class AssociatedServicesController : Controller
    {
        private const string SelectViewName = "SelectAssociatedServices";
        private const char Separator = ',';

        private readonly IAssociatedServicesService associatedServicesService;
        private readonly IOrderItemService orderItemService;
        private readonly IOrderService orderService;

        public AssociatedServicesController(
            IAssociatedServicesService associatedServicesService,
            IOrderItemService orderItemService,
            IOrderService orderService)
        {
            this.associatedServicesService = associatedServicesService ?? throw new ArgumentNullException(nameof(associatedServicesService));
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }

        [HttpGet("add")]
        public IActionResult AddAssociatedServices(string internalOrgId, CallOffId callOffId)
        {
            return View(new AddAssociatedServicesModel
            {
                BackLink = Url.Action(
                    nameof(OrderController.Order),
                    typeof(OrderController).ControllerName(),
                    new { internalOrgId, callOffId }),
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
            });
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
                    new { internalOrgId, callOffId });
            }

            return RedirectToAction(
                nameof(ReviewSolutionsController.ReviewSolutions),
                typeof(ReviewSolutionsController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("select")]
        public async Task<IActionResult> SelectAssociatedServices(string internalOrgId, CallOffId callOffId)
        {
            return View(await GetModel(internalOrgId, callOffId));
        }

        [HttpPost("select")]
        public async Task<IActionResult> SelectAssociatedServices(string internalOrgId, CallOffId callOffId, SelectServicesModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(await GetModel(internalOrgId, callOffId));
            }

            var serviceIds = model.Services?
                .Where(x => x.IsSelected)
                .Select(x => x.CatalogueItemId)
                .ToList();

            if (serviceIds?.Any() ?? false)
            {
                await orderItemService.AddOrderItems(internalOrgId, callOffId, serviceIds);

                var catalogueItemId = serviceIds.First();

                return RedirectToAction(
                    nameof(ServiceRecipientsController.AddServiceRecipients),
                    typeof(ServiceRecipientsController).ControllerName(),
                    new { internalOrgId, callOffId, catalogueItemId });
            }

            return RedirectToAction(
                nameof(ReviewSolutionsController.ReviewSolutions),
                typeof(ReviewSolutionsController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("edit")]
        public async Task<IActionResult> EditAssociatedServices(string internalOrgId, CallOffId callOffId)
        {
            return View(SelectViewName, await GetModel(internalOrgId, callOffId, returnToTaskList: true));
        }

        [HttpPost("edit")]
        public async Task<IActionResult> EditAssociatedServices(string internalOrgId, CallOffId callOffId, SelectServicesModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(SelectViewName, await GetModel(internalOrgId, callOffId, returnToTaskList: true));
            }

            var order = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);

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
                    nameof(ServiceRecipientsController.AddServiceRecipients),
                    typeof(ServiceRecipientsController).ControllerName(),
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
            var order = await orderService.GetOrderWithOrderItems(callOffId, internalOrgId);
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

            var model = new ConfirmServiceChangesModel(internalOrgId, callOffId, CatalogueItemType.AssociatedService)
            {
                BackLink = Url.Action(
                    nameof(EditAssociatedServices),
                    typeof(AssociatedServicesController).ControllerName(),
                    new { internalOrgId, callOffId }),
                ToAdd = toAdd.ToList(),
                ToRemove = toRemove.ToList(),
            };

            return View("ConfirmChanges", model);
        }

        [HttpPost("confirm-changes")]
        public async Task<IActionResult> ConfirmAssociatedServiceChanges(string internalOrgId, CallOffId callOffId, ConfirmServiceChangesModel model)
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
            var associatedServices = await associatedServicesService.GetPublishedAssociatedServicesForSupplier(order.SupplierId);

            var backLink = returnToTaskList
                ? Url.Action(nameof(TaskListController.TaskList), typeof(TaskListController).ControllerName(), new { internalOrgId, callOffId })
                : Url.Action(nameof(OrderController.Order), typeof(OrderController).ControllerName(), new { internalOrgId, callOffId });

            return new SelectServicesModel(order, associatedServices, CatalogueItemType.AssociatedService)
            {
                BackLink = backLink,
                InternalOrgId = internalOrgId,
                CallOffId = callOffId,
                AssociatedServicesOnly = order.AssociatedServicesOnly,
            };
        }
    }
}
