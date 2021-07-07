using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed class PricingUnit
    {
        public Guid PricingUnitId { get; set; }

        public string Name { get; set; }

        public string TierName { get; set; }

        public string Description { get; set; }
    }
}
