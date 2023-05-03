using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders
{
    public class AmendOrderItemModel
    {
        public AmendOrderItemModel(CallOffId callOffId, OrderItem orderItem, OrderItem previous, bool isAmendment, FundingTypeDescriptionModel fundingTypeDescription)
        {
            CallOffId = callOffId;
            IsAmendment = isAmendment;
            IsOrderItemAdded = orderItem != null && previous == null;
            OrderItem = orderItem;
            Previous = previous;
            FundingTypeDescriptionModel = fundingTypeDescription;
        }

        public CallOffId CallOffId { get; }

        public bool IsAmendment { get; }

        public bool IsOrderItemAdded { get; }

        public OrderItemPrice OrderItemPrice => OrderItem.OrderItemPrice;

        public CatalogueItem CatalogueItem => OrderItem.CatalogueItem;

        public ICollection<OrderItemRecipient> OrderItemRecipients => OrderItem.OrderItemRecipients;

        public int RolledUpTotalQuantity => OrderItem.TotalQuantity;

        public int PreviousTotalQuantity => Previous?.TotalQuantity ?? 0;

        public string FundingTypeDescription
        {
            get
            {
                var itemType = CatalogueItem?.CatalogueItemType.DisplayName() ?? string.Empty;
                return FundingTypeDescriptionModel.Value(itemType);
            }
        }

        private OrderItem OrderItem { get; }

        private OrderItem Previous { get; }

        private FundingTypeDescriptionModel FundingTypeDescriptionModel { get; }

        public bool IsServiceRecipientAdded(string odsCode) =>
            OrderItem?.OrderItemRecipients?.FirstOrDefault(x => x.OdsCode == odsCode) != null
            && Previous?.OrderItemRecipients?.FirstOrDefault(x => x.OdsCode == odsCode) == null;
    }
}
