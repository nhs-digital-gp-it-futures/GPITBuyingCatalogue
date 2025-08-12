using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces
{
    public interface IPrice
    {
        private const int DefaultQuantityOffset = 0;

        public ICollection<IPriceTier> PriceTiers { get; }

        public string Description { get; set; }

        public ProvisioningType ProvisioningType { get; set; }

        public CataloguePriceCalculationType CataloguePriceCalculationType { get; set; }

        public CataloguePriceQuantityCalculationType? CataloguePriceQuantityCalculationType { get; set; }

        public TimeUnit? BillingPeriod { get; }

        public int CataloguePriceId { get; }

        public CataloguePriceType CataloguePriceType { get; set; }

        public string CurrencyCode { get; set; }

        public string RangeDescription { get; set; }

        public bool IsPerServiceRecipient() => ProvisioningType.IsPerServiceRecipient()
            || CataloguePriceQuantityCalculationType is Catalogue.Models.CataloguePriceQuantityCalculationType
                .PerServiceRecipient;

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

        public decimal CostForBillingPeriod(int quantity) => CostForBillingPeriod(quantity, DefaultQuantityOffset);

        public decimal CostForBillingPeriod(int quantity, int quantityOffset)
        {
            var costPerTier = CalculateCostPerTier(quantity, quantityOffset);
            return costPerTier.Sum(pcm => pcm.Cost);
        }

        public IList<PriceCalculationModel> CalculateCostPerTier(int quantity) =>
            CalculateCostPerTier(quantity, DefaultQuantityOffset);

        public IList<PriceCalculationModel> CalculateCostPerTier(int quantity, int quantityOffset)
        {
            return CataloguePriceCalculationType switch
            {
                CataloguePriceCalculationType.SingleFixed => CalculateCostSingleFixed(quantity),
                CataloguePriceCalculationType.Cumulative => CalculateCostCumulative(quantity, quantityOffset),
                _ => CalculateCostVolume(quantity),
            };
        }

        public decimal CalculateOneOffCost(int quantity)
        {
            return BillingPeriod is null
                ? CostForBillingPeriod(quantity)
                : decimal.Zero;
        }

        public decimal CalculateCostPerMonth(int quantity) => CalculateCostPerMonth(quantity, DefaultQuantityOffset);

        public decimal CalculateCostPerYear(int quantity) => CalculateCostPerMonth(quantity, DefaultQuantityOffset) * 12;

        public decimal CalculateCostPerMonth(int quantity, int quantityOffset)
        {
            if (BillingPeriod is null)
                return decimal.Zero;

            var cost = CostForBillingPeriod(quantity, quantityOffset);

            return BillingPeriod == TimeUnit.PerMonth
                ? cost
                : cost / 12;
        }

        private IList<PriceCalculationModel> CalculateCostCumulative(int quantity, int quantityOffset)
        {
            var output = new List<PriceCalculationModel>();

            foreach ((IPriceTier tier, var index) in PriceTiers.OrderBy(t => t.LowerRange).Select((x, i) => (x, i)))
            {
                int tierEnd = tier.UpperRange ?? int.MaxValue;
                int tierQuantity = 0;

                if (quantityOffset < tierEnd)
                {
                    int tierStart = quantityOffset <= 0
                        ? 0
                        : Math.Max(quantityOffset, tier.LowerRange + 1);

                    int tierCapacity = tierEnd - tierStart;
                    tierQuantity = Math.Min(quantity, tierCapacity);

                    quantity -= tierQuantity;
                }

                output.Add(
                    new PriceCalculationModel(
                        index + 1,
                        tierQuantity,
                        tier.Price));
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
