using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed class CataloguePrice
    {
        public CataloguePrice()
        {
            CataloguePriceTiers = new HashSet<CataloguePriceTier>();
        }

        public int CataloguePriceId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        // TODO: remove
        public short PricingUnitId { get; set; }

        public string CurrencyCode { get; set; }

        public DateTime LastUpdated { get; set; }

        public decimal? Price { get; set; }

        public CatalogueItem CatalogueItem { get; set; }

        public CataloguePriceType CataloguePriceType { get; set; }

        public PricingUnit PricingUnit { get; set; }

        public ProvisioningType ProvisioningType { get; set; }

        public TimeUnit? TimeUnit { get; set; }

        public ICollection<CataloguePriceTier> CataloguePriceTiers { get; set; }
    }
}
