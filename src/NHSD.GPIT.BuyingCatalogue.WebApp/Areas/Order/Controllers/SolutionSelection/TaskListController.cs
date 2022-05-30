using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
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
        private readonly IOrderService orderService;
        private readonly IRoutingService routingService;

        public TaskListController(IOrderService orderService, IRoutingService routingService)
        {
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

            return View(new TaskListModel(internalOrgId, callOffId, order)
            {
                BackLink = Url.Action(route.ActionName, route.ControllerName, route.RouteValues),
            });
        }
    }
}
