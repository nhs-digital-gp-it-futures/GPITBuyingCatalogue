using EnumsNET;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions
{
    public static class OrderItemFundingTypeExtensions
    {
        public static string Name(this OrderItemFundingType publicationStatus) => publicationStatus.AsString(EnumFormat.DisplayName);

        public static string EnumMemberName(this OrderItemFundingType publicationStatus) => publicationStatus.ToString();

        public static string Description(this OrderItemFundingType publicationStatus) => publicationStatus.AsString(EnumFormat.Description);

        public static bool IsForcedFunding(this OrderItemFundingType fundingType) =>
            fundingType is OrderItemFundingType.LocalFundingOnly or OrderItemFundingType.NoFundingRequired;
    }
}
