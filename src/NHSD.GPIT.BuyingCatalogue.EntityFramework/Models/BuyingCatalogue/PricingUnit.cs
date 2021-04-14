using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue
{
    public partial class PricingUnit
    {
        public PricingUnit()
        {
            CataloguePrices = new HashSet<CataloguePrice>();
        }

        public Guid PricingUnitId { get; set; }
        public string Name { get; set; }
        public string TierName { get; set; }
        public string Description { get; set; }

        public virtual ICollection<CataloguePrice> CataloguePrices { get; set; }
    }
}
