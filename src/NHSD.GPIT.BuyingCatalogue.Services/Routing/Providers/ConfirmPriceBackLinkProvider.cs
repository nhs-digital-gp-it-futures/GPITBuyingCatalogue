using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers
{
    public class ConfirmPriceBackLinkProvider : IRoutingResultProvider
    {
        public RoutingResult Process(Order order, RouteValues routeValues)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            if (routeValues?.CatalogueItemId == null)
            {
                throw new ArgumentNullException(nameof(routeValues));
            }

            var publishedPriceCount = order.OrderItem(routeValues.CatalogueItemId.Value)
                .CatalogueItem
                .CataloguePrices
                .Count(x => x.PublishedStatus == PublicationStatus.Published);

            if (publishedPriceCount > 1)
            {
                return new RoutingResult
                {
                    ActionName = Constants.Actions.SelectPrice,
                    ControllerName = Constants.Controllers.Prices,
                    RouteValues = new
                    {
                        routeValues.InternalOrgId,
                        routeValues.CallOffId,
                        routeValues.CatalogueItemId,
                        routeValues.SelectedPriceId,
                        routeValues.Source,
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
