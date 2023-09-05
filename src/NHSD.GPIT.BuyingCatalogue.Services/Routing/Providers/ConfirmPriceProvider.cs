using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers
{
    public class ConfirmPriceProvider : IRoutingResultProvider
    {
        public RoutingResult Process(OrderWrapper orderWrapper, RouteValues routeValues)
        {
            ArgumentNullException.ThrowIfNull(orderWrapper);
            var order = orderWrapper.Order ?? throw new ArgumentNullException(nameof(orderWrapper));

            if (routeValues?.CatalogueItemId == null)
            {
                throw new ArgumentNullException(nameof(routeValues));
            }

            var orderItem = order.OrderItem(routeValues.CatalogueItemId.Value);
            var recipientsForOrderItem = orderWrapper.DetermineOrderRecipients(orderItem.CatalogueItemId);

            var attentionRequired = !recipientsForOrderItem.AllQuantitiesEntered(orderItem)
                || (order.IsAmendment && !recipientsForOrderItem.AllDeliveryDatesEntered(orderItem.CatalogueItemId));

            if (routeValues.Source == RoutingSource.TaskList
                && !attentionRequired)
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
