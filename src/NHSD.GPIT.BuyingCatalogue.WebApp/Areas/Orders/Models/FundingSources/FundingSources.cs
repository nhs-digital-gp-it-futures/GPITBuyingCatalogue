﻿using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Calculations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.FundingSources
{
    public sealed class FundingSources : OrderingBaseModel
    {
        public FundingSources()
        {
        }

        public FundingSources(string internalOrgId, CallOffId callOffId, OrderWrapper orderWrapper, int countOfOrderFrameworks)
        {
            ArgumentNullException.ThrowIfNull(orderWrapper);
            var order = orderWrapper.Order ?? throw new ArgumentNullException(nameof(orderWrapper));

            if (order is null || order.OrderingParty is null || order.SelectedFramework is null)
                throw new ArgumentNullException(nameof(orderWrapper));

            OrderWrapper = orderWrapper;
            Order = order;
            Title = "Funding sources";
            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
            Caption = $"Order {CallOffId}";
            CountOfOrderFrameworks = countOfOrderFrameworks;

            SelectedFramework = order.SelectedFramework;

            var completedOrderItems = order.OrderItems.Where(oi => orderWrapper.DetermineOrderRecipients(oi.CatalogueItemId).AllQuantitiesEntered(oi)).ToList();

            if (order.IsLocalFundingOnly)
            {
                OrderItemsLocalOnly = completedOrderItems.Where(oi => oi.OrderItemPrice.CostForBillingPeriod(oi.TotalQuantity(orderWrapper.DetermineOrderRecipients(oi.CatalogueItemId))) != 0).ToList();
            }
            else
            {
                OrderItemsSelectable = completedOrderItems.Where(oi => oi.OrderItemPrice.CostForBillingPeriod(oi.TotalQuantity(orderWrapper.DetermineOrderRecipients(oi.CatalogueItemId))) != 0).ToList();
            }

            OrderItemsNoFundingRequired = completedOrderItems.Where(oi => oi.OrderItemPrice.CostForBillingPeriod(oi.TotalQuantity(orderWrapper.DetermineOrderRecipients(oi.CatalogueItemId))) == 0).ToList();
        }

        public Order Order { get; set; }

        public OrderWrapper OrderWrapper { get; set; }

        public CallOffId CallOffId { get; set; }

        public List<OrderItem> OrderItemsSelectable { get; set; }

        public List<OrderItem> OrderItemsLocalOnly { get; set; }

        public List<OrderItem> OrderItemsNoFundingRequired { get; set; }

        public EntityFramework.Catalogue.Models.Framework SelectedFramework { get; set; }

        public int CountOfOrderFrameworks { get; set; }
    }
}
