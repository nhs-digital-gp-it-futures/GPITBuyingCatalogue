using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed partial class OrderItem
    {
        public OrderItemFundingType FundingType => OrderItemFunding?.OrderItemFundingType ?? OrderItemFundingType.None;

        public int TotalQuantity(ICollection<OrderRecipient> recipients)
        {
            if (OrderItemPrice == null)
                return 0;

            return ((IPrice)OrderItemPrice).IsPerServiceRecipient()
                ? recipients?.Sum(r => r.GetQuantityForItem(CatalogueItemId) ?? 0) ?? 0
                : Quantity ?? 0;
        }

        public bool IsReadyForReview(bool isAmendment, ICollection<OrderRecipient> recipients) =>
            (isAmendment && !recipients.Any())
            || (OrderItemPrice != null
            && TotalQuantity(recipients) > 0
            && (!isAmendment || recipients.AllDeliveryDatesEntered(CatalogueItemId)));
    }
}
