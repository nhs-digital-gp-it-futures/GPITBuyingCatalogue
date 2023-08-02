using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders
{
    public class AmendOrderItemModel
    {
        public AmendOrderItemModel(
            CallOffId callOffId,
            IEnumerable<OrderRecipient> recipients,
            IEnumerable<OrderRecipient> previousRecipients,
            OrderItem orderItem,
            OrderItem previous,
            bool isAmendment,
            FundingTypeDescriptionModel fundingTypeDescription)
        {
            CallOffId = callOffId;
            IsAmendment = isAmendment;
            IsOrderItemAdded = orderItem != null && previous == null;
            OrderItem = orderItem;
            Previous = previous;
            FundingTypeDescriptionModel = fundingTypeDescription;
            Recipients = recipients.ToList();
            PreviousRecipients = (previousRecipients ?? Enumerable.Empty<OrderRecipient>()).ToList();
        }

        public CallOffId CallOffId { get; }

        public bool IsAmendment { get; }

        public bool IsOrderItemAdded { get; }

        public OrderItemPrice OrderItemPrice => OrderItem.OrderItemPrice;

        public CatalogueItem CatalogueItem => OrderItem.CatalogueItem;

        public List<OrderRecipient> Recipients { get; }

        public List<OrderRecipient> PreviousRecipients { get; }

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
            Recipients?.FirstOrDefault(x => x.OdsCode == odsCode) != null
            && PreviousRecipients?.FirstOrDefault(x => x.OdsCode == odsCode) == null;
    }
}
