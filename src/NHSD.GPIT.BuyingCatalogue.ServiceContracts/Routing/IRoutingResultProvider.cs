using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing
{
    public interface IRoutingResultProvider
    {
        RoutingResult Process(Order order, RouteValues routeValues);
    }
}
