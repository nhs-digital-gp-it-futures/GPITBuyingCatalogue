using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
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

        public RoutingResult Process(OrderWrapper orderWrapper, RouteValues routeValues)
        {
            ArgumentNullException.ThrowIfNull(orderWrapper);
            var order = orderWrapper.Order ?? throw new ArgumentNullException(nameof(orderWrapper));

            if (routeValues?.CatalogueItemId == null)
            {
                throw new ArgumentNullException(nameof(routeValues));
            }

            var orderItem = order.OrderItem(routeValues.CatalogueItemId.Value);
            var attentionRequired = order.IsAmendment
                && !orderWrapper.DetermineOrderRecipients(orderItem.CatalogueItemId)
                                .AllDeliveryDatesEntered(orderItem.CatalogueItemId);

            if (routeValues.Source == RoutingSource.TaskList
                && !attentionRequired)
            {
                return new RoutingResult
                {
                    ControllerName = Constants.Controllers.TaskList,
                    ActionName = Constants.Actions.TaskList,
                    RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId },
                };
            }

            var additionalService = order.GetAdditionalServices()
                .FirstOrDefault(x => x.OrderItemPrice is null);

            if (additionalService != null)
            {
                return PricesRoute(routeValues, additionalService);
            }

            var associatedService = order.GetAssociatedServices()
                .FirstOrDefault(x => x.OrderItemPrice is null);

            if (associatedService != null)
            {
                return PricesRoute(routeValues, associatedService);
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
                ControllerName = Constants.Controllers.TaskList,
                ActionName = Constants.Actions.TaskList,
                RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId },
            };
        }

        private static RoutingResult PricesRoute(RouteValues routeValues, OrderItem service)
        {
            var publishedPrices = service.CatalogueItem.CataloguePrices
                .Where(x => x.PublishedStatus == PublicationStatus.Published)
                .ToList();

            if (publishedPrices.Count > 1)
            {
                return new RoutingResult
                {
                    ControllerName = Constants.Controllers.Prices,
                    ActionName = Constants.Actions.SelectPrice,
                    RouteValues = new
                    {
                        routeValues.InternalOrgId,
                        routeValues.CallOffId,
                        service.CatalogueItemId,
                        routeValues.Source,
                    },
                };
            }

            return new RoutingResult
            {
                ControllerName = Constants.Controllers.Prices,
                ActionName = Constants.Actions.ConfirmPrice,
                RouteValues = new
                {
                    routeValues.InternalOrgId,
                    routeValues.CallOffId,
                    service.CatalogueItemId,
                    priceId = publishedPrices[0].CataloguePriceId,
                    routeValues.Source,
                },
            };
        }
    }
}
