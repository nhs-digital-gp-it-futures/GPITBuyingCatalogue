using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    public class CataloguePrice
    {
        public CataloguePrice()
        {
            CataloguePriceTiers = new HashSet<CataloguePriceTier>();
        }

        public int CataloguePriceId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        // TODO: remove
        public Guid PricingUnitId { get; set; }

        public string CurrencyCode { get; set; }

        public DateTime LastUpdated { get; set; }

        public decimal? Price { get; set; }

        public virtual CatalogueItem CatalogueItem { get; set; }

        public virtual CataloguePriceType CataloguePriceType { get; set; }

        public virtual PricingUnit PricingUnit { get; set; }

        public virtual ProvisioningType ProvisioningType { get; set; }

        public virtual TimeUnit? TimeUnit { get; set; }

        public virtual ICollection<CataloguePriceTier> CataloguePriceTiers { get; set; }
    }
}
