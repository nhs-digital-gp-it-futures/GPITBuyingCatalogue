using System;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers
{
    public class ConfirmPriceProvider : IRoutingResultProvider
    {
        public RoutingResult Process(OrderWrapper orderWrapper, RouteValues routeValues)
        {
            ArgumentNullException.ThrowIfNull(orderWrapper);

            if (routeValues?.CatalogueItemId == null)
            {
                throw new ArgumentNullException(nameof(routeValues));
            }

            if (routeValues.Source == RoutingSource.ManageAssociatedServices)
            {
                var associatedService = orderWrapper.Order.OrderItem(routeValues.OrderItemId.GetValueOrDefault());
                var catalogueItemId = associatedService.Parent.CatalogueItemId;
                return new RoutingResult
                {
                    ActionName = Constants.Actions.ManageAssociatedServices,
                    ControllerName = Constants.Controllers.AssociatedServices,
                    RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId, catalogueItemId },
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
