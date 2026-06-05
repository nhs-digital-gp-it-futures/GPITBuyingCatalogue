using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers
{
    public class DeliveryDatesProvider : IRoutingResultProvider
    {
        public RoutingResult Process(OrderWrapper orderWrapper, RouteValues routeValues)
        {
            ArgumentNullException.ThrowIfNull(orderWrapper);
            var order = orderWrapper.Order ?? throw new ArgumentNullException(nameof(orderWrapper));

            if (routeValues?.CatalogueItemId == null)
            {
                throw new ArgumentNullException(nameof(routeValues));
            }

            if (routeValues.Source is RoutingSource.TaskList)
            {
                return new RoutingResult
                {
                    ActionName = Constants.Actions.ReviewDeliveryDates,
                    ControllerName = Constants.Controllers.DeliveryDates,
                    RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId },
                };
            }

            var orderItemId = order.GetNextOrderItemId(routeValues.OrderItemId!.Value);

            if (orderItemId == null)
            {
                return new RoutingResult
                {
                    ActionName = Constants.Actions.ReviewDeliveryDates,
                    ControllerName = Constants.Controllers.DeliveryDates,
                    RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId },
                };
            }

            var solution = order.GetSolutionOrderItem();
            var orderItem = order.OrderItem(orderItemId.Value);
            var item = IsParentAdditionalService(orderItem)
                ? orderItem.Parent
                : orderItem;

            if (order.OrderType.AssociatedServicesOnly
                || solution == null)
            {
                return new RoutingResult
                {
                    ActionName = Constants.Actions.EditDeliveryDates,
                    ControllerName = Constants.Controllers.DeliveryDates,
                    RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId, orderItemId },
                };
            }

            var solutionDates = orderWrapper.DetermineOrderRecipients(solution.CatalogueItemId)
                .Select(x => x.GetDeliveryDateForItem(solution.CatalogueItemId))
                .Distinct()
                .ToList();

            IEnumerable<string> solutionOdsCodes = orderWrapper.DetermineOrderRecipients(solution.CatalogueItemId)
                .Select(x => x.RecipientOdsCode);
            IEnumerable<string> nextItemOdsCodes = orderWrapper.DetermineOrderRecipients(item.CatalogueItemId)
                .Select(x => x.RecipientOdsCode);
            var crossOver = solutionOdsCodes.Intersect(nextItemOdsCodes);

            if (solutionDates.Count == 0
                || solutionDates.All(x => x == order.DeliveryDate)
                || !crossOver.Any()
                || IsParentAdditionalService(orderItem))
            {
                return new RoutingResult
                {
                    ActionName = Constants.Actions.EditDeliveryDates,
                    ControllerName = Constants.Controllers.DeliveryDates,
                    RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId, orderItemId },
                };
            }

            return new RoutingResult
            {
                ActionName = Constants.Actions.MatchDeliveryDates,
                ControllerName = Constants.Controllers.DeliveryDates,
                RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId, orderItemId },
            };
        }

        private static bool IsParentAdditionalService(OrderItem orderItem)
        {
            return orderItem.Parent?.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService;
        }
    }
}
