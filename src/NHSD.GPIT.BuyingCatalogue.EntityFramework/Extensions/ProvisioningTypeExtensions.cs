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
                ProvisioningType.Patient => TimeUnit.PerMonth,
                ProvisioningType.Declarative => TimeUnit.PerYear,
                ProvisioningType.OnDemand => estimationPeriod,
                _ => throw new ArgumentOutOfRangeException(nameof(provisioningType)),
            };
        }

        public static string Name(this ProvisioningType provisioningType) => provisioningType.AsString(EnumFormat.DisplayName);
    }
}
