using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers
{
    public class EditPriceBackLinkProvider : IRoutingResultProvider
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

            var orderItem = order.OrderItem(routeValues.CatalogueItemId.Value);
            var numberOfPrices = orderItem?.CatalogueItem?.CataloguePrices?.Count ?? 0;

            if (numberOfPrices > 1)
            {
                return new RoutingResult
                {
                    ActionName = Constants.Actions.SelectPrice,
                    ControllerName = Constants.Controllers.Prices,
                    RouteValues = new
                    {
                        routeValues.CallOffId,
                        routeValues.InternalOrgId,
                        routeValues.CatalogueItemId,
                        selectedPriceId = orderItem?.OrderItemPrice?.CataloguePriceId,
                        routeValues.Source,
                    },
                };
            }

            return new RoutingResult
            {
                ActionName = Constants.Actions.TaskList,
                ControllerName = Constants.Controllers.TaskList,
                RouteValues = new { routeValues.CallOffId, routeValues.InternalOrgId },
            };
        }
    }
}
