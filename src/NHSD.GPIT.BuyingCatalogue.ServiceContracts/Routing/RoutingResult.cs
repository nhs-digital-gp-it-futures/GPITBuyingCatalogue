namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing
{
    public class RoutingResult
    {
        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public object RouteValues { get; set; }
    }
}
