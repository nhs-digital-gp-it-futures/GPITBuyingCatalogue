using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces
{
    public interface IPrice
    {
        public ICollection<IPriceTier> PriceTiers { get; }

        public string Description { get; set; }

        public ProvisioningType ProvisioningType { get; set; }

        public CataloguePriceCalculationType CataloguePriceCalculationType { get; set; }

        public CataloguePriceQuantityCalculationType? CataloguePriceQuantityCalculationType { get; set; }

        public TimeUnit? BillingPeriod { get; }

        public CataloguePriceType CataloguePriceType { get; set; }

        public string CurrencyCode { get; set; }

        public string RangeDescription { get; set; }

        public bool IsPerServiceRecipient() => ProvisioningType.IsPerServiceRecipient()
            || CataloguePriceQuantityCalculationType is Catalogue.Models.CataloguePriceQuantityCalculationType.PerServiceRecipient;

        public string ToPriceUnitString()
        {
            return $"{Description} {BillingPeriod?.Description() ?? string.Empty}".Trim();
        }

        public string GetRangeDescription(IPriceTier priceTier)
        {
            var upperRange = priceTier.UpperRange == null
                ? "+"
                : $" to {priceTier.UpperRange.Value}";

            return $"{priceTier.LowerRange}{upperRange} {RangeDescription}".Trim();
        }

        public decimal CostForBillingPeriod(int quantity)
        {
            var costPerTier = CostPerTierForBillingPeriod(quantity);
            return costPerTier.Sum(pcm => pcm.Cost);
        }

        public IList<PriceCalculationModel> CostPerTierForBillingPeriod(int quantity)
        {
            return CataloguePriceCalculationType switch
            {
                CataloguePriceCalculationType.SingleFixed => CalculateCostSingleFixed(quantity),
                CataloguePriceCalculationType.Cumulative => CalculateCostCumulative(quantity),
                CataloguePriceCalculationType.Volume or _ => CalculateCostVolume(quantity),
            };
        }

        public decimal CalculateOneOffCost(int quantity)
        {
            return BillingPeriod is null
                ? CostForBillingPeriod(quantity)
                : decimal.Zero;
        }

        public decimal CalculateCostPerMonth(int quantity)
        {
            if (BillingPeriod is null)
            {
                return decimal.Zero;
            }

            var cost = CostForBillingPeriod(quantity);

            return BillingPeriod == TimeUnit.PerMonth
                ? cost
                : cost / 12;
        }

        public decimal CalculateCostPerYear(int quantity)
        {
            if (BillingPeriod is null)
            {
                return decimal.Zero;
            }

            var cost = CostForBillingPeriod(quantity);

            return BillingPeriod == TimeUnit.PerYear
                ? cost
                : cost * 12;
        }

        private List<PriceCalculationModel> CalculateCostCumulative(int quantity)
        {
            var output = new List<PriceCalculationModel>();

            foreach (var (tier, index) in PriceTiers.OrderBy(t => t.LowerRange).Select((x, i) => (x, i)))
            {
                var tierQuantity = quantity < tier.Quantity
                    ? (quantity < 0 ? 0 : quantity)
                    : tier.Quantity;

                output.Add(new(index + 1, tierQuantity, tier.Price, tierQuantity * tier.Price));

                quantity -= tierQuantity;
            }

            return output;
        }

        private List<PriceCalculationModel> CalculateCostVolume(int quantity)
        {
            return PriceTiers
                .OrderBy(x => x.LowerRange)
                .Select((x, i) => new PriceCalculationModel(
                    i + 1,
                    x.AppliesTo(quantity) ? quantity : 0,
                    x.Price,
                    x.AppliesTo(quantity) ? quantity * x.Price : decimal.Zero))
                .ToList();
        }

        private List<PriceCalculationModel> CalculateCostSingleFixed(int quantity)
        {
            return PriceTiers
                .OrderBy(x => x.LowerRange)
                .Select((x, i) => new PriceCalculationModel(
                    i + 1,
                    x.AppliesTo(quantity) ? quantity : 0,
                    x.Price,
                    x.AppliesTo(quantity) ? x.Price : decimal.Zero))
                .ToList();
        }
    }
}
