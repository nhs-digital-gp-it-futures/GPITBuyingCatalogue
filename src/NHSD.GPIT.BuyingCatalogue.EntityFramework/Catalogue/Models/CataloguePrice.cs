using System;
using System.Collections.Generic;
using System.Linq;
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

        public PublicationStatus PublishedStatus { get; set; }

        public CatalogueItem CatalogueItem { get; set; }

        public CataloguePriceType CataloguePriceType { get; set; }

        public PricingUnit PricingUnit { get; set; }

        public ProvisioningType ProvisioningType { get; set; }

        public CataloguePriceCalculationType CataloguePriceCalculationType { get; set; }

        public TimeUnit? TimeUnit { get; set; }

        public ICollection<CataloguePriceTier> CataloguePriceTiers { get; set; }

        public string ToPriceUnitString()
        {
            return $"{PricingUnit?.Description ?? string.Empty} {(TimeUnit.HasValue ? TimeUnit.Value.Description() : string.Empty)}".Trim();
        }

        public (bool HasOverlap, int? LowerTierIndex, int? UpperTierIndex) HasTierRangeOverlap()
        {
            var tiers = CataloguePriceTiers.OrderBy(t => t.LowerRange).ToList();
            for (int i = 0; i < tiers.Count; ++i)
            {
                if (i == (tiers.Count - 1))
                    continue;

                var current = tiers[i];
                var next = tiers[i + 1];

                if ((next.LowerRange - current.UpperRange) != 1)
                    return (true, i + 1, i + 2);
            }

            return (false, null, null);
        }

        public (bool HasGaps, int? LowerTierIndex, int? UpperTierIndex) HasTierRangeGaps()
        {
            var tiers = CataloguePriceTiers.OrderBy(t => t.LowerRange).ToList();

            for (int i = 0; i < tiers.Count; ++i)
            {
                if (i == (tiers.Count - 1))
                    continue;

                var current = tiers[i];
                var next = tiers[i + 1];

                if ((next.LowerRange - current.UpperRange) > 1)
                    return (true, i + 1, i + 2);
            }

            return (false, null, null);
        }
    }
}
