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
        private static readonly Dictionary<CatalogueItemId, int> DefaultQuantityOffsets = new();

        public static decimal TotalOneOffCost(this Order order, Order previous, bool roundResult = false)
        {
            var total = order?.OrderItems.Sum(x =>
                ((IPrice)x.OrderItemPrice).CalculateOneOffCost(
                    x.TotalQuantity(order.DetermineOrderRecipients(previous, x.CatalogueItemId)))) ?? decimal.Zero;

            if (roundResult)
            {
                total = Math.Round(total, 2, MidpointRounding.AwayFromZero);
            }

            return total;
        }

        public static decimal TotalMonthlyCost(this Order order, Order previous, bool roundResult = false) =>
            order.TotalMonthlyCost(previous, DefaultQuantityOffsets, roundResult);

        public static decimal TotalAnnualCost(this Order order, Order previous, bool roundResult = false)
        {
            var total = order?.OrderItems.Sum(x =>
                ((IPrice)x.OrderItemPrice).CalculateCostPerYear(
                    x.TotalQuantity(order.DetermineOrderRecipients(previous, x.CatalogueItemId)))) ?? decimal.Zero;

            if (roundResult)
            {
                total = Math.Round(total, 2, MidpointRounding.AwayFromZero);
            }

            return total;
        }

        public static decimal TotalPreviousCost(this OrderWrapper orderWrapper, bool roundResult = false)
        {
            if (orderWrapper == null) return decimal.Zero;

            var orderedRevisions = orderWrapper.PreviousOrders
                .OrderBy(o => o.Created)
                .ToList();

            return TotalCost(orderedRevisions, roundResult);
        }

        public static decimal TotalCost(this OrderWrapper orderWrapper, bool roundResult = false)
        {
            if (orderWrapper == null) return decimal.Zero;

            var orderedRevisions = orderWrapper.PreviousOrders.Append(orderWrapper.Order)
                .OrderBy(o => o.Created)
                .ToList();

            return TotalCost(orderedRevisions, roundResult);
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

            return CalculateForTerm(orderItem, order.GetTerm(), recipients);
        }

        public static decimal TotalCost(this OrderItem orderItem, ICollection<OrderSublocationRecipient> recipients)
        {
            if (orderItem?.OrderItemPrice is null)
            {
                return decimal.Zero;
            }

            var quantity = orderItem.TotalQuantity(recipients);

            return orderItem.OrderItemPrice.BillingPeriod switch
            {
                TimeUnit.PerMonth => ((IPrice)orderItem.OrderItemPrice).CalculateCostPerMonth(quantity),
                TimeUnit.PerYear => ((IPrice)orderItem.OrderItemPrice).CalculateCostPerYear(quantity),
                _ => ((IPrice)orderItem.OrderItemPrice).CalculateOneOffCost(quantity),
            };
        }

        private static decimal TotalCost(IReadOnlyList<Order> orders, bool roundResult = false)
        {
            var cumulativeOffsets = new Dictionary<CatalogueItemId, int>();
            decimal total = 0;

            for (int i = 0; i < orders.Count; i++)
            {
                var order = orders[i];
                var previous = i > 0 ? orders[i - 1] : null;

                decimal revisionCost = order.TotalCost(previous, cumulativeOffsets);

                foreach (var item in order.OrderItems)
                {
                    var qty = item.TotalQuantity(order.DetermineOrderRecipients(previous, item.CatalogueItemId));
                    if (!cumulativeOffsets.TryAdd(item.CatalogueItemId, qty))
                        cumulativeOffsets[item.CatalogueItemId] += qty;
                }

                total += revisionCost;
            }

            return roundResult
                ? Math.Round(total, 2, MidpointRounding.AwayFromZero)
                : total;
        }

        private static decimal CalculateForTerm(
            OrderItem orderItem,
            int term,
            ICollection<OrderSublocationRecipient> recipients)
        {
            if (orderItem == null)
                return decimal.Zero;

            IPrice price = orderItem.OrderItemPrice;
            return price.CalculateOneOffCost(orderItem.TotalQuantity(recipients))
                + (price.CalculateCostPerMonth(orderItem.TotalQuantity(recipients)) * term);
        }

        private static decimal TotalMonthlyCost(
            this Order order,
            Order previous,
            Dictionary<CatalogueItemId, int> quantityOffsets,
            bool roundResult = false)
        {
            if (order is null) return decimal.Zero;

            var total = order.OrderItems.Sum(item =>
                TotalMonthlyCostInternal(item, order, previous, quantityOffsets));

            return roundResult
                ? Math.Round(total, 2, MidpointRounding.AwayFromZero)
                : total;

            static decimal TotalMonthlyCostInternal(
                OrderItem item,
                Order order,
                Order previous,
                Dictionary<CatalogueItemId, int> quantityOffsets)
            {
                if (item?.OrderItemPrice is not IPrice price)
                    return decimal.Zero;

                var quantity = item.TotalQuantity(order.DetermineOrderRecipients(previous, item.CatalogueItemId));
                var offset = quantityOffsets.TryGetValue(item.CatalogueItemId, out var val) ? val : 0;

                return price.CalculateCostPerMonth(quantity, offset);
            }
        }

        private static decimal TotalCost(
            this Order order,
            Order previous,
            Dictionary<CatalogueItemId, int> quantityOffsets)
        {
            if (order is null) return decimal.Zero;

            var oneOff = order.TotalOneOffCost(previous);
            var monthly = order.TotalMonthlyCost(previous, quantityOffsets);

            var total = oneOff + (monthly * order.GetTerm());

            return total;
        }

        private static int GetTerm(this Order order)
        {
            return order.DeliveryDate.HasValue && order.IsAmendment
                ? order.EndDate.RemainingTermInMonths(order.DeliveryDate.Value)
                : order.MaximumTerm ?? 36;
        }
    }
}
