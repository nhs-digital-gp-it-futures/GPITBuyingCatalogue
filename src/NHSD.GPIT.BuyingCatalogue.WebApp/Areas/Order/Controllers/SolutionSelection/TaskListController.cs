﻿using System;
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
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection
{
    [Authorize("Buyer")]
    [Area("Order")]
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
            var order = await orderService.GetOrderWithCatalogueItemAndPrices(callOffId, internalOrgId);

            var route = routingService.GetRoute(
                RoutingPoint.TaskListBackLink,
                order,
                new RouteValues(internalOrgId, callOffId) { Source = source });

            var solutionId = order.GetSolutionId();

            var additionalServices = order.AssociatedServicesOnly
                ? new List<CatalogueItem>()
                : await additionalServicesService.GetAdditionalServicesBySolutionId(solutionId, publishedOnly: true);

            var associatedServices = await associatedServicesService.GetPublishedAssociatedServicesForSolution(solutionId);

            return View(new TaskListModel(internalOrgId, callOffId, order)
            {
                BackLink = Url.Action(route.ActionName, route.ControllerName, route.RouteValues),
                AdditionalServicesAvailable = additionalServices.Any(),
                AssociatedServicesAvailable = associatedServices.Any(),
            });
        }
    }
}
