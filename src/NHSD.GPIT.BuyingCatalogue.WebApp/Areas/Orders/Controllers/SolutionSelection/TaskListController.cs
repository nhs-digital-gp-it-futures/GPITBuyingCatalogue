using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection
{
    [Authorize("Buyer")]
    [Area("Orders")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/task-list")]
    public class TaskListController : Controller
    {
        private readonly ISolutionsService solutionsService;
        private readonly IAdditionalServicesService additionalServicesService;
        private readonly IAssociatedServicesService associatedServicesService;
        private readonly IOrderService orderService;
        private readonly IRoutingService routingService;

        public TaskListController(
            ISolutionsService solutionsService,
            IAdditionalServicesService additionalServicesService,
            IAssociatedServicesService associatedServicesService,
            IOrderService orderService,
            IRoutingService routingService)
        {
            this.solutionsService = solutionsService ?? throw new ArgumentNullException(nameof(solutionsService));
            this.additionalServicesService = additionalServicesService ?? throw new ArgumentNullException(nameof(additionalServicesService));
            this.associatedServicesService = associatedServicesService ?? throw new ArgumentNullException(nameof(associatedServicesService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.routingService = routingService ?? throw new ArgumentNullException(nameof(routingService));
        }

        [HttpGet]
        public async Task<IActionResult> TaskList(string internalOrgId, CallOffId callOffId, RoutingSource? source = null)
        {
            var wrapper = await orderService.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId);

            var order = wrapper.Order;

            var solutions = order.OrderType.AssociatedServicesOnly
                ? await solutionsService.GetSupplierSolutionsWithAssociatedServices(order.SupplierId, order.OrderType.ToPracticeReorganisationType, order.SelectedFrameworkId)
                : await solutionsService.GetSupplierSolutions(order.SupplierId, order.SelectedFrameworkId);

            var backRoute = routingService.GetRoute(
                RoutingPoint.TaskListBackLink,
                wrapper,
                new RouteValues(internalOrgId, callOffId) { Source = source });

            var onwardRoute = routingService.GetRoute(
                RoutingPoint.TaskList,
                wrapper,
                new RouteValues(internalOrgId, callOffId) { Source = source });

            var solutionId = order.GetSolutionId();

            var additionalServices = order.OrderType.AssociatedServicesOnly
                ? new List<CatalogueItem>()
                : await additionalServicesService.GetAdditionalServicesBySolutionId(solutionId, publishedOnly: true);

            var selectedAdditionalServices = order.GetAdditionalServices().ToList();

            var associatedServicesForSolution = await associatedServicesService.GetPublishedAssociatedServicesForCatalogueItem(solutionId, order.OrderType.ToPracticeReorganisationType);
            var selectedAdditionalServiceIds = selectedAdditionalServices.Select(x => x.CatalogueItemId).ToList();
            var associatedServicesCountGroupedByAdditionalService =
                await associatedServicesService.GetCountOfAssociatedServicesForCatalogueItems(
                    selectedAdditionalServiceIds.ToHashSet());

            var selectedAssociatedServices = order.GetAssociatedServices();

            var model = new TaskListModel(internalOrgId, callOffId, wrapper, associatedServicesCountGroupedByAdditionalService)
            {
                BackLink = Url.Action(backRoute.ActionName, backRoute.ControllerName, backRoute.RouteValues),
                OnwardLink = Url.Action(onwardRoute.ActionName, onwardRoute.ControllerName, onwardRoute.RouteValues),
                AlternativeSolutionsAvailable = solutions.Count > 1,
                AdditionalServicesAvailable = additionalServices.Any(),
                UnselectedAdditionalServicesAvailable = additionalServices.Any(x => selectedAdditionalServices.All(y => x.Id != y.CatalogueItemId)),
                AssociatedServicesAvailable = associatedServicesForSolution.Any(),
                UnselectedAssociatedServicesAvailable = associatedServicesForSolution.Any(x => selectedAssociatedServices.All(y => x.Id != y.CatalogueItemId)),
            };

            return View(model);
        }
    }
}
