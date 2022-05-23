using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers
{
    public class EditPriceBackLinkProvider : IRoutingResultProvider
    {
        public RoutingResult Process(Order order, RouteValues routeValues)
        {
            if (routeValues == null)
            {
                throw new ArgumentNullException(nameof(routeValues));
            }

            if (routeValues.Source == RoutingSource.TaskList)
            {
                return new RoutingResult
                {
                    ActionName = Constants.Actions.TaskList,
                    ControllerName = Constants.Controllers.TaskList,
                    RouteValues = new { routeValues.CallOffId, routeValues.InternalOrgId },
                };
            }

            return new RoutingResult
            {
                ActionName = Constants.Actions.EditServiceRecipients,
                ControllerName = Constants.Controllers.ServiceRecipients,
                RouteValues = new { routeValues.CallOffId, routeValues.InternalOrgId, routeValues.CatalogueItemId },
            };
        }
    }
}
