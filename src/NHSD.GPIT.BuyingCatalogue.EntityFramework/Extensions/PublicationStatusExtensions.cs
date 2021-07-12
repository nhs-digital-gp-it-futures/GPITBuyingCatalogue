using EnumsNET;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions
{
    public static class PublicationStatusExtensions
    {
        public static string Name(this PublicationStatus publicationStatus) => publicationStatus.AsString(EnumFormat.DisplayName);

        public static string EnumMemberName(this PublicationStatus publicationStatus) => publicationStatus.ToString();
    }
}
