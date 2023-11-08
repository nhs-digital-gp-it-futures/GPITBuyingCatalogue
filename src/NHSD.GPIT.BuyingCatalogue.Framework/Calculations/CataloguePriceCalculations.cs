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
        public static decimal TotalOneOffCost(this Order order, Order previous, bool roundResult = false)
        {
            var total = order?.OrderItems.Sum(x => ((IPrice)x.OrderItemPrice).CalculateOneOffCost(x.TotalQuantity(order.DetermineOrderRecipients(previous, x.CatalogueItemId)))) ?? decimal.Zero;

            if (roundResult)
            {
                total = Math.Round(total, 2, MidpointRounding.AwayFromZero);
            }

            return total;
        }

        public static decimal TotalMonthlyCost(this Order order, Order previous, bool roundResult = false)
        {
            var total = order?.OrderItems
                .Sum(x => ((IPrice)x.OrderItemPrice).CalculateCostPerMonth(x.TotalQuantity(order.DetermineOrderRecipients(previous, x.CatalogueItemId))))
                ?? decimal.Zero;

            if (roundResult)
            {
                total = Math.Round(total, 2, MidpointRounding.AwayFromZero);
            }

            return total;
        }

        public static decimal TotalAnnualCost(this Order order, Order previous, bool roundResult = false)
        {
            var total = order?.OrderItems.Sum(x => ((IPrice)x.OrderItemPrice).CalculateCostPerYear(x.TotalQuantity(order.DetermineOrderRecipients(previous, x.CatalogueItemId)))) ?? decimal.Zero;

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

            return CalculateForTerm(orderItem, order.GetTerm(), recipients);
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
                TimeUnit.PerMonth => ((IPrice)orderItem.OrderItemPrice).CalculateCostPerMonth(quantity),
                TimeUnit.PerYear => ((IPrice)orderItem.OrderItemPrice).CalculateCostPerYear(quantity),
                _ => ((IPrice)orderItem.OrderItemPrice).CalculateOneOffCost(quantity),
            };
        }

        private static decimal CalculateForTerm(OrderItem orderItem, int term, ICollection<OrderRecipient> recipients)
        {
            if (orderItem == null)
                return decimal.Zero;

            var price = orderItem.OrderItemPrice as IPrice;
            return price.CalculateOneOffCost(orderItem.TotalQuantity(recipients))
                       + (price.CalculateCostPerMonth(orderItem.TotalQuantity(recipients)) * term);
        }

        private static decimal TotalCost(this Order order, Order previous)
        {
            return order.TotalOneOffCost(previous) + (order.TotalMonthlyCost(previous) * order.GetTerm());
        }

        private static int GetTerm(this Order order)
        {
            return order.DeliveryDate.HasValue && order.IsAmendment
                ? order.EndDate.RemainingTerm(order.DeliveryDate.Value)
                : order.MaximumTerm ?? 36;
        }
    }
}
