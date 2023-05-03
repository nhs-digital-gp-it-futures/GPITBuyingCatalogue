using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders
{
    public class FundingTypeDescriptionModel
    {
        public FundingTypeDescriptionModel(IEnumerable<OrderItemFundingType> fundingTypes)
        {
            FundingTypes = fundingTypes;
        }

        private IEnumerable<OrderItemFundingType> FundingTypes { get; }

        public string Value(string itemType)
        {
            if (new[] { OrderItemFundingType.MixedFunding }.All(f => FundingTypes.Contains(f))
                || new[] { OrderItemFundingType.LocalFunding, OrderItemFundingType.CentralFunding }.All(f => FundingTypes.Contains(f))
                || new[] { OrderItemFundingType.LocalFundingOnly, OrderItemFundingType.CentralFunding }.All(f => FundingTypes.Contains(f)))
            {
                return $"This {itemType} is being paid for using a combination of central and local funding.";
            }

            if (new[] { OrderItemFundingType.LocalFunding, OrderItemFundingType.LocalFundingOnly }.Any(f => FundingTypes.Contains(f)))
            {
                return $"This {itemType} is being paid for using local funding.";
            }

            if (FundingTypes.Contains(OrderItemFundingType.CentralFunding))
            {
                return $"This {itemType} is being paid for using central funding.";
            }

            if (FundingTypes.Contains(OrderItemFundingType.NoFundingRequired))
            {
                return $"This {itemType} does not require funding.";
            }

            if (FundingTypes.Contains(OrderItemFundingType.None))
            {
                return $"Funding information has not been entered for this {itemType}.";
            }

            throw new InvalidOperationException($"Unhandled funding type(s) {string.Join(", ", FundingTypes)}");
        }
    }
}
