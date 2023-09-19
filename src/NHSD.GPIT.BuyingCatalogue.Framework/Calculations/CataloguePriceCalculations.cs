using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Calculations
{
    public static class CataloguePriceCalculations
    {
        public static decimal TotalOneOffCost(this Order order, bool roundResult = false)
        {
            var total = order?.OrderItems.Sum(x => ((IPrice)x.OrderItemPrice).CalculateOneOffCost(x.TotalQuantity)) ?? decimal.Zero;

            if (roundResult)
            {
                total = Math.Round(total, 2, MidpointRounding.AwayFromZero);
            }

            return total;
        }

        public static decimal TotalMonthlyCost(this Order order, bool roundResult = false)
        {
            var total = order?.OrderItems.Sum(x => ((IPrice)x.OrderItemPrice).CalculateCostPerMonth(x.TotalQuantity)) ?? decimal.Zero;

            if (roundResult)
            {
                total = Math.Round(total, 2, MidpointRounding.AwayFromZero);
            }

            return total;
        }

        public static decimal TotalAnnualCost(this Order order, bool roundResult = false)
        {
            var total = order?.OrderItems.Sum(x => ((IPrice)x.OrderItemPrice).CalculateCostPerYear(x.TotalQuantity)) ?? decimal.Zero;

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

            var total = orderWrapper.PreviousOrders.Sum(o => o.TotalCost());

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

            var total = orderWrapper.TotalPreviousCost() + orderWrapper.Order.TotalCost();

            if (roundResult)
            {
                total = Math.Round(total, 2, MidpointRounding.AwayFromZero);
            }

            return total;
        }

        public static decimal TotalCostForOrderItem(this Order order, CatalogueItemId catalogueItemId)
        {
            if (order == null)
            {
                return decimal.Zero;
            }

            var orderItem = order.OrderItem(catalogueItemId);

            if (order.IsAmendment)
            {
                return CalculateForTerm(orderItem, GetTerm(order.EndDate, orderItem));
            }
            else
            {
                var maximumTerm = order.MaximumTerm ?? 36;
                return CalculateForTerm(orderItem, maximumTerm);
            }
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
                TimeUnit.PerMonth => ((IPrice)orderItem.OrderItemPrice).CalculateCostPerMonth(quantity),
                TimeUnit.PerYear => ((IPrice)orderItem.OrderItemPrice).CalculateCostPerYear(quantity),
                _ => ((IPrice)orderItem.OrderItemPrice).CalculateOneOffCost(quantity),
            };
        }

        private static decimal CalculateForTerm(OrderItem orderItem, int term)
        {
            if (orderItem == null)
                return decimal.Zero;

            var price = orderItem.OrderItemPrice as IPrice;
            return price.CalculateOneOffCost(orderItem.TotalQuantity)
                       + (price.CalculateCostPerMonth(orderItem.TotalQuantity) * term);
        }

        private static decimal TotalCost(this Order order)
        {
            return order.IsAmendment
                ? order.TotalCostByPlannedDelivery()
                : order.TotalCostForMaximumTerm();
        }

        private static decimal TotalCostByPlannedDelivery(this Order order)
        {
            return order.TotalOneOffCost() + order?.OrderItems.Sum(i =>
            {
                var term = GetTerm(order.EndDate, i);
                return ((IPrice)i.OrderItemPrice).CalculateCostPerMonth(i.TotalQuantity) * term;
            }) ?? decimal.Zero;
        }

        private static decimal TotalCostForMaximumTerm(this Order order)
        {
            var maximumTerm = order?.MaximumTerm ?? 36;

            return order.TotalOneOffCost() + (order.TotalMonthlyCost() * maximumTerm);
        }

        private static int GetTerm(EndDate endDate, OrderItem orderItem)
        {
            if (orderItem == null || endDate == null)
            {
                return 0;
            }

            var deliveryDate = orderItem.OrderItemRecipients.FirstOrDefault()?.DeliveryDate;
            if (deliveryDate.HasValue)
            {
                var term = endDate.RemainingTerm(deliveryDate.Value);
                return term;
            }

            return 0;
        }
    }
}
