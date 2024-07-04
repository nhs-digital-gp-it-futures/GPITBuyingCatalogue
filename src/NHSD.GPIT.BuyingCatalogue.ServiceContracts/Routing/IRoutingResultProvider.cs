using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing
{
    public interface IRoutingResultProvider
    {
        RoutingResult Process(OrderWrapper orderWrapper, RouteValues routeValues);
    }
}
