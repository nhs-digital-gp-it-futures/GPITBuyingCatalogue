using EnumsNET;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions
{
    public static class SolutionTypeExtensions
    {
        public static string DisplayName(this SolutionType itemType) => itemType.AsString(EnumFormat.DisplayName);
    }
}
