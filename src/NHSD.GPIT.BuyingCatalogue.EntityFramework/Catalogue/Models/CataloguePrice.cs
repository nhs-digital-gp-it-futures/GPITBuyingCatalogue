using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    [Serializable]
    public sealed class CataloguePrice : IAudited, IPrice
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

        public CataloguePriceQuantityCalculationType? CataloguePriceQuantityCalculationType { get; set; }

        public TimeUnit? TimeUnit { get; set; }

        public ICollection<CataloguePriceTier> CataloguePriceTiers { get; set; }

        public ICollection<IPriceTier> PriceTiers => CataloguePriceTiers.Cast<IPriceTier>().ToList();

        public string Description
        {
            get { return PricingUnit?.Description; }
            set { (PricingUnit ??= new PricingUnit()).RangeDescription = value; }
        }

        public TimeUnit? BillingPeriod => TimeUnit;

        public string RangeDescription
        {
            get { return PricingUnit?.RangeDescription; }
            set { (PricingUnit ??= new PricingUnit()).RangeDescription = value; }
        }

        public string ToPriceUnitString()
        {
            return
                $"{PricingUnit?.Description ?? string.Empty} {(TimeUnit.HasValue ? TimeUnit.Value.Description() : string.Empty)}"
                    .Trim();
        }

        public bool HasDifferentQuantityBasisThan(IPrice price)
        {
            if (price == null)
            {
                throw new ArgumentNullException(nameof(price));
            }

            return ProvisioningType != price.ProvisioningType
                || CataloguePriceQuantityCalculationType != price.CataloguePriceQuantityCalculationType;
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
