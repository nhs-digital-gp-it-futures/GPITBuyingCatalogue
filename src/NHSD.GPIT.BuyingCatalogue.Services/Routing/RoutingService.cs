using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers;

namespace NHSD.GPIT.BuyingCatalogue.Services.Routing
{
    [ExcludeFromCodeCoverage]
    public class RoutingService : IRoutingService
    {
        private readonly Dictionary<RoutingPoint, IRoutingResultProvider> providers = new();

        public RoutingService(IAssociatedServicesService associatedServicesService)
        {
            providers.Add(RoutingPoint.ConfirmPrice, new ConfirmPriceProvider());
            providers.Add(RoutingPoint.ConfirmPriceBackLink, new ConfirmPriceBackLinkProvider());
            providers.Add(RoutingPoint.EditDeliveryDates, new DeliveryDatesProvider());
            providers.Add(RoutingPoint.EditDeliveryDatesBackLink, new DeliveryDatesBackLinkProvider());
            providers.Add(RoutingPoint.EditPrice, new EditPriceProvider());
            providers.Add(RoutingPoint.EditPriceBackLink, new EditPriceBackLinkProvider());
            providers.Add(RoutingPoint.SelectAdditionalServices, new SelectAdditionalServicesProvider());
            providers.Add(RoutingPoint.SelectPriceBackLink, new SelectPriceBackLinkProvider());
            providers.Add(RoutingPoint.SelectQuantity, new SelectQuantityProvider(associatedServicesService));
            providers.Add(RoutingPoint.SelectQuantityBackLink, new SelectQuantityBackLinkProvider());
            providers.Add(RoutingPoint.TaskList, new TaskListProvider());
            providers.Add(RoutingPoint.TaskListBackLink, new TaskListBackLinkProvider());
        }

        public RoutingResult GetRoute(RoutingPoint point, OrderWrapper orderWrapper, RouteValues routeValues)
        {
            return GetProvider(point).Process(orderWrapper, routeValues);
        }

        private IRoutingResultProvider GetProvider(RoutingPoint point)
        {
            if (!providers.TryGetValue(point, out var routingProvider))
            {
                throw new ArgumentOutOfRangeException(nameof(point), point, null);
            }

            return routingProvider;
        }
    }
}
