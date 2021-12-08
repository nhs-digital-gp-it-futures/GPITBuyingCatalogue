using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed class CataloguePrice : IAudited
    {
        public CataloguePrice()
        {
            CataloguePriceTiers = new HashSet<CataloguePriceTier>();
        }

        public int CataloguePriceId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public short PricingUnitId { get; set; }

        public string CurrencyCode { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public decimal? Price { get; set; }

        public bool IsLocked { get; set; }

        public PublicationStatus PublishedStatus { get; set; }

        public CatalogueItem CatalogueItem { get; set; }

        public CataloguePriceType CataloguePriceType { get; set; }

        public PricingUnit PricingUnit { get; set; }

        public ProvisioningType ProvisioningType { get; set; }

        public TimeUnit? TimeUnit { get; set; }

        public ICollection<CataloguePriceTier> CataloguePriceTiers { get; set; }

        public override string ToString()
        {
            return $"£{Price.Value:N4} {PricingUnit?.Description}".Trim();
        }

        public string ToPriceUnitString()
        {
            return $"{PricingUnit?.Description ?? string.Empty} {(TimeUnit.HasValue ? TimeUnit.Value.Description() : string.Empty)}".Trim();
        }
    }
}
