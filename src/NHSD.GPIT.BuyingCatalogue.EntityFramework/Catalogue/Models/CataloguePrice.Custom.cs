using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed partial class CataloguePrice : IPrice
    {
        public ICollection<IPriceTier> PriceTiers => CataloguePriceTiers.Cast<IPriceTier>().ToList();

        public string Description => PricingUnit.Description;

        public TimeUnit? BillingPeriod => TimeUnit;

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
