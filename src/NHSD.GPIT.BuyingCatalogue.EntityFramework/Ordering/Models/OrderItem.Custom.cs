using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed partial class OrderItem
    {
        public OrderItemFundingType FundingType => OrderItemFunding?.OrderItemFundingType ?? OrderItemFundingType.None;

        public int TotalQuantity(ICollection<OrderSublocationRecipient> recipients)
        {
            if (OrderItemPrice == null)
                return 0;

            return Quantity.HasValue
                ? Quantity ?? 0
                : recipients?.Sum(r => r.GetQuantityForItem(CatalogueItemId) ?? 0) ?? 0;
        }

        public bool IsReadyForReview(bool isAmendment, ICollection<OrderSublocationRecipient> recipients)
        {
            return (isAmendment && recipients.Count == 0)
                || (OrderItemPrice != null
                    && TotalQuantity(recipients) > 0
                    && (!isAmendment || recipients.AllDeliveryDatesEntered(CatalogueItemId)));
        }
    }
}
