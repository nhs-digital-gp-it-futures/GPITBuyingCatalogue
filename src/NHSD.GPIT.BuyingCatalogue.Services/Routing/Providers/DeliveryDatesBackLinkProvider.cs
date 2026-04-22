using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers
{
    public class DeliveryDatesBackLinkProvider : IRoutingResultProvider
    {
        public RoutingResult Process(OrderWrapper orderWrapper, RouteValues routeValues)
        {
            ArgumentNullException.ThrowIfNull(orderWrapper);
            var order = orderWrapper.Order ?? throw new ArgumentNullException(nameof(orderWrapper));

            if (routeValues == null)
            {
                throw new ArgumentNullException(nameof(routeValues));
            }

            if (routeValues.Source is RoutingSource.TaskList
                || routeValues.CatalogueItemId == null)
            {
                return new RoutingResult
                {
                    ActionName = Constants.Actions.ReviewDeliveryDates,
                    ControllerName = Constants.Controllers.DeliveryDates,
                    RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId },
                };
            }

            var catalogueItemId = order.GetPreviousOrderItemId(routeValues.CatalogueItemId.Value);

            if (catalogueItemId == null)
            {
                return new RoutingResult
                {
                    ActionName = Constants.Actions.SelectDeliveryDate,
                    ControllerName = Constants.Controllers.DeliveryDates,
                    RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId, setAllPDD = false },
                };
            }

            var solution = order.GetSolutionOrderItem();
            var orderItem = order.OrderItem(catalogueItemId.Value);

            if (order.OrderType.AssociatedServicesOnly
                || solution == null)
            {
                return new RoutingResult
                {
                    ActionName = Constants.Actions.EditDeliveryDates,
                    ControllerName = Constants.Controllers.DeliveryDates,
                    RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId, catalogueItemId },
                };
            }

            var solutionDates = orderWrapper.DetermineOrderRecipients(solution.Id)
                .Select(x => x.GetDeliveryDateForItem(solution.Id))
                .Distinct()
                .ToList();

            IEnumerable<string> solutionOdsCodes = orderWrapper.DetermineOrderRecipients(solution.Id)
                .Select(x => x.RecipientOdsCode);
            IEnumerable<string> nextItemOdsCodes = orderWrapper.DetermineOrderRecipients(orderItem.Id)
                .Select(x => x.RecipientOdsCode);
            var crossOver = solutionOdsCodes.Intersect(nextItemOdsCodes);

            if (!solutionDates.Any()
                || solutionDates.All(x => x == order.DeliveryDate)
                || !crossOver.Any())
            {
                return new RoutingResult
                {
                    ActionName = Constants.Actions.EditDeliveryDates,
                    ControllerName = Constants.Controllers.DeliveryDates,
                    RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId, catalogueItemId },
                };
            }

            return new RoutingResult
            {
                ActionName = Constants.Actions.MatchDeliveryDates,
                ControllerName = Constants.Controllers.DeliveryDates,
                RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId, catalogueItemId = routeValues.CatalogueItemId.Value },
            };
        }
    }
}
