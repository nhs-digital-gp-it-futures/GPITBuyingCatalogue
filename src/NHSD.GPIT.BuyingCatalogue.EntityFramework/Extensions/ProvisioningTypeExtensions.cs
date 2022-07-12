using System;
using EnumsNET;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions
{
    public static class ProvisioningTypeExtensions
    {
        public static TimeUnit? InferEstimationPeriod(this ProvisioningType provisioningType, TimeUnit? estimationPeriod)
        {
            return provisioningType switch
            {
                ProvisioningType.Declarative => TimeUnit.PerYear,
                ProvisioningType.OnDemand => estimationPeriod,
                ProvisioningType.Patient => TimeUnit.PerMonth,
                ProvisioningType.PerServiceRecipient => TimeUnit.PerMonth,
                _ => throw new ArgumentOutOfRangeException(nameof(provisioningType)),
            };
        }

        public static string Name(this ProvisioningType provisioningType) => provisioningType.AsString(EnumFormat.DisplayName);

        public static bool IsPerServiceRecipient(this ProvisioningType provisioningType)
        {
            return provisioningType is ProvisioningType.Patient or ProvisioningType.PerServiceRecipient;
        }
    }
}
