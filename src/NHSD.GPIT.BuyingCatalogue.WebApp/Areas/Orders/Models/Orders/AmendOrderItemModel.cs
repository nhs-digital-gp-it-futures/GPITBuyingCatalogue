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
        public AmendOrderItemModel(
            CallOffId callOffId,
            OrderType orderType,
            ICollection<OrderSublocationRecipient> recipients,
            ICollection<OrderSublocationRecipient> previousRecipients,
            OrderItem orderItem,
            OrderItem previous,
            bool isAmendment,
            FundingTypeDescriptionModel fundingTypeDescription)
        {
            ArgumentNullException.ThrowIfNull(orderItem);

            CallOffId = callOffId;
            OrderType = orderType;
            IsAmendment = isAmendment;
            IsOrderItemAdded = previous == null;
            OrderItem = orderItem;
            Previous = previous;
            FundingTypeDescriptionModel = fundingTypeDescription;
            RolledUpRecipientsForItem = recipients
                .ForCatalogueItem(orderItem.CatalogueItemId)
                .ToList();
            PreviousRecipientsForItem =
                (previousRecipients?.ForCatalogueItem(orderItem.CatalogueItemId) ?? []).ToList();
        }

        public CallOffId CallOffId { get; }

        public OrderType OrderType { get; }

        public bool IsAmendment { get; }

        public bool CanEdit { get; set; }

        public bool IsOrderItemAdded { get; }

        public OrderItemPrice OrderItemPrice => OrderItem.OrderItemPrice;

        public CatalogueItem CatalogueItem => OrderItem.CatalogueItem;

        public List<OrderSublocationRecipient> RolledUpRecipientsForItem { get; }

        public int RolledUpTotalQuantity => OrderItem.TotalQuantity(RolledUpRecipientsForItem);

        public int PreviousTotalQuantity => Previous?.TotalQuantity(PreviousRecipientsForItem) ?? 0;

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

        private List<OrderSublocationRecipient> PreviousRecipientsForItem { get; }

        private OrderItem OrderItem { get; }

        private OrderItem Previous { get; }

        private FundingTypeDescriptionModel FundingTypeDescriptionModel { get; }

        public bool IsServiceRecipientAdded(string odsCode)
        {
            OrderSublocationRecipient rolledUpRecipient =
                RolledUpRecipientsForItem.FirstOrDefault(x => x.RecipientOdsCode == odsCode);
            OrderSublocationRecipient previousRecipient =
                PreviousRecipientsForItem.FirstOrDefault(x => x.RecipientOdsCode == odsCode);

            return (rolledUpRecipient != null && previousRecipient == null)
                || Previous == null;
        }
    }
}
