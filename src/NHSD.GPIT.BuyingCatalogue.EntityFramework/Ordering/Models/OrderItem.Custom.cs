using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed partial class OrderItem
    {
        public int GetTotalRecipientQuantity() => OrderItemRecipients.ToList().Sum(oir => oir.Quantity ?? 0);

        public OrderItemFundingType CurrentFundingType()
        {
            if (OrderItemFunding is null)
                return OrderItemFundingType.None;

            return OrderItemFunding.CentralAllocation switch
            {
                var c when c > 0 && OrderItemFunding.LocalAllocation == 0 => OrderItemFundingType.CentralFunding,
                var c when c == 0 && OrderItemFunding.LocalAllocation > 0 => OrderItemFundingType.LocalFunding,
                _ => OrderItemFundingType.MixedFunding,
            };
        }

        public bool ItemIsLocalFundingOnly() => CatalogueItem?.Solution?.FrameworkSolutions?.All(fs => fs.Framework.LocalFundingOnly) ?? false;
    }
}
