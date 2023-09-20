﻿using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Calculations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.FundingSources
{
    public sealed class FundingSource : OrderingBaseModel
    {
        public FundingSource()
        {
        }

        public FundingSource(string internalOrgId, CallOffId callOffId, OrderWrapper orderWrapper, OrderItem orderItem)
        {
            Title = "Funding source";
            Caption = orderItem.CatalogueItem.Name;
            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
            CatalogueItemName = orderItem.CatalogueItem.Name;
            SelectedFundingType = orderItem.FundingType;
            TotalCost = orderWrapper.TotalCostForOrderItem(orderItem.CatalogueItem.Id);
        }

        public string CatalogueItemName { get; set; }

        public CallOffId CallOffId { get; set; }

        public OrderItemFundingType SelectedFundingType { get; set; } = OrderItemFundingType.None;

        public decimal TotalCost { get; set; }
    }
}
