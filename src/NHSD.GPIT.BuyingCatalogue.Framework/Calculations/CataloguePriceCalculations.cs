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
                    x.TotalQuantity(order.DetermineOrderRecipients(previous, x.Id)))) ?? decimal.Zero;

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
                    x.TotalQuantity(order.DetermineOrderRecipients(previous, x.Id)))) ?? decimal.Zero;

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

        public static Dictionary<int, decimal> TotalCostPerTier(
            this OrderWrapper orderWrapper,
            int orderItemId,
            bool includeCurrentRevision = true)
        {
            if (orderWrapper == null)
            {
                return new Dictionary<int, decimal>();
            }

            var allRevisions = orderWrapper.PreviousOrders.Append(orderWrapper.Order)
                .OrderBy(o => o.Created)
                .ToList();
            var selectedOrder = allRevisions.FirstOrDefault(order => order.OrderItem(orderItemId) != null);
            var selectedOrderItem = selectedOrder?.OrderItem(orderItemId);

            if (selectedOrderItem == null)
            {
                return new Dictionary<int, decimal>();
            }

            var selectedOrderIndex = allRevisions.IndexOf(selectedOrder);
            var firstRevisionIndex = selectedOrderItem.CatalogueItem.CatalogueItemType
                == CatalogueItemType.AssociatedService
                    ? selectedOrderIndex
                    : 0;
            var lastRevisionIndex = includeCurrentRevision ? selectedOrderIndex : selectedOrderIndex - 1;
            var totalCosts = new Dictionary<int, decimal>();
            var quantityOffset = 0;

            for (var i = firstRevisionIndex; i <= lastRevisionIndex; i++)
            {
                var order = allRevisions[i];
                var previous = i > 0 ? allRevisions[i - 1] : null;
                var orderItem = order.OrderItems.FirstOrDefault(FilterItemByItemType(selectedOrderItem));

                if (orderItem?.OrderItemPrice is not IPrice price)
                {
                    continue;
                }

                var quantity = orderItem.TotalQuantity(order.DetermineOrderRecipients(previous, orderItem.Id));
                var tierCosts = price.CalculateCostPerTier(
                    quantity,
                    price.BillingPeriod.HasValue ? quantityOffset : 0);
                var term = order.GetTerm();

                AddTierToTotal(tierCosts, price, term, totalCosts);

                quantityOffset += quantity;
            }

            return totalCosts;
        }

        public static decimal TotalCostForOrderItem(this OrderWrapper orderWrapper, int orderItemId)
        {
            if (orderWrapper == null)
            {
                return decimal.Zero;
            }

            var order = orderWrapper.Order;
            var orderItem = orderWrapper.Order.OrderItem(orderItemId);

            if (orderItem == null) return decimal.Zero;

            var recipients = orderWrapper.DetermineOrderRecipients(orderItem);

            return CalculateForTerm(orderItem, order.GetTerm(), recipients);
        }

        public static decimal TotalCost(this OrderItem orderItem, ICollection<OrderSublocationRecipient> recipients)
        {
            if (orderItem?.OrderItemPrice is not IPrice price)
            {
                return decimal.Zero;
            }

            var quantity = orderItem.TotalQuantity(recipients);

            return orderItem.OrderItemPrice.BillingPeriod switch
            {
                TimeUnit.PerMonth => price.CalculateCostPerMonth(quantity),
                TimeUnit.PerYear => price.CalculateCostPerYear(quantity),
                _ => price.CalculateOneOffCost(quantity),
            };
        }

        private static void AddTierToTotal(IList<PriceCalculationModel> tierCosts, IPrice price, int term, Dictionary<int, decimal> totalCosts)
        {
            foreach (var tierCost in tierCosts)
            {
                var cost = price.BillingPeriod switch
                {
                    TimeUnit.PerMonth => tierCost.Cost * term,
                    TimeUnit.PerYear => tierCost.Cost * term / 12,
                    _ => tierCost.Cost,
                };

                totalCosts[tierCost.Id] = totalCosts.GetValueOrDefault(tierCost.Id) + cost;
            }
        }

        private static Func<OrderItem, bool> FilterItemByItemType(OrderItem selectedOrderItem)
        {
            return item => item.CatalogueItem.CatalogueItemType != CatalogueItemType.AssociatedService
                ? item.CatalogueItemId == selectedOrderItem.CatalogueItemId
                : item.Id == selectedOrderItem.Id;
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
                    var qty = item.TotalQuantity(order.DetermineOrderRecipients(previous, item.Id));
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

                var quantity = item.TotalQuantity(order.DetermineOrderRecipients(previous, item.Id));
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
