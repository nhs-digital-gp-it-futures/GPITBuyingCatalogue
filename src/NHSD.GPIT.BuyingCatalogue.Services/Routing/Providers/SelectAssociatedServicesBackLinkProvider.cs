using System;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;

namespace NHSD.GPIT.BuyingCatalogue.Services.Routing.Providers
{
    public class SelectAssociatedServicesBackLinkProvider : IRoutingResultProvider
    {
        public RoutingResult Process(OrderWrapper orderWrapper, RouteValues routeValues)
        {
            if (routeValues == null)
            {
                throw new ArgumentNullException(nameof(routeValues));
            }

            var defaultRouteValues = new
            {
                routeValues.InternalOrgId,
                routeValues.CallOffId,
            };

            return routeValues.Source switch
            {
                RoutingSource.AddAssociatedServices => new RoutingResult
                {
                    ActionName = Constants.Actions.AddAssociatedServices,
                    ControllerName = Constants.Controllers.AssociatedServices,
                    RouteValues = new
                    {
                        routeValues.InternalOrgId,
                        routeValues.CallOffId,
                        selected = true,
                    },
                },

                RoutingSource.EditSolution => new RoutingResult
                {
                    ActionName = Constants.Actions.EditSolutionAssociatedServicesOnly,
                    ControllerName = Constants.Controllers.CatalogueSolutions,
                    RouteValues = defaultRouteValues,
                },

                RoutingSource.SelectSolution => new RoutingResult
                {
                    ActionName = Constants.Actions.SelectSolutionAssociatedServicesOnly,
                    ControllerName = Constants.Controllers.CatalogueSolutions,
                    RouteValues = new
                    {
                        routeValues.InternalOrgId,
                        routeValues.CallOffId,
                        source = RoutingSource.SelectAssociatedServices,
                    },
                },

                RoutingSource.TaskList => new RoutingResult
                {
                    ActionName = Constants.Actions.TaskList,
                    ControllerName = Constants.Controllers.TaskList,
                    RouteValues = defaultRouteValues,
                },

                _ => new RoutingResult
                {
                    ActionName = Constants.Actions.OrderDashboard,
                    ControllerName = Constants.Controllers.Orders,
                    RouteValues = defaultRouteValues,
                },
            };
        }
    }
}
