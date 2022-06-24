﻿using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed partial class OrderItem
    {
        public bool IsReadyForReview => (OrderItemRecipients?.Any() ?? false) && OrderItemPrice != null;

        public bool IsForcedFunding => FundingType.IsForcedFunding();

        public OrderItemFundingType FundingType => OrderItemFunding?.OrderItemFundingType ?? OrderItemFundingType.None;

        public string FundingTypeDescription
        {
            get
            {
                var itemType = CatalogueItem?.CatalogueItemType.DisplayName() ?? string.Empty;

                return FundingType switch
                {
                    OrderItemFundingType.None => $"Funding information has not been entered for this {itemType}.",
                    OrderItemFundingType.CentralFunding => $"This {itemType} is being paid for using central funding.",
                    OrderItemFundingType.LocalFunding => $"This {itemType} is being paid for using local funding.",
                    OrderItemFundingType.MixedFunding => $"This {itemType} is being paid for using a mix of central and local funding.",
                    OrderItemFundingType.NoFundingRequired => $"This {itemType} does not require funding.",
                    OrderItemFundingType.LocalFundingOnly => $"This {itemType} is being paid for using local funding.",
                    _ => throw new ArgumentOutOfRangeException(),
                };
            }
        }

        public int GetQuantity() => Quantity ?? OrderItemRecipients.Sum(oir => oir.Quantity ?? 0);
    }
}
