using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed partial class OrderItem
    {
        public bool IsReadyForReview => (OrderItemRecipients?.Any() ?? false) && OrderItemPrice != null;

        public bool IsForcedFunding => FundingType.IsForcedFunding();

        public OrderItemFundingType FundingType => OrderItemFunding?.OrderItemFundingType ?? OrderItemFundingType.None;

        public int GetQuantity() => Quantity ?? OrderItemRecipients.Sum(oir => oir.Quantity ?? 0);
    }
}
