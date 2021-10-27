using EnumsNET;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions
{
    public static class SlaTypeExtensions
    {
        public static string Name(this SlaType slaType) => slaType.AsString(EnumFormat.DisplayName);

        public static string EnumMemberName(this SlaType slaType) => slaType.ToString();

        public static string Description(this SlaType slaType) => slaType.AsString(EnumFormat.Description);
    }
}
