﻿using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Calculations;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.FundingSources
{
    public sealed class FundingSource : OrderingBaseModel
    {
        public FundingSource()
        {
        }

        public FundingSource(string internalOrgId, CallOffId callOffId, EntityFramework.Ordering.Models.Order order, OrderItem orderItem)
        {
            Title = $"{orderItem.CatalogueItem.Name} funding source";
            Caption = $"Order {callOffId}";
            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
            CatalogueItemName = orderItem.CatalogueItem.Name;
            SelectedFundingType = orderItem.FundingType;
            TotalCost = orderItem.OrderItemPrice.CalculateTotalCostForContractLength(orderItem.TotalQuantity, order.MaximumTerm!.Value);
        }

        public string CatalogueItemName { get; set; }

        public string Caption { get; set; }

        public CallOffId CallOffId { get; set; }

        public OrderItemFundingType SelectedFundingType { get; set; } = OrderItemFundingType.None;

        public decimal TotalCost { get; set; }
    }
}
