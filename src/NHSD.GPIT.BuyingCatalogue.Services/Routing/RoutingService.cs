using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers;

namespace NHSD.GPIT.BuyingCatalogue.Services.Routing
{
    public class RoutingService : IRoutingService
    {
        private readonly Dictionary<RoutingPoint, IRoutingResultProvider> providers = new();

        public RoutingService()
        {
            providers.Add(RoutingPoint.ConfirmPriceBackLink, new ConfirmPriceBackLinkProvider());
            providers.Add(RoutingPoint.EditPrice, new EditPriceProvider());
            providers.Add(RoutingPoint.EditPriceBackLink, new EditPriceBackLinkProvider());
            providers.Add(RoutingPoint.SelectPriceBackLink, new SelectPriceBackLinkProvider());
            providers.Add(RoutingPoint.SelectQuantity, new SelectQuantityProvider());
            providers.Add(RoutingPoint.SelectQuantityBackLink, new SelectQuantityBackLinkProvider());
            providers.Add(RoutingPoint.SelectServiceRecipients, new SelectServiceRecipientsProvider());
            providers.Add(RoutingPoint.SelectServiceRecipientsBackLink, new SelectServiceRecipientsBackLinkProvider());
            providers.Add(RoutingPoint.TaskListBackLink, new TaskListBackLinkProvider());
        }

        public RoutingResult GetRoute(RoutingPoint point, Order order, RouteValues routeValues)
        {
            return GetProvider(point).Process(order, routeValues);
        }

        private IRoutingResultProvider GetProvider(RoutingPoint point)
        {
            return providers.ContainsKey(point)
                ? providers[point]
                : throw new ArgumentOutOfRangeException(nameof(point), point, null);
        }
    }
}
