using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Calculations
{
    public static class CataloguePriceCalculations
    {
        public static decimal CalculateTotalCost(this IPrice price, int quantity)
        {
            if (price == null)
                throw new ArgumentNullException(nameof(price));

            return price.CataloguePriceCalculationType switch
            {
                CataloguePriceCalculationType.SingleFixed => CalculateTotalCostSingleFixed(price, quantity),
                CataloguePriceCalculationType.Cumulative => CalculateTotalCostCumulative(price, quantity),
                CataloguePriceCalculationType.Volume or _ => CalculateTotalCostVolume(price, quantity),
            };
        }

        public static IList<PriceCalculationModel> CalculateTotalCostPerTier(this IPrice price, int quantity)
        {
            if (price == null)
                throw new ArgumentNullException(nameof(price));

            return price.CataloguePriceCalculationType switch
            {
                CataloguePriceCalculationType.SingleFixed => CalculateCostSingleFixed(price, quantity),
                CataloguePriceCalculationType.Cumulative => CalculateCostCumulative(price, quantity),
                CataloguePriceCalculationType.Volume or _ => CalculateCostVolume(price, quantity),
            };
        }

        public static decimal CalculateCostPerMonth(this IPrice price, int quantity)
        {
            var cost = CalculateTotalCost(price, quantity);

            return price.EstimationPeriod is TimeUnit.PerMonth
                ? cost
                : cost / 12;
        }

        public static decimal CalculateCostPerYear(this IPrice price, int quantity)
        {
            var cost = CalculateTotalCost(price, quantity);

            return price.EstimationPeriod is TimeUnit.PerYear
                ? cost
                : cost * 12;
        }

        public static decimal CalculateTotalCostForContractLength(this IPrice price, int quantity, int maxTerm) =>
            CalculateCostPerMonth(price, quantity) * maxTerm;

        private static decimal CalculateTotalCostCumulative(IPrice price, int quantity) =>
            CalculateCostCumulative(price, quantity).Sum(pcm => pcm.Cost);

        private static List<PriceCalculationModel> CalculateCostCumulative(IPrice price, int quantity)
        {
            var lastUpperRange = 0;
            var tierCalculations = new List<PriceCalculationModel>();

            var tiers = price.PriceTiers.OrderBy(t => t.LowerRange);

            for (var i = 0; i < tiers.Count(); i++)
            {
                var tier = tiers.ElementAt(i);

                int range;
                if (!tier.UpperRange.HasValue)
                {
                    range = quantity;
                }
                else if (quantity == 0)
                {
                    range = quantity;
                }
                else
                {
                    range = tier.UpperRange.Value;
                    range -= lastUpperRange;
                    lastUpperRange = tier.UpperRange.Value;

                    if (range > quantity)
                        range = quantity;
                }

                var tierCost = tier.Price * range;

                tierCalculations.Add(new PriceCalculationModel(i + 1, range, tierCost));

                var futureQuantity = quantity - range;

                if (futureQuantity < 0)
                    futureQuantity = 0;

                quantity = futureQuantity;
            }

            return tierCalculations;
        }

        private static decimal CalculateTotalCostSingleFixed(IPrice price, int quantity) =>
            CalculateCostSingleFixed(price, quantity).Sum(pcm => pcm.Cost);

        private static List<PriceCalculationModel> CalculateCostSingleFixed(IPrice price, int quantity)
        {
            var tierCalculations = new List<PriceCalculationModel>();

            var tiers = price.PriceTiers.OrderBy(t => t.LowerRange);

            for (var i = 0; i < tiers.Count(); i++)
            {
                var tier = tiers.ElementAt(i);

                if (quantity > 0 && ((tier.UpperRange.HasValue && tier.UpperRange.Value >= quantity) || !tier.UpperRange.HasValue))
                {
                    tierCalculations.Add(new PriceCalculationModel(i + 1, quantity, tier.Price));
                    quantity = 0;
                }
                else
                {
                    tierCalculations.Add(new PriceCalculationModel(i + 1, 0, 0M));
                }
            }

            return tierCalculations;
        }

        private static decimal CalculateTotalCostVolume(IPrice price, int quantity) =>
            CalculateCostVolume(price, quantity).Sum(pcm => pcm.Cost);

        private static List<PriceCalculationModel> CalculateCostVolume(IPrice price, int quantity)
        {
            var tierCalculations = new List<PriceCalculationModel>();

            var tiers = price.PriceTiers.OrderBy(t => t.LowerRange);

            for (var i = 0; i < tiers.Count(); i++)
            {
                var tier = tiers.ElementAt(i);

                if (quantity > 0 && ((tier.UpperRange.HasValue && tier.UpperRange.Value >= quantity) || !tier.UpperRange.HasValue))
                {
                    tierCalculations.Add(new PriceCalculationModel(i + 1, quantity, tier.Price * quantity));
                    quantity = 0;
                }
                else
                {
                    tierCalculations.Add(new PriceCalculationModel(i + 1, 0, 0M));
                }
            }

            return tierCalculations;
        }
    }
}
