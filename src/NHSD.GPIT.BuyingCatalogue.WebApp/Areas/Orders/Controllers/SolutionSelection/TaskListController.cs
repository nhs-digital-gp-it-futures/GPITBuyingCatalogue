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
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.SolutionSelection.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection
{
    [Authorize("Buyer")]
    [Area("Orders")]
    [Route("order/organisation/{internalOrgId}/order/{callOffId}/task-list")]
    public class TaskListController : Controller
    {
        private readonly IAdditionalServicesService additionalServicesService;
        private readonly IAssociatedServicesService associatedServicesService;
        private readonly IOrderService orderService;
        private readonly IRoutingService routingService;

        public TaskListController(
            IAdditionalServicesService additionalServicesService,
            IAssociatedServicesService associatedServicesService,
            IOrderService orderService,
            IRoutingService routingService)
        {
            this.additionalServicesService = additionalServicesService ?? throw new ArgumentNullException(nameof(additionalServicesService));
            this.associatedServicesService = associatedServicesService ?? throw new ArgumentNullException(nameof(associatedServicesService));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.routingService = routingService ?? throw new ArgumentNullException(nameof(routingService));
        }

        [HttpGet]
        public async Task<IActionResult> TaskList(string internalOrgId, CallOffId callOffId, RoutingSource? source = null)
        {
            var wrapper = await orderService.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId);
            // TODO: MJK review????
            if (wrapper.IsAmendment)
            {
                await orderService.InitialiseAmendedOrderItemsIfRequired(internalOrgId, callOffId);
            }

            var order = wrapper.IsAmendment ? wrapper.RolledUp : wrapper.Order;

            var backRoute = routingService.GetRoute(
                RoutingPoint.TaskListBackLink,
                wrapper,
                new RouteValues(internalOrgId, callOffId) { Source = source });

            var onwardRoute = routingService.GetRoute(
                RoutingPoint.TaskList,
                wrapper,
                new RouteValues(internalOrgId, callOffId) { Source = source });

            var solutionId = order.GetSolutionId();

            var additionalServices = order.AssociatedServicesOnly
                ? new List<CatalogueItem>()
                : await additionalServicesService.GetAdditionalServicesBySolutionId(solutionId, publishedOnly: true);

            var associatedServices = await associatedServicesService.GetPublishedAssociatedServicesForSolution(solutionId);

            return View(new TaskListModel(internalOrgId, callOffId, wrapper)
            {
                BackLink = Url.Action(backRoute.ActionName, backRoute.ControllerName, backRoute.RouteValues),
                OnwardLink = Url.Action(onwardRoute.ActionName, onwardRoute.ControllerName, onwardRoute.RouteValues),
                AdditionalServicesAvailable = additionalServices.Any(),
                AssociatedServicesAvailable = associatedServices.Any(),
            });
        }
    }
}
