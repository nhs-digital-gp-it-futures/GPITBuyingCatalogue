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
        public static decimal CostForBillingPeriod(this IPrice price, int quantity)
        {
            var costPerTier = CostPerTierForBillingPeriod(price, quantity);
            return costPerTier.Sum(pcm => pcm.Cost);
        }

        public static IList<PriceCalculationModel> CostPerTierForBillingPeriod(this IPrice price, int quantity)
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
                ? CostForBillingPeriod(price, quantity)
                : decimal.Zero;
        }

        public static decimal CalculateCostPerMonth(this IPrice price, int quantity)
        {
            if (price?.BillingPeriod is null)
            {
                return decimal.Zero;
            }

            var cost = CostForBillingPeriod(price, quantity);

            return price.BillingPeriod == TimeUnit.PerMonth
                ? cost
                : cost / 12;
        }

        public static decimal TotalOneOffCost(this Order order, Order previous, bool roundResult = false)
        {
            var total = order?.OrderItems.Sum(x => x.OrderItemPrice.CalculateOneOffCost(x.TotalQuantity(order.DetermineOrderRecipients(previous, x.CatalogueItemId)))) ?? decimal.Zero;

            if (roundResult)
            {
                total = Math.Round(total, 2, MidpointRounding.AwayFromZero);
            }

            return total;
        }

        public static decimal TotalMonthlyCost(this Order order, Order previous, bool roundResult = false)
        {
            var total = order?.OrderItems
                .Sum(x => x.OrderItemPrice.CalculateCostPerMonth(x.TotalQuantity(order.DetermineOrderRecipients(previous, x.CatalogueItemId))))
                ?? decimal.Zero;

            if (roundResult)
            {
                total = Math.Round(total, 2, MidpointRounding.AwayFromZero);
            }

            return total;
        }

        public static decimal TotalAnnualCost(this Order order, Order previous, bool roundResult = false)
        {
            var total = order?.OrderItems.Sum(x => x.OrderItemPrice.CalculateCostPerYear(x.TotalQuantity(order.DetermineOrderRecipients(previous, x.CatalogueItemId)))) ?? decimal.Zero;

            if (roundResult)
            {
                total = Math.Round(total, 2, MidpointRounding.AwayFromZero);
            }

            return total;
        }

        public static decimal TotalPreviousCost(this OrderWrapper orderWrapper, bool roundResult = false)
        {
            if (orderWrapper == null)
            {
                return decimal.Zero;
            }

            var total = orderWrapper.PreviousOrders
                .Select((o, i) => new { Order = o, Previous = i > 0 ? orderWrapper.PreviousOrders[i - 1] : null })
                .Sum(i => i.Order.TotalCost(i.Previous));

            if (roundResult)
            {
                total = Math.Round(total, 2, MidpointRounding.AwayFromZero);
            }

            return total;
        }

        public static decimal TotalCost(this OrderWrapper orderWrapper, bool roundResult = false)
        {
            if (orderWrapper == null)
            {
                return decimal.Zero;
            }

            var total = orderWrapper.TotalPreviousCost() + orderWrapper.Order.TotalCost(orderWrapper.Previous);

            if (roundResult)
            {
                total = Math.Round(total, 2, MidpointRounding.AwayFromZero);
            }

            return total;
        }

        public static decimal TotalCostForOrderItem(this OrderWrapper orderWrapper, CatalogueItemId catalogueItemId)
        {
            if (orderWrapper == null)
            {
                return decimal.Zero;
            }

            var order = orderWrapper.Order;
            var orderItem = orderWrapper.Order.OrderItem(catalogueItemId);
            var recipients = orderWrapper.DetermineOrderRecipients(catalogueItemId);

            if (order.IsAmendment)
            {
                return CalculateForTerm(orderItem, GetTerm(order.EndDate, recipients, orderItem), recipients);
            }
            else
            {
                var maximumTerm = order.MaximumTerm ?? 36;
                return CalculateForTerm(orderItem, maximumTerm, recipients);
            }
        }

        public static decimal TotalCost(this OrderItem orderItem, ICollection<OrderRecipient> recipients)
        {
            if (orderItem?.OrderItemPrice is null)
            {
                return decimal.Zero;
            }

            var quantity = orderItem.TotalQuantity(recipients);

            return orderItem.OrderItemPrice.BillingPeriod switch
            {
                TimeUnit.PerMonth => orderItem.OrderItemPrice.CalculateCostPerMonth(quantity),
                TimeUnit.PerYear => orderItem.OrderItemPrice.CalculateCostPerYear(quantity),
                _ => orderItem.OrderItemPrice.CalculateOneOffCost(quantity),
            };
        }

        private static decimal CalculateCostPerYear(this IPrice price, int quantity)
        {
            if (price?.BillingPeriod is null)
            {
                return decimal.Zero;
            }

            var cost = CostForBillingPeriod(price, quantity);

            return price.BillingPeriod == TimeUnit.PerYear
                ? cost
                : cost * 12;
        }

        private static decimal CalculateForTerm(OrderItem orderItem, int term, ICollection<OrderRecipient> recipients)
        {
            if (orderItem == null)
                return decimal.Zero;

            var price = orderItem.OrderItemPrice;
            return price.CalculateOneOffCost(orderItem.TotalQuantity(recipients))
                       + (price.CalculateCostPerMonth(orderItem.TotalQuantity(recipients)) * term);
        }

        private static decimal TotalCost(this Order order, Order previous)
        {
            return order.IsAmendment
                ? order.TotalCostByPlannedDelivery(previous)
                : order.TotalCostForMaximumTerm(previous);
        }

        private static decimal TotalCostByPlannedDelivery(this Order order, Order previous)
        {
            return order.TotalOneOffCost(previous) + order?.OrderItems.Sum(i =>
            {
                var term = GetTerm(order.EndDate, order.DetermineOrderRecipients(previous, i.CatalogueItemId), i);
                return i.OrderItemPrice.CalculateCostPerMonth(i.TotalQuantity(order.DetermineOrderRecipients(previous, i.CatalogueItemId))) * term;
            }) ?? decimal.Zero;
        }

        private static decimal TotalCostForMaximumTerm(this Order order, Order previous)
        {
            var maximumTerm = order?.MaximumTerm ?? 36;

            return order.TotalOneOffCost(previous) + (order.TotalMonthlyCost(previous) * maximumTerm);
        }

        private static int GetTerm(EndDate endDate, IEnumerable<OrderRecipient> orderRecipients, OrderItem orderItem)
        {
            if (orderItem == null || endDate == null)
            {
                return 0;
            }

            var deliveryDate = orderRecipients.Select(x => x.GetDeliveryDateForItem(orderItem.CatalogueItemId))
                .FirstOrDefault(x => x != null);

            if (!deliveryDate.HasValue) return 0;

            var term = endDate.RemainingTerm(deliveryDate.Value);
            return term;
        }

        private static List<PriceCalculationModel> CalculateCostCumulative(IPrice price, int quantity)
        {
            var output = new List<PriceCalculationModel>();

            foreach (var (tier, index) in price.PriceTiers.OrderBy(t => t.LowerRange).Select((x, i) => (x, i)))
            {
                var tierQuantity = quantity < tier.Quantity
                    ? (quantity < 0 ? 0 : quantity)
                    : tier.Quantity;

                output.Add(new(index + 1, tierQuantity, tier.Price, tierQuantity * tier.Price));

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
                    x.Price,
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
                    x.Price,
                    x.AppliesTo(quantity) ? x.Price : decimal.Zero))
                .ToList();
        }
    }
}
