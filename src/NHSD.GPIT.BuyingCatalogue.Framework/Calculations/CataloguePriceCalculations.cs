using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

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

        public static decimal CalculateOneOffCost(this IPrice price, int quantity)
        {
            return price?.EstimationPeriod is null
                ? CalculateTotalCost(price, quantity)
                : decimal.Zero;
        }

        public static decimal CalculateCostPerMonth(this IPrice price, int quantity)
        {
            if (price?.EstimationPeriod is null)
            {
                return decimal.Zero;
            }

            var cost = CalculateTotalCost(price, quantity);

            return price.EstimationPeriod == TimeUnit.PerMonth
                ? cost
                : cost / 12;
        }

        public static decimal CalculateCostPerYear(this IPrice price, int quantity)
        {
            if (price?.EstimationPeriod is null)
            {
                return decimal.Zero;
            }

            var cost = CalculateTotalCost(price, quantity);

            return price.EstimationPeriod == TimeUnit.PerYear
                ? cost
                : cost * 12;
        }

        public static decimal CalculateTotalCostForContractLength(this IPrice price, int quantity, int maxTerm)
        {
            return price?.EstimationPeriod == null
                ? CalculateOneOffCost(price, quantity)
                : CalculateCostPerMonth(price, quantity) * maxTerm;
        }

        public static decimal TotalOneOffCost(this Order order)
        {
            return order?.OrderItems.Sum(x => x.OrderItemPrice.CalculateOneOffCost(x.GetQuantity())) ?? decimal.Zero;
        }

        public static decimal TotalMonthlyCost(this Order order)
        {
            return order?.OrderItems.Sum(x => x.OrderItemPrice.CalculateCostPerMonth(x.GetQuantity())) ?? decimal.Zero;
        }

        public static decimal TotalAnnualCost(this Order order)
        {
            return order?.OrderItems.Sum(x => x.OrderItemPrice.CalculateCostPerYear(x.GetQuantity())) ?? decimal.Zero;
        }

        public static decimal TotalCost(this Order order)
        {
            if (order?.MaximumTerm is null)
            {
                return decimal.Zero;
            }

            return order.TotalOneOffCost()
                + (order.TotalMonthlyCost() * order.MaximumTerm!.Value);
        }

        public static decimal TotalCost(this OrderItem orderItem)
        {
            if (orderItem?.OrderItemPrice is null)
            {
                return decimal.Zero;
            }

            var quantity = orderItem.GetQuantity();

            return orderItem.OrderItemPrice.EstimationPeriod switch
            {
                TimeUnit.PerMonth => orderItem.OrderItemPrice.CalculateCostPerMonth(quantity),
                TimeUnit.PerYear => orderItem.OrderItemPrice.CalculateCostPerYear(quantity),
                _ => orderItem.OrderItemPrice.CalculateOneOffCost(quantity),
            };
        }

        private static decimal CalculateTotalCostCumulative(IPrice price, int quantity) =>
            CalculateCostCumulative(price, quantity).Sum(pcm => pcm.Cost);

        private static List<PriceCalculationModel> CalculateCostCumulative(IPrice price, int quantity)
        {
            var output = new List<PriceCalculationModel>();

            foreach (var (tier, index) in price.PriceTiers.OrderBy(t => t.LowerRange).Select((x, i) => (x, i)))
            {
                var tierQuantity = quantity < tier.Quantity
                    ? (quantity < 0 ? 0 : quantity)
                    : tier.Quantity;

                output.Add(new(index + 1, tierQuantity, tierQuantity * tier.Price));

                quantity -= tierQuantity;
            }

            return output;
        }

        private static decimal CalculateTotalCostSingleFixed(IPrice price, int quantity) =>
            CalculateCostSingleFixed(price, quantity).Sum(pcm => pcm.Cost);

        private static List<PriceCalculationModel> CalculateCostSingleFixed(IPrice price, int quantity)
        {
            return price.PriceTiers
                .OrderBy(x => x.LowerRange)
                .Select((x, i) => new PriceCalculationModel(
                    i + 1,
                    x.AppliesTo(quantity) ? quantity : 0,
                    x.AppliesTo(quantity) ? quantity * x.Price : decimal.Zero))
                .ToList();
        }

        private static decimal CalculateTotalCostVolume(IPrice price, int quantity) =>
            CalculateCostVolume(price, quantity).Sum(pcm => pcm.Cost);

        private static List<PriceCalculationModel> CalculateCostVolume(IPrice price, int quantity) => CalculateCostSingleFixed(price, quantity);
    }
}
