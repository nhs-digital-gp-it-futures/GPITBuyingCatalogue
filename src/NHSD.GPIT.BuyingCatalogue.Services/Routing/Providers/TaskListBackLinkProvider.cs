using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers
{
    public class TaskListBackLinkProvider : IRoutingResultProvider
    {
        public RoutingResult Process(OrderWrapper orderWrapper, RouteValues routeValues)
        {
            ArgumentNullException.ThrowIfNull(orderWrapper);

            var order = orderWrapper.Order ?? throw new ArgumentNullException(nameof(orderWrapper));

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

            var isAmendment = routeValues.CallOffId.IsAmendment;

            if (!order.AssociatedServicesOnly)
            {
                if (order.GetSolution() == null
                    || order.OrderItems.Any(x => !x.IsReadyForReview(isAmendment, orderWrapper.DetermineOrderRecipients(x.CatalogueItemId))))
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
                if (order.GetAssociatedServices().Any(x => !x.IsReadyForReview(isAmendment, orderWrapper.DetermineOrderRecipients(x.CatalogueItemId))))
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
