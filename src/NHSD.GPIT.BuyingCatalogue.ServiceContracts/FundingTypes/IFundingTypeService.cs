using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.FundingTypes
{
    public interface IFundingTypeService
    {
        OrderItemFundingType GetFundingType(List<OrderItemFundingType> allFundingTypes, OrderItemFundingType fundingType);
    }
}
