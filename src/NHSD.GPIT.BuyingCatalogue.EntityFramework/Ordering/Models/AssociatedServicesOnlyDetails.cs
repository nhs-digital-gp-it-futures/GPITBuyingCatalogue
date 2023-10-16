using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public class AssociatedServicesOnlyDetails
    {
        public CatalogueItemId? SolutionId { get; set; }

        public CatalogueItem Solution { get; set; }
    }
}
