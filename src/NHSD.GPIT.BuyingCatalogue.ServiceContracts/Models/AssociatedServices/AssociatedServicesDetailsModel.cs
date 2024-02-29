using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AssociatedServices
{
    public sealed class AssociatedServicesDetailsModel
    {
        public CatalogueItemId Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string OrderGuidance { get; set; }

        public int UserId { get; set; }

        public PracticeReorganisationTypeEnum PracticeReorganisationType { get; set; }
    }
}
