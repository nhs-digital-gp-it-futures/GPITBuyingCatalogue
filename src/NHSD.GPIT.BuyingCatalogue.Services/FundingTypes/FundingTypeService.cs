using System;
using System.Collections.Generic;
using EnumsNET;
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
                OrderItemFundingType.MixedFunding => GetMixed(allFundingTypes),
                _ => OrderItemFundingType.None,
            };
        }

        private static OrderItemFundingType GetMixed(List<OrderItemFundingType> allFundingTypes)
        {
            var containsCentral = allFundingTypes.Contains(OrderItemFundingType.CentralFunding);
            var containsLocal = allFundingTypes.Contains(OrderItemFundingType.LocalFunding) || allFundingTypes.Contains(OrderItemFundingType.LocalFundingOnly);

            return (!containsCentral && containsLocal) ? OrderItemFundingType.LocalFunding : OrderItemFundingType.CentralFunding;
        }
}
}
