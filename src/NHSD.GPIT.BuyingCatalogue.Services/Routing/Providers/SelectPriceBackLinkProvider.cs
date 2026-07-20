using System;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers
{
    public class SelectPriceBackLinkProvider : IRoutingResultProvider
    {
        public RoutingResult Process(OrderWrapper orderWrapper, RouteValues routeValues)
        {
            ArgumentNullException.ThrowIfNull(orderWrapper);
            ArgumentNullException.ThrowIfNull(routeValues);

            if (routeValues.Source == RoutingSource.ManageAssociatedServices)
            {
                var associatedService = orderWrapper.Order.OrderItem(routeValues.OrderItemId!.Value);
                var catalogueItemId = associatedService.Parent.CatalogueItemId;
                return new RoutingResult
                {
                    ActionName = Constants.Actions.ManageAssociatedServices,
                    ControllerName = Constants.Controllers.AssociatedServices,
                    RouteValues = new
                    {
                        routeValues.InternalOrgId, routeValues.CallOffId, catalogueItemId,
                    },
                };
            }

            return new RoutingResult
            {
                ActionName = Constants.Actions.TaskList,
                ControllerName = Constants.Controllers.TaskList,
                RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId },
            };
        }
    }
}
