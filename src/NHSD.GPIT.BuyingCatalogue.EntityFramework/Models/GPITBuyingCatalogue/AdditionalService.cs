using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    public sealed class AdditionalService
    {
        public CatalogueItemId CatalogueItemId { get; set; }

        public string Summary { get; set; }

        public string FullDescription { get; set; }

        public DateTime? LastUpdated { get; set; }

        public Guid? LastUpdatedBy { get; set; }

        public CatalogueItemId SolutionId { get; set; }

        public CatalogueItem CatalogueItem { get; set; }

        public Solution Solution { get; set; }
    }
}
