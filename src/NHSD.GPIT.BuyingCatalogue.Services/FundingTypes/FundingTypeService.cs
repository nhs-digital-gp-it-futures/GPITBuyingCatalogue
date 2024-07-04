using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.FundingTypes;

namespace NHSD.GPIT.BuyingCatalogue.Services.FundingTypes
{
    public class FundingTypeService : IFundingTypeService
    {
        public OrderItemFundingType GetFundingType(List<OrderItemFundingType> allFundingTypes, OrderItemFundingType fundingType)
        {
            if (allFundingTypes == null)
            {
                throw new ArgumentNullException(nameof(allFundingTypes));
            }

            if (Enum.IsDefined(typeof(FundingType), fundingType.ToString()) || fundingType == OrderItemFundingType.CentralFunding)
                return fundingType;

            return fundingType switch
            {
                OrderItemFundingType.LocalFundingOnly => OrderItemFundingType.LocalFunding,
                OrderItemFundingType.MixedFunding => GetDefault(allFundingTypes),
                _ => GetNoFunding(allFundingTypes),
            };
        }

        private static OrderItemFundingType GetDefault(List<OrderItemFundingType> allFundingTypes)
        {
            var containsCentral = allFundingTypes.Contains(OrderItemFundingType.CentralFunding);
            var containsLocal = allFundingTypes.Contains(OrderItemFundingType.LocalFunding) || allFundingTypes.Contains(OrderItemFundingType.LocalFundingOnly);

            return (!containsCentral && containsLocal) ? OrderItemFundingType.LocalFunding : OrderItemFundingType.CentralFunding;
        }

        private static OrderItemFundingType GetNoFunding(List<OrderItemFundingType> allFundingTypes)
        {
            var containsCentral = allFundingTypes.Contains(OrderItemFundingType.CentralFunding);
            var containsMixed = allFundingTypes.Contains(OrderItemFundingType.MixedFunding);

            // If order contains deprecated Central and Mixed funding types then can assume it is an old order.
            // In all other cases set to None
            return (containsCentral || containsMixed) ? GetDefault(allFundingTypes) : OrderItemFundingType.None;
        }
    }
}
