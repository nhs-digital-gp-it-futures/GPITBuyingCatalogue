using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed partial class OrderItem
    {
        public int GetQuantity() => Quantity ?? OrderItemRecipients.ToList().Sum(oir => oir.Quantity ?? 0);

        public OrderItemFundingType CurrentFundingType()
        {
            if (OrderItemFunding is null)
                return OrderItemFundingType.None;

            return OrderItemFunding.OrderItemFundingType;
        }

        public bool IsCurrentlyForcedFunding() => CurrentFundingType() is OrderItemFundingType.NoFundingRequired or OrderItemFundingType.LocalFundingOnly;
    }
}
