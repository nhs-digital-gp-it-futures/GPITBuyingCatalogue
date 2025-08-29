using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders
{
    public class AmendOrderItemModel : OrderingBaseModel
    {
        private readonly Dictionary<string, OrderSublocationRecipient> rolledUpRecipients;
        private readonly Dictionary<string, OrderSublocationRecipient> previousRecipients;

        public AmendOrderItemModel(
            CallOffId callOffId,
            OrderType orderType,
            ICollection<OrderSublocationRecipient> recipients,
            ICollection<OrderSublocationRecipient> previousRecipients,
            OrderItem orderItem,
            OrderItem previous,
            FundingTypeDescriptionModel fundingTypeDescription)
        {
            ArgumentNullException.ThrowIfNull(orderItem);

            CallOffId = callOffId;
            OrderType = orderType;
            IsOrderItemAdded = previous == null;
            OrderItem = orderItem;
            Previous = previous;
            FundingTypeDescriptionModel = fundingTypeDescription;
            rolledUpRecipients = recipients
                .ForCatalogueItem(orderItem.CatalogueItemId)
                .ToDictionary(
                    x => x.RecipientOdsCode,
                    x => x);

            this.previousRecipients =
                (previousRecipients?.ForCatalogueItem(orderItem.CatalogueItemId) ?? []).ToDictionary(
                    x => x.RecipientOdsCode,
                    x => x);
        }

        public CallOffId CallOffId { get; }

        public OrderType OrderType { get; }

        public bool IsAmendment => CallOffId.IsAmendment;

        public bool CanEdit { get; set; }

        public bool IsOrderItemAdded { get; }

        public OrderItemPrice OrderItemPrice => OrderItem.OrderItemPrice;

        public CatalogueItem CatalogueItem => OrderItem.CatalogueItem;

        public ICollection<OrderSublocationRecipient> RolledUpRecipientsForItem => rolledUpRecipients.Values;

        public int RolledUpTotalQuantity => OrderItem.TotalQuantity(RolledUpRecipientsForItem);

        public int PreviousTotalQuantity => Previous?.TotalQuantity(previousRecipients.Values) ?? 0;

        public string FundingTypeDescription
        {
            get
            {
                var itemType = CatalogueItem?.CatalogueItemType.DisplayName() ?? string.Empty;
                return FundingTypeDescriptionModel.Value(itemType);
            }
        }

        public OrderTotalModel OrderTotals { get; set; }

        public string PracticeReorganisationName { get; set; }

        private OrderItem OrderItem { get; }

        private OrderItem Previous { get; }

        private FundingTypeDescriptionModel FundingTypeDescriptionModel { get; }

        public bool IsServiceRecipientAdded(string odsCode) =>
            (rolledUpRecipients.ContainsKey(odsCode) && !previousRecipients.ContainsKey(odsCode))
            || IsOrderItemAdded;
    }
}
