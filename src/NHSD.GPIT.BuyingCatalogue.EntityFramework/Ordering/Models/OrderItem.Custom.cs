using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed partial class OrderItem
    {
        public OrderItemFundingType FundingType => OrderItemFunding?.OrderItemFundingType ?? OrderItemFundingType.None;

        public int TotalQuantity
        {
            get
            {
                if (OrderItemPrice == null)
                    return 0;

                return ((IPrice)OrderItemPrice).IsPerServiceRecipient()
                    ? OrderItemRecipients?.Sum(x => x.Quantity ?? 0) ?? 0
                    : Quantity ?? 0;
            }
        }

        public bool AllDeliveryDatesEntered => (OrderItemRecipients?.Any() ?? false) && OrderItemRecipients.All(x => x.DeliveryDate != null);

        public bool AllQuantitiesEntered
        {
            get
            {
                if (OrderItemPrice == null)
                    return false;

                return ((IPrice)OrderItemPrice).IsPerServiceRecipient()
                    ? OrderItemRecipients?.All(x => x.Quantity.HasValue) ?? false
                    : Quantity.HasValue;
            }
        }

        public bool IsReadyForReview(bool isAmendment) =>
            (OrderItemRecipients?.Any() ?? false)
            && OrderItemPrice != null
            && TotalQuantity > 0
            && (!isAmendment || OrderItemRecipients.All(x => x.DeliveryDate != null));
    }
}
