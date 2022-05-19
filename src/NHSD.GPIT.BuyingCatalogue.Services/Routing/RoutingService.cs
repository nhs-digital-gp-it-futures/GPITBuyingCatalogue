using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers;

namespace NHSD.GPIT.BuyingCatalogue.Services.Routing
{
    public class RoutingService : IRoutingService
    {
        private readonly Dictionary<RoutingSource, IRoutingResultProvider> providers = new();

        public RoutingService()
        {
            providers.Add(RoutingSource.ConfirmPriceBackLink, new ConfirmPriceBackLinkProvider());
            providers.Add(RoutingSource.SelectQuantity, new SelectQuantityProvider());
            providers.Add(RoutingSource.SelectServiceRecipients, new SelectServiceRecipientsProvider());
        }

        public RoutingResult GetRoute(RoutingSource source, Order order, RouteValues routeValues)
        {
            return GetProvider(source).Process(order, routeValues);
        }

        private IRoutingResultProvider GetProvider(RoutingSource source)
        {
            return providers.ContainsKey(source)
                ? providers[source]
                : throw new ArgumentOutOfRangeException(nameof(source), source, null);
        }
    }
}
