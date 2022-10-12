using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.FundingTypes;

namespace NHSD.GPIT.BuyingCatalogue.Services.FundingTypes
{
    public class FundingTypeService : IFundingTypeService
    {
        public OrderItemFundingType GetFundingType(List<OrderItemFundingType> allFundingTypes, OrderItemFundingType current)
        {
            if (allFundingTypes == null)
            {
                throw new ArgumentNullException(nameof(allFundingTypes));
            }

            var containsCentral = allFundingTypes.Contains(OrderItemFundingType.CentralFunding);
            var containsLocal = allFundingTypes.Contains(OrderItemFundingType.LocalFunding) || allFundingTypes.Contains(OrderItemFundingType.LocalFundingOnly);

            if (containsCentral)
            {
                return DefaultToCentral(current);
            }

            return containsLocal
                ? DefaultToLocal(current)
                : DefaultToCentral(current);
        }

        private static OrderItemFundingType DefaultToCentral(OrderItemFundingType input)
        {
            var output = input switch
            {
                OrderItemFundingType.CentralFunding => OrderItemFundingType.CentralFunding,
                OrderItemFundingType.LocalFunding => OrderItemFundingType.LocalFunding,
                OrderItemFundingType.LocalFundingOnly => OrderItemFundingType.LocalFunding,
                OrderItemFundingType.MixedFunding => OrderItemFundingType.CentralFunding,
                OrderItemFundingType.NoFundingRequired => OrderItemFundingType.CentralFunding,
                OrderItemFundingType.None => OrderItemFundingType.CentralFunding,
                _ => throw new ArgumentOutOfRangeException(nameof(input), input, null),
            };

            return output;
        }

        private static OrderItemFundingType DefaultToLocal(OrderItemFundingType input)
        {
            var output = input switch
            {
                OrderItemFundingType.CentralFunding => OrderItemFundingType.CentralFunding,
                OrderItemFundingType.LocalFunding => OrderItemFundingType.LocalFunding,
                OrderItemFundingType.LocalFundingOnly => OrderItemFundingType.LocalFunding,
                OrderItemFundingType.MixedFunding => OrderItemFundingType.LocalFunding,
                OrderItemFundingType.NoFundingRequired => OrderItemFundingType.LocalFunding,
                OrderItemFundingType.None => OrderItemFundingType.LocalFunding,
                _ => throw new ArgumentOutOfRangeException(nameof(input), input, null),
            };

            return output;
        }
    }
}
