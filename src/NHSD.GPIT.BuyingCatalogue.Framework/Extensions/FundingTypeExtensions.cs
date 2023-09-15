using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

public static class FundingTypeExtensions
{
    public static OrderItemFundingType AsOrderItemFundingType(this FundingType fundingType) => fundingType switch
    {
        FundingType.Gpit => OrderItemFundingType.Gpit,
        FundingType.Pcarp => OrderItemFundingType.Pcarp,
        _ or FundingType.LocalFunding => OrderItemFundingType.LocalFunding,
    };
}
