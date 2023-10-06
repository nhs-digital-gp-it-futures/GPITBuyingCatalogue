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
            ArgumentNullException.ThrowIfNull(orderWrapper.Order);
            
            if (routeValues == null)
            {
                throw new ArgumentNullException(nameof(routeValues));
            }

            return new RoutingResult
            {
                ActionName = Constants.Actions.OrderDashboard,
                ControllerName = Constants.Controllers.Orders,
                RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId },
            };
        }
    }
}
