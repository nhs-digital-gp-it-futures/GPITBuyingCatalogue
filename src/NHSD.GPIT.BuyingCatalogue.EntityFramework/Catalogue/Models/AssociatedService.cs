using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    [Serializable]
    public sealed class AssociatedService : IAudited
    {
        public CatalogueItemId CatalogueItemId { get; set; }

        public string Description { get; set; }

        public string OrderGuidance { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public CatalogueItem CatalogueItem { get; set; }

        public PracticeReorganisationTypeEnum PracticeReorganisationType { get; set; }
    }
}
