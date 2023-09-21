using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers
{
    public class AmendDeliveryDatesBackLinkProvider : IRoutingResultProvider
    {
        public RoutingResult Process(OrderWrapper orderWrapper, RouteValues routeValues)
        {
            if (routeValues == null)
            {
                throw new ArgumentNullException(nameof(routeValues));
            }

            if (routeValues.Source == RoutingSource.TaskList)
            {
                return new RoutingResult
                {
                    ActionName = Constants.Actions.TaskList,
                    ControllerName = Constants.Controllers.TaskList,
                    RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId },
                };
            }

            return new RoutingResult
            {
                ActionName = Constants.Actions.SelectQuantity,
                ControllerName = Constants.Controllers.Quantity,
                RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId, routeValues.CatalogueItemId },
            };
        }
    }
}
