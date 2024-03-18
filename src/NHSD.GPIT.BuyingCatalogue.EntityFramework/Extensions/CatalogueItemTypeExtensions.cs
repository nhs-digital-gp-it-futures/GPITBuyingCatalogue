using EnumsNET;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions
{
    public static class CatalogueItemTypeExtensions
    {
        public static string DisplayName(this CatalogueItemType itemType) => itemType.AsString(EnumFormat.DisplayName);
    }
}
