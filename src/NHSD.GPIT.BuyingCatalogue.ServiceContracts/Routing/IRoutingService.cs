using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing
{
    public interface IRoutingService
    {
        RoutingResult GetRoute(RoutingPoint point, Order order, RouteValues routeValues);
    }
}
