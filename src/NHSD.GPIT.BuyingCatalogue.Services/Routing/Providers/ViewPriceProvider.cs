using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers
{
    public class ViewPriceProvider : IRoutingResultProvider
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

            var attentionRequired = orderItem != null
                && (!orderItem.AllQuantitiesEntered || (order.IsAmendment && !orderItem.AllDeliveryDatesEntered));

            if (!attentionRequired)
            {
                return new RoutingResult
                {
                    ActionName = Constants.Actions.TaskList,
                    ControllerName = Constants.Controllers.TaskList,
                    RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId },
                };
            }

            return new RoutingResult
            {
                ActionName = Constants.Actions.SelectQuantity,
                ControllerName = Constants.Controllers.Quantity,
                RouteValues = new
                {
                    routeValues.InternalOrgId,
                    routeValues.CallOffId,
                    routeValues.CatalogueItemId,
                    routeValues.Source,
                },
            };
        }
    }
}
