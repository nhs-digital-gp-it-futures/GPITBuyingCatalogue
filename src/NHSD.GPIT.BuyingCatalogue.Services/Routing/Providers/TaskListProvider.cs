using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers
{
    public class TaskListProvider : IRoutingResultProvider
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

            var isAmendment = routeValues.CallOffId.IsAmendment;

            if (!order.AssociatedServicesOnly)
            {
                if (order.GetSolution() == null
                    || order.OrderItems.Any(x => !x.IsReadyForReview(isAmendment)))
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
                if (order.GetAssociatedServices().Any(x => !x.IsReadyForReview(isAmendment)))
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
