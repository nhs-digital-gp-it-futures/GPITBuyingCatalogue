using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers
{
    public class SelectAdditionalServicesProvider : IRoutingResultProvider
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

            var solution = order.GetSolution();

            if (!solution.OrderItemRecipients.Any())
            {
                return new RoutingResult
                {
                    ControllerName = Constants.Controllers.ServiceRecipients,
                    ActionName = Constants.Actions.AddServiceRecipients,
                    RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId, solution.CatalogueItemId },
                };
            }

            var additionalService = order.GetAdditionalServices().FirstOrDefault(x => x.OrderItemRecipients.Count == 0);

            if (additionalService != null)
            {
                return new RoutingResult
                {
                    ControllerName = Constants.Controllers.ServiceRecipients,
                    ActionName = Constants.Actions.AddServiceRecipients,
                    RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId, additionalService.CatalogueItemId },
                };
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
