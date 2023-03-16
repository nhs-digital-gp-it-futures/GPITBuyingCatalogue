using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Calculations
{
    public static class CataloguePriceCalculations
    {
        public static decimal CalculateTotalCost(this IPrice price, int quantity)
        {
            var costPerTier = CalculateTotalCostPerTier(price, quantity);
            return costPerTier.Sum(pcm => pcm.Cost);
        }

        public static IList<PriceCalculationModel> CalculateTotalCostPerTier(this IPrice price, int quantity)
        {
            if (price == null)
                return new List<PriceCalculationModel>();

            return price.CataloguePriceCalculationType switch
            {
                CataloguePriceCalculationType.SingleFixed => CalculateCostSingleFixed(price, quantity),
                CataloguePriceCalculationType.Cumulative => CalculateCostCumulative(price, quantity),
                CataloguePriceCalculationType.Volume or _ => CalculateCostVolume(price, quantity),
            };
        }

        public static decimal CalculateOneOffCost(this IPrice price, int quantity)
        {
            return price?.BillingPeriod is null
                ? CalculateTotalCost(price, quantity)
                : decimal.Zero;
        }

        public static decimal CalculateCostPerMonth(this IPrice price, int quantity)
        {
            if (price?.BillingPeriod is null)
            {
                return decimal.Zero;
            }

            var cost = CalculateTotalCost(price, quantity);

            return price.BillingPeriod == TimeUnit.PerMonth
                ? cost
                : cost / 12;
        }

        public static decimal CalculateCostPerYear(this IPrice price, int quantity)
        {
            if (price?.BillingPeriod is null)
            {
                return decimal.Zero;
            }

            var cost = CalculateTotalCost(price, quantity);

            return price.BillingPeriod == TimeUnit.PerYear
                ? cost
                : cost * 12;
        }

        public static decimal CalculateTotalCostForContractLength(this IPrice price, int quantity, int maxTerm)
        {
            return price?.BillingPeriod == null
                ? CalculateOneOffCost(price, quantity)
                : CalculateCostPerMonth(price, quantity) * maxTerm;
        }

        public static decimal TotalOneOffCost(this Order order, bool roundResult = false)
        {
            var total = order?.OrderItems.Sum(x => x.OrderItemPrice.CalculateOneOffCost(x.TotalQuantity)) ?? decimal.Zero;

            if (roundResult)
            {
                total = Math.Round(total, 2, MidpointRounding.AwayFromZero);
            }

            return total;
        }

        public static decimal TotalMonthlyCost(this Order order, bool roundResult = false)
        {
            var total = order?.OrderItems.Sum(x => x.OrderItemPrice.CalculateCostPerMonth(x.TotalQuantity)) ?? decimal.Zero;

            if (roundResult)
            {
                total = Math.Round(total, 2, MidpointRounding.AwayFromZero);
            }

            return total;
        }

        public static decimal TotalAnnualCost(this Order order, bool roundResult = false)
        {
            var total = order?.OrderItems.Sum(x => x.OrderItemPrice.CalculateCostPerYear(x.TotalQuantity)) ?? decimal.Zero;

            if (roundResult)
            {
                total = Math.Round(total, 2, MidpointRounding.AwayFromZero);
            }

            return total;
        }

        public static decimal TotalCost(this Order order, bool roundResult = false)
        {
            var maximumTerm = order?.MaximumTerm ?? 36;

            var total = order.TotalOneOffCost()
                + (order.TotalMonthlyCost() * maximumTerm);

            if (roundResult)
            {
                total = Math.Round(total, 2, MidpointRounding.AwayFromZero);
            }

            return total;
        }

        public static decimal TotalCost(this OrderWrapper orderWrapper, bool roundResult = false)
        {
            var total = (orderWrapper?.Previous?.TotalCost() ?? 0)
                + (orderWrapper?.Order?.TotalCostForAmendment() ?? 0);

            if (roundResult)
            {
                total = Math.Round(total, 2, MidpointRounding.AwayFromZero);
            }

            return total;
        }

        public static decimal TotalCost(this OrderItem orderItem)
        {
            if (orderItem?.OrderItemPrice is null)
            {
                return decimal.Zero;
            }

            var quantity = orderItem.TotalQuantity;

            return orderItem.OrderItemPrice.BillingPeriod switch
            {
                TimeUnit.PerMonth => orderItem.OrderItemPrice.CalculateCostPerMonth(quantity),
                TimeUnit.PerYear => orderItem.OrderItemPrice.CalculateCostPerYear(quantity),
                _ => orderItem.OrderItemPrice.CalculateOneOffCost(quantity),
            };
        }

        private static decimal TotalCostForAmendment(this Order order)
        {
            return order.TotalOneOffCost() + order?.OrderItems.Sum(i =>
            {
                var deliveryDate = i.OrderItemRecipients.First().DeliveryDate.Value;
                var monthlyCost = i.OrderItemPrice.CalculateCostPerMonth(i.TotalQuantity);
                var term = order.EndDate.RemainingTerm(deliveryDate);
                return monthlyCost * term;
            }) ?? decimal.Zero;
        }

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

        private static List<PriceCalculationModel> CalculateCostVolume(IPrice price, int quantity)
        {
            return price.PriceTiers
                .OrderBy(x => x.LowerRange)
                .Select((x, i) => new PriceCalculationModel(
                    i + 1,
                    x.AppliesTo(quantity) ? quantity : 0,
                    x.AppliesTo(quantity) ? quantity * x.Price : decimal.Zero))
                .ToList();
        }

        private static List<PriceCalculationModel> CalculateCostSingleFixed(IPrice price, int quantity)
        {
            return price.PriceTiers
                .OrderBy(x => x.LowerRange)
                .Select((x, i) => new PriceCalculationModel(
                    i + 1,
                    x.AppliesTo(quantity) ? quantity : 0,
                    x.AppliesTo(quantity) ? x.Price : decimal.Zero))
                .ToList();
        }
    }
}
