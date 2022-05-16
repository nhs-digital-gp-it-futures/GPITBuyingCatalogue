using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers
{
    public class SelectQuantityProvider : IRoutingResultProvider
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

            var additionalService = order.GetAdditionalServices()
                .FirstOrDefault(x => x.OrderItemRecipients.Count == 0);

            if (additionalService != null)
            {
                return new RoutingResult
                {
                    ControllerName = Constants.Controllers.ServiceRecipients,
                    ActionName = Constants.Actions.SelectServiceRecipients,
                    RouteValues = new
                    {
                        routeValues.InternalOrgId,
                        routeValues.CallOffId,
                        additionalService.CatalogueItemId,
                    },
                };
            }

            // TODO: check for existing / catalogue associated services, task list otherwise
            return new RoutingResult
            {
                ControllerName = Constants.Controllers.AssociatedServices,
                ActionName = Constants.Actions.AddAssociatedServices,
                RouteValues = new
                {
                    routeValues.InternalOrgId,
                    routeValues.CallOffId,
                },
            };
        }
    }
}
