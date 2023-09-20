using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing
{
    public interface IRoutingService
    {
        RoutingResult GetRoute(RoutingPoint point, OrderWrapper orderWrapper, RouteValues routeValues);
    }
}
