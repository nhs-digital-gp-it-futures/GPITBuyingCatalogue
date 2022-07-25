using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers
{
    public class SelectQuantityProvider : IRoutingResultProvider
    {
        private readonly IAssociatedServicesService associatedServicesService;

        public SelectQuantityProvider(IAssociatedServicesService associatedServicesService)
        {
            this.associatedServicesService = associatedServicesService ?? throw new ArgumentNullException(nameof(associatedServicesService));
        }

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

            if (routeValues.Source == RoutingSource.TaskList)
            {
                return new RoutingResult
                {
                    ControllerName = Constants.Controllers.TaskList,
                    ActionName = Constants.Actions.TaskList,
                    RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId },
                };
            }

            var additionalService = order.GetAdditionalServices()
                .FirstOrDefault(x => (x.OrderItemRecipients?.Count ?? 0) == 0);

            if (additionalService != null)
            {
                return new RoutingResult
                {
                    ControllerName = Constants.Controllers.ServiceRecipients,
                    ActionName = Constants.Actions.AddServiceRecipients,
                    RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId, additionalService.CatalogueItemId },
                };
            }

            var associatedService = order.GetAssociatedServices()
                .FirstOrDefault(x => (x.OrderItemRecipients?.Count ?? 0) == 0);

            if (associatedService != null)
            {
                return new RoutingResult
                {
                    ControllerName = Constants.Controllers.ServiceRecipients,
                    ActionName = Constants.Actions.AddServiceRecipients,
                    RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId, associatedService.CatalogueItemId },
                };
            }

            if (order.GetAssociatedServices().Any())
            {
                return new RoutingResult
                {
                    ControllerName = Constants.Controllers.Review,
                    ActionName = Constants.Actions.Review,
                    RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId },
                };
            }

            var associatedServices = associatedServicesService.GetPublishedAssociatedServicesForSolution(order.GetSolutionId()).Result;

            if (associatedServices.Any())
            {
                return new RoutingResult
                {
                    ControllerName = Constants.Controllers.AssociatedServices,
                    ActionName = Constants.Actions.AddAssociatedServices,
                    RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId },
                };
            }

            return new RoutingResult
            {
                ControllerName = Constants.Controllers.Review,
                ActionName = Constants.Actions.Review,
                RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId },
            };
        }
    }
}
