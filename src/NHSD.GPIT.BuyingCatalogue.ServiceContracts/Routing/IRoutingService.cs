using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing
{
    public interface IRoutingService
    {
        RoutingResult GetRoute(RoutingSource source, Order order, RouteValues routeValues);
    }
}
