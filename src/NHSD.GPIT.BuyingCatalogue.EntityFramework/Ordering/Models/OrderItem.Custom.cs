using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed partial class OrderItem
    {
        public int GetQuantity() => Quantity ?? OrderItemRecipients.ToList().Sum(oir => oir.Quantity ?? 0);

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

        public DateTime? GetCentralAllocationEndDate(decimal totalCost)
        {
            if (CurrentFundingType() is not OrderItemFundingType.MixedFunding)
                return null;

            var startDate = Order.CommencementDate!.Value;
            var endDate = startDate.AddMonths(Order.MaximumTerm!.Value);
            var totalDays = endDate.Subtract(startDate).TotalDays;

            var centralAllocation = OrderItemFunding.CentralAllocation;
            var percentage = (double)Math.Round(centralAllocation / totalCost, 2);
            var estimatedDays = percentage * totalDays;

            return startDate.AddDays(estimatedDays);
        }
    }
}
