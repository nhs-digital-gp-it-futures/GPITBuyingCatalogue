using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    public sealed class AssociatedService
    {
        public CatalogueItemId AssociatedServiceId { get; set; }

        public string Description { get; set; }

        public string OrderGuidance { get; set; }

        public DateTime? LastUpdated { get; set; }

        public Guid? LastUpdatedBy { get; set; }

        public CatalogueItem CatalogueItem { get; set; }
    }
}
