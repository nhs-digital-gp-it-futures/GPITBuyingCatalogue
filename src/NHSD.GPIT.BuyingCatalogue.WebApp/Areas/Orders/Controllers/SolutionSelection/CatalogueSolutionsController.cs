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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Contracts;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.Shared;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Services;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection
{
    [Authorize("Buyer")]
    [Area("Orders")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/solutions")]
    public class CatalogueSolutionsController : Controller
    {
        private const string SelectViewName = "SelectSolution";

        private readonly IAdditionalServicesService additionalServicesService;
        private readonly IOrderItemService orderItemService;
        private readonly IOrderService orderService;
        private readonly ISolutionsService solutionsService;
        private readonly IContractsService contractsService;

        public CatalogueSolutionsController(
            IAdditionalServicesService additionalServicesService,
            IOrderItemService orderItemService,
            IOrderService orderService,
            ISolutionsService solutionsService,
            IContractsService contractsService)
        {
            this.additionalServicesService = additionalServicesService ?? throw new ArgumentNullException(nameof(additionalServicesService));
            this.orderItemService = orderItemService ?? throw new ArgumentNullException(nameof(orderItemService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.contractsService = contractsService ?? throw new ArgumentNullException(nameof(contractsService));
        }

        [HttpGet("select")]
        public async Task<IActionResult> SelectSolution(string internalOrgId, CallOffId callOffId)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;

            if (order.OrderType.AssociatedServicesOnly)
            {
                return RedirectToAction(
                    nameof(SelectSolutionAssociatedServicesOnly),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            var model = await GetSelectModel(internalOrgId, callOffId, includeAdditionalServices: true);

            return View(model);
        }

        [HttpPost("select")]
        public async Task<IActionResult> SelectSolution(string internalOrgId, CallOffId callOffId, SelectSolutionModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await GetSelectModel(internalOrgId, callOffId, model.SelectedCatalogueSolutionId, includeAdditionalServices: true);

                return View(model);
            }

            var catalogueItemId = CatalogueItemId.ParseExact(model.SelectedCatalogueSolutionId);
            var additionalServiceIds = model.GetAdditionalServicesIdsForSelectedCatalogueSolution();
            var ids = additionalServiceIds.Union(new[] { catalogueItemId });

            await orderItemService.AddOrderItems(internalOrgId, callOffId, ids);

            await orderService.SetSolutionId(internalOrgId, callOffId, catalogueItemId);

            var wrapper = await orderService.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId);
            var orderItem = wrapper.Order.OrderItem(catalogueItemId);

            var publishedPrices = orderItem.CatalogueItem.CataloguePrices
                .Where(x => x.PublishedStatus == PublicationStatus.Published)
                .ToList();

            return RedirectToAction(
                nameof(TaskListController.TaskList),
                typeof(TaskListController).ControllerName(),
                new { internalOrgId, callOffId });
        }

        [HttpGet("select/associated-services-only")]
        public async Task<IActionResult> SelectSolutionAssociatedServicesOnly(string internalOrgId, CallOffId callOffId, RoutingSource? source = null)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;

            if (order.AssociatedServicesOnlyDetails.SolutionId is not null
                && source != RoutingSource.SelectAssociatedServices)
            {
                return RedirectToAction(
                    nameof(EditSolutionAssociatedServicesOnly),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            var model = await GetSelectModel(internalOrgId, callOffId, includeAdditionalServices: false);

            return View(SelectViewName, model);
        }

        [HttpPost("select/associated-services-only")]
        public async Task<IActionResult> SelectSolutionAssociatedServicesOnly(string internalOrgId, CallOffId callOffId, SelectSolutionModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await GetSelectModel(internalOrgId, callOffId, model.SelectedCatalogueSolutionId, includeAdditionalServices: false);

                return View(SelectViewName, model);
            }

            var catalogueItemId = CatalogueItemId.ParseExact(model.SelectedCatalogueSolutionId);

            await orderService.SetSolutionId(internalOrgId, callOffId, catalogueItemId);

            return RedirectToAction(
                nameof(AssociatedServicesController.SelectAssociatedServices),
                typeof(AssociatedServicesController).ControllerName(),
                new { internalOrgId, callOffId, source = RoutingSource.SelectSolution });
        }

        [HttpGet("edit-solution-and-services")]
        public async Task<IActionResult> EditSolutionServices(string internalOrgId, CallOffId callOffId)
        {
            var order = (await orderService.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId)).Order;

            var solutionId = order.GetSolutionId();
            var solution = await solutionsService.GetSolutionThin(solutionId.GetValueOrDefault());

            var model = new EditSolutionServices()
            {
                CallOffId = callOffId,
                BackLink = Url.Action(
                    nameof(SelectSolution),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { internalOrgId, callOffId }),
                CatalogueItem = solution,
            };

            return View("Areas/Orders/Views/SolutionServices/EditSolutionServices.cshtml", model);
        }

        [HttpGet("edit")]
        public async Task<IActionResult> EditSolution(string internalOrgId, CallOffId callOffId)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;

            if (order.OrderType.AssociatedServicesOnly)
            {
                return RedirectToAction(
                    nameof(EditSolutionAssociatedServicesOnly),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            var model = await GetSelectModel(
                internalOrgId,
                callOffId,
                includeAdditionalServices: false,
                returnToTaskList: true);

            return View(SelectViewName, model);
        }

        [HttpPost("edit")]
        public async Task<IActionResult> EditSolution(string internalOrgId, CallOffId callOffId, SelectSolutionModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await GetSelectModel(
                    internalOrgId,
                    callOffId,
                    model.SelectedCatalogueSolutionId,
                    includeAdditionalServices: false,
                    returnToTaskList: true);

                return View(SelectViewName, model);
            }

            var order = (await orderService.GetOrderWithOrderItems(callOffId, internalOrgId)).Order;
            var solution = order.GetSolutionOrderItem();
            var catalogueItemId = CatalogueItemId.ParseExact(model.SelectedCatalogueSolutionId);

            if (solution.CatalogueItemId == catalogueItemId)
            {
                return RedirectToAction(
                    nameof(TaskListController.TaskList),
                    typeof(TaskListController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            return RedirectToAction(
                nameof(ConfirmSolutionChanges),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { internalOrgId, callOffId, catalogueItemId });
        }

        [HttpGet("edit/associated-services-only")]
        public async Task<IActionResult> EditSolutionAssociatedServicesOnly(string internalOrgId, CallOffId callOffId)
        {
            var order = (await orderService.GetOrderWithOrderItems(callOffId, internalOrgId)).Order;

            if (order.AssociatedServicesOnlyDetails.Solution is null)
            {
                return RedirectToAction(
                    nameof(SelectSolutionAssociatedServicesOnly),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            var model = await GetSelectModel(
                internalOrgId,
                callOffId,
                includeAdditionalServices: false,
                returnToTaskList: true);

            return View(SelectViewName, model);
        }

        [HttpPost("edit/associated-services-only")]
        public async Task<IActionResult> EditSolutionAssociatedServicesOnly(string internalOrgId, CallOffId callOffId, SelectSolutionModel model)
        {
            if (!ModelState.IsValid)
            {
                model = await GetSelectModel(
                    internalOrgId,
                    callOffId,
                    model.SelectedCatalogueSolutionId,
                    includeAdditionalServices: false,
                    returnToTaskList: true);

                return View(SelectViewName, model);
            }

            var catalogueItemId = CatalogueItemId.ParseExact(model.SelectedCatalogueSolutionId);
            var order = (await orderService.GetOrderWithOrderItems(callOffId, internalOrgId)).Order;

            if (order.AssociatedServicesOnlyDetails.Solution.Id == catalogueItemId)
            {
                return RedirectToAction(
                    nameof(TaskListController.TaskList),
                    typeof(TaskListController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            return RedirectToAction(
                nameof(ConfirmSolutionChangesAssociatedServicesOnly),
                typeof(CatalogueSolutionsController).ControllerName(),
                new { internalOrgId, callOffId, catalogueItemId });
        }

        [HttpGet("confirm-changes")]
        public async Task<IActionResult> ConfirmSolutionChanges(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            var order = (await orderService.GetOrderWithOrderItems(callOffId, internalOrgId)).Order;
            var solution = order.GetSolutionOrderItem();
            var newSolution = await solutionsService.GetSolutionThin(catalogueItemId);

            var toAdd = new[]
            {
                new ServiceModel
                {
                    CatalogueItemId = newSolution.Id,
                    Description = newSolution.Name,
                },
            };

            var toRemove = new List<ServiceModel>
            {
                new()
                {
                    CatalogueItemId = solution.CatalogueItemId,
                    Description = solution.CatalogueItem.Name,
                },
            };

            toRemove.AddRange(order.GetAdditionalServices().Select(x => new ServiceModel
            {
                CatalogueItemId = x.CatalogueItemId,
                Description = x.CatalogueItem.Name,
            }));

            toRemove.AddRange(order.GetAssociatedServices().Select(x => new ServiceModel
            {
                CatalogueItemId = x.CatalogueItemId,
                Description = x.CatalogueItem.Name,
            }));

            var model = new ConfirmServiceChangesModel(internalOrgId, CatalogueItemType.Solution)
            {
                BackLink = Url.Action(
                    nameof(EditSolution),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { internalOrgId, callOffId }),
                ToAdd = toAdd.ToList(),
                ToRemove = toRemove.ToList(),
                Caption = $"Order {callOffId}",
            };

            return View("Services/ConfirmChanges", model);
        }

        [HttpPost("confirm-changes")]
        public async Task<IActionResult> ConfirmSolutionChanges(string internalOrgId, CallOffId callOffId, ConfirmServiceChangesModel model)
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

            var orderId = await orderService.GetOrderId(internalOrgId, callOffId);

            await contractsService.RemoveContract(orderId);
            await orderItemService.DeleteOrderItems(internalOrgId, callOffId, model.ToRemove.Select(x => x.CatalogueItemId));
            await orderService.DeleteSelectedFramework(internalOrgId, callOffId);
            await orderItemService.AddOrderItems(internalOrgId, callOffId, model.ToAdd.Select(x => x.CatalogueItemId));

            var catalogueItemId = model.ToAdd.First().CatalogueItemId;
            var additionalServices = await additionalServicesService.GetAdditionalServicesBySolutionId(catalogueItemId, publishedOnly: true);

            if (additionalServices.Any())
            {
                return RedirectToAction(
                    nameof(AdditionalServicesController.SelectAdditionalServices),
                    typeof(AdditionalServicesController).ControllerName(),
                    new { internalOrgId, callOffId });
            }

            return RedirectToAction(
                nameof(PricesController.SelectPrice),
                typeof(PricesController).ControllerName(),
                new { internalOrgId, callOffId, catalogueItemId });
        }

        [HttpGet("confirm-changes/associated-services-only")]
        public async Task<IActionResult> ConfirmSolutionChangesAssociatedServicesOnly(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            var order = (await orderService.GetOrderWithOrderItems(callOffId, internalOrgId)).Order;
            var newSolution = await solutionsService.GetSolutionThin(catalogueItemId);

            var toAdd = new[]
            {
                new ServiceModel
                {
                    CatalogueItemId = newSolution.Id,
                    Description = newSolution.Name,
                },
            };

            var toRemove = new List<ServiceModel>
            {
                new()
                {
                    CatalogueItemId = order.AssociatedServicesOnlyDetails.Solution.Id,
                    Description = order.AssociatedServicesOnlyDetails.Solution.Name,
                },
            };

            toRemove.AddRange(order.GetAssociatedServices().Select(x => new ServiceModel
            {
                CatalogueItemId = x.CatalogueItemId,
                Description = x.CatalogueItem.Name,
            }));

            var model = new ConfirmServiceChangesModel(internalOrgId, CatalogueItemType.Solution)
            {
                BackLink = Url.Action(
                    nameof(EditSolutionAssociatedServicesOnly),
                    typeof(CatalogueSolutionsController).ControllerName(),
                    new { internalOrgId, callOffId }),
                ToAdd = toAdd.ToList(),
                ToRemove = toRemove.ToList(),
            };

            return View("Services/ConfirmChanges", model);
        }

        [HttpPost("confirm-changes/associated-services-only")]
        public async Task<IActionResult> ConfirmSolutionChangesAssociatedServicesOnly(string internalOrgId, CallOffId callOffId, ConfirmServiceChangesModel model)
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

            var orderId = await orderService.GetOrderId(internalOrgId, callOffId);

            await contractsService.RemoveContract(orderId);
            await orderItemService.DeleteOrderItems(internalOrgId, callOffId, model.ToRemove.Select(x => x.CatalogueItemId));
            await orderService.DeleteSelectedFramework(internalOrgId, callOffId);
            await orderService.SetSolutionId(internalOrgId, callOffId, model.ToAdd.First().CatalogueItemId);

            return RedirectToAction(
                nameof(AssociatedServicesController.SelectAssociatedServices),
                typeof(AssociatedServicesController).ControllerName(),
                new { internalOrgId, callOffId, source = RoutingSource.EditSolution });
        }

        [HttpGet("remove-service")]
        public async Task<IActionResult> RemoveService(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            var order = (await orderService.GetOrderWithOrderItems(callOffId, internalOrgId)).Order;
            var service = await solutionsService.GetSolutionThin(catalogueItemId);
            var model = new RemoveServiceModel(order, service)
            {
                BackLink = Url.Action(
                    nameof(TaskListController.TaskList),
                    typeof(TaskListController).ControllerName(),
                    new { internalOrgId, callOffId }),
            };

            return View(model);
        }

        [HttpPost("remove-service")]
        public async Task<IActionResult> RemoveService(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId, RemoveServiceModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.ConfirmRemoveService ?? false)
            {
                await orderItemService.DeleteOrderItems(internalOrgId, callOffId, new List<CatalogueItemId> { catalogueItemId });
            }

            return RedirectToAction(
                    nameof(TaskListController.TaskList),
                    typeof(TaskListController).ControllerName(),
                    new { internalOrgId, callOffId });
        }

        private async Task<SelectSolutionModel> GetSelectModel(
            string internalOrgId,
            CallOffId callOffId,
            string selected = null,
            bool includeAdditionalServices = true,
            bool returnToTaskList = false)
        {
            var order = (await orderService.GetOrderThin(callOffId, internalOrgId)).Order;
            var solutionId = order.GetSolutionId();

            var solutions = order.OrderType.AssociatedServicesOnly
                ? await solutionsService.GetSupplierSolutionsWithAssociatedServices(order.SupplierId, order.OrderType.ToPracticeReorganisationType)
                : await solutionsService.GetSupplierSolutions(order.SupplierId);

            var additionalServices = includeAdditionalServices
                ? await additionalServicesService.GetAdditionalServicesBySolutionIds(solutions.Select(x => x.Id))
                : new List<CatalogueItem>();

            var backLink = returnToTaskList
                ? Url.Action(nameof(TaskListController.TaskList), typeof(TaskListController).ControllerName(), new { internalOrgId, callOffId })
                : Url.Action(nameof(OrderController.Order), typeof(OrderController).ControllerName(), new { internalOrgId, callOffId });

            return new SelectSolutionModel(order, solutions, additionalServices)
            {
                BackLink = backLink,
                SelectedCatalogueSolutionId = selected ?? $"{solutionId}",
            };
        }
    }
}
