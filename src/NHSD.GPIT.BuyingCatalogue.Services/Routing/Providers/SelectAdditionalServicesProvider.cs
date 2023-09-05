using System;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers
{
    public class SelectAdditionalServicesProvider : IRoutingResultProvider
    {
        public RoutingResult Process(OrderWrapper orderWrapper, RouteValues routeValues)
        {
            ArgumentNullException.ThrowIfNull(orderWrapper);

            if (routeValues == null)
            {
                throw new ArgumentNullException(nameof(routeValues));
            }

            return new RoutingResult
            {
                ControllerName = Constants.Controllers.TaskList,
                ActionName = Constants.Actions.TaskList,
                RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId },
            };
        }
    }
}
