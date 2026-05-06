using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

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

            this.previousRecipients = (previousRecipients?.ForCatalogueItem(orderItem.CatalogueItemId) ?? []).ToDictionary(
                    x => x.RecipientOdsCode,
                    x => x);
        }

        public CallOffId CallOffId { get; init; }

        public OrderWrapper OrderWrapper { get; init; }

        public OrderType OrderType { get; }

        public bool IsAmendment => CallOffId.IsAmendment;

        public bool CanEdit { get; set; }

        public bool IsOrderItemAdded { get; }

        public string ItemName { get; init; }

        public bool IsCardView { get; set; }

        public bool FromPreviousRevision { get; init; }

        /// <summary>
        /// Gets a value indicating whether the price should be shown for the order item.
        /// </summary>
        /// <remarks>
        /// The price will be shown in the following scenarios:<br/>
        /// * The item is an Associated Service.<br/>
        /// * The order is an original order with any price type.<br/>
        /// * The order is an amendment, and an adjustment has been made to the quantity of an existing item which is not using a single-fixed price.<br/>
        /// * The order is an amendment, and the item did not exist on a previous amendment.<br/>
        /// </remarks>
        public bool ShouldShowPrice => OrderItemPrice is { PriceTiers.Count: > 0 }
            && (CatalogueItem is { CatalogueItemType: CatalogueItemType.AssociatedService }
                || !(OrderItemPrice is { CataloguePriceCalculationType: CataloguePriceCalculationType.SingleFixed }
                    && IsAmendment
                    && !IsOrderItemAdded));

        public OrderItemPrice OrderItemPrice => OrderItem.OrderItemPrice;

        public CatalogueItem CatalogueItem => OrderItem.CatalogueItem;

        public ICollection<OrderSublocationRecipient> RolledUpRecipientsForItem => rolledUpRecipients.Values;

        public ICollection<OrderSublocationRecipient> PreviousRecipientsForItem => previousRecipients.Values;

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

        public OrderItem OrderItem { get; }

        public OrderItem Previous { get; }

        private FundingTypeDescriptionModel FundingTypeDescriptionModel { get; }

        public bool IsServiceRecipientAdded(string odsCode) =>
            (rolledUpRecipients.ContainsKey(odsCode) && !previousRecipients.ContainsKey(odsCode))
            || IsOrderItemAdded;
    }
}
