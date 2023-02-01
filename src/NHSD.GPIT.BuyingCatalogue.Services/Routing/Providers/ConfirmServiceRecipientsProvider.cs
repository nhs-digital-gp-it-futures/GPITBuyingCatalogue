using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers
{
    public class ConfirmServiceRecipientsProvider : IRoutingResultProvider
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

            if (routeValues.Source == RoutingSource.TaskList)
            {
                return new RoutingResult
                {
                    ActionName = Constants.Actions.TaskList,
                    ControllerName = Constants.Controllers.TaskList,
                    RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId },
                };
            }

            var orderItem = order.OrderItem(routeValues.CatalogueItemId.Value);

            if (orderItem.OrderItemPrice != null)
            {
                return new RoutingResult
                {
                    ActionName = Constants.Actions.EditPrice,
                    ControllerName = Constants.Controllers.Prices,
                    RouteValues = new
                    {
                        routeValues.InternalOrgId,
                        routeValues.CallOffId,
                        routeValues.CatalogueItemId,
                    },
                };
            }

            var publishedPrices = orderItem.CatalogueItem.CataloguePrices
                .Where(x => x.PublishedStatus == PublicationStatus.Published)
                .ToList();

            if (publishedPrices.Count > 1)
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
                    },
                };
            }

            return new RoutingResult
            {
                ActionName = Constants.Actions.ConfirmPrice,
                ControllerName = Constants.Controllers.Prices,
                RouteValues = new
                {
                    routeValues.InternalOrgId,
                    routeValues.CallOffId,
                    routeValues.CatalogueItemId,
                    priceId = publishedPrices[0].CataloguePriceId,
                },
            };
        }
    }
}
