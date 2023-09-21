using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing
{
    public interface IRoutingResultProvider
    {
        RoutingResult Process(OrderWrapper orderWrapper, RouteValues routeValues);
    }
}
