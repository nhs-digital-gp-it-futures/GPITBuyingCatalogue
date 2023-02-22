using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing
{
    public class RouteValues
    {
        public RouteValues()
        {
        }

        public RouteValues(string internalOrgId, CallOffId callOffId)
        {
            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
        }

        public RouteValues(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId)
        {
            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
            CatalogueItemId = catalogueItemId;
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public CatalogueItemId? CatalogueItemId { get; set; }

        public JourneyType? Journey { get; set; }

        public RoutingSource? Source { get; set; }

        public int? SelectedPriceId { get; set; }

        public string RecipientIds { get; set; }

        public bool FromPreviousRevision { get; set; }
    }
}
