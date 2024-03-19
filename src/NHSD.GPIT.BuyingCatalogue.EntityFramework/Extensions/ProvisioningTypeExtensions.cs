using EnumsNET;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions
{
    public static class ProvisioningTypeExtensions
    {
        public static string Name(this ProvisioningType provisioningType) => provisioningType.AsString(EnumFormat.DisplayName);

        public static bool IsPerServiceRecipient(this ProvisioningType provisioningType)
        {
            return provisioningType is ProvisioningType.Patient;
        }
    }
}
