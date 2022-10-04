using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers
{
    public class EditDeliveryDatesBackLinkProvider : IRoutingResultProvider
    {
        public RoutingResult Process(Order order, RouteValues routeValues)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

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
                    RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId },
                };
            }

            var solution = order.GetSolution();

            if (order.AssociatedServicesOnly
                || solution == null)
            {
                return new RoutingResult
                {
                    ActionName = Constants.Actions.EditDeliveryDates,
                    ControllerName = Constants.Controllers.DeliveryDates,
                    RouteValues = new { routeValues.InternalOrgId, routeValues.CallOffId, catalogueItemId },
                };
            }

            var solutionDates = solution.OrderItemRecipients.Select(x => x.DeliveryDate).Distinct().ToList();
            var solutionOdsCodes = solution.OrderItemRecipients.Select(x => x.OdsCode);
            var nextItemOdsCodes = order.OrderItem(catalogueItemId.Value).OrderItemRecipients.Select(x => x.OdsCode);
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
