using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers
{
    public class TaskListBackLinkProvider : IRoutingResultProvider
    {
        public RoutingResult Process(Order order, RouteValues routeValues)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            if (routeValues == null)
            {
                throw new ArgumentNullException(nameof(routeValues));
            }

            if (routeValues.Source == RoutingSource.Dashboard)
            {
                return new RoutingResult
                {
                    ActionName = Constants.Actions.OrderDashboard,
                    ControllerName = Constants.Controllers.Orders,
                    RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId },
                };
            }

            if (!order.AssociatedServicesOnly)
            {
                var solution = order.GetSolution();

                if (solution == null
                    || !(solution.OrderItemRecipients?.Any() ?? false)
                    || solution.OrderItemPrice == null)
                {
                    return new RoutingResult
                    {
                        ActionName = Constants.Actions.OrderDashboard,
                        ControllerName = Constants.Controllers.Orders,
                        RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId },
                    };
                }
            }
            else
            {
                if (order.GetAssociatedServices().Any(x => !(x.OrderItemRecipients?.Any() ?? false) || x.OrderItemPrice == null))
                {
                    return new RoutingResult
                    {
                        ActionName = Constants.Actions.OrderDashboard,
                        ControllerName = Constants.Controllers.Orders,
                        RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId },
                    };
                }
            }

            return new RoutingResult
            {
                ActionName = Constants.Actions.Review,
                ControllerName = Constants.Controllers.Review,
                RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId },
            };
        }
    }
}
