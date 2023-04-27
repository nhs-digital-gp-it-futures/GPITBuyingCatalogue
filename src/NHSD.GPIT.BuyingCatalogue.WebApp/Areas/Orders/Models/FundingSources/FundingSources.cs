using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Calculations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.FundingSources
{
    public sealed class FundingSources : OrderingBaseModel
    {
        public FundingSources()
        {
        }

        public FundingSources(string internalOrgId, CallOffId callOffId, Order order, int countOfOrderFrameworks)
        {
            if (order is null || order.OrderingParty is null || order.SelectedFramework is null)
                throw new ArgumentNullException(nameof(order));

            Order = order;
            Title = "Funding sources";
            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
            Caption = $"Order {CallOffId}";
            CountOfOrderFrameworks = countOfOrderFrameworks;

            SelectedFramework = order.SelectedFramework;

            var completedOrderItems = order.OrderItems.Where(oi => oi.AllQuantitiesEntered).ToList();

            if (order.IsLocalFundingOnly)
            {
                OrderItemsLocalOnly = completedOrderItems.Where(oi => oi.OrderItemPrice.CostForBillingPeriod(oi.TotalQuantity) != 0).ToList();
            }
            else
            {
                OrderItemsSelectable = completedOrderItems.Where(oi => oi.OrderItemPrice.CostForBillingPeriod(oi.TotalQuantity) != 0).ToList();
            }

            OrderItemsNoFundingRequired = completedOrderItems.Where(oi => oi.OrderItemPrice.CostForBillingPeriod(oi.TotalQuantity) == 0).ToList();
        }

        public Order Order { get; set; }

        public CallOffId CallOffId { get; set; }

        public List<OrderItem> OrderItemsSelectable { get; set; }

        public List<OrderItem> OrderItemsLocalOnly { get; set; }

        public List<OrderItem> OrderItemsNoFundingRequired { get; set; }

        public EntityFramework.Catalogue.Models.Framework SelectedFramework { get; set; }

        public int CountOfOrderFrameworks { get; set; }
    }
}
