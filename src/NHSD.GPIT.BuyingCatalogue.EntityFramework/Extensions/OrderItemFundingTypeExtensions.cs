using EnumsNET;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions
{
    public static class OrderItemFundingTypeExtensions
    {
        public static string Description(this OrderItemFundingType publicationStatus) => publicationStatus.AsString(EnumFormat.Description);
    }
}
