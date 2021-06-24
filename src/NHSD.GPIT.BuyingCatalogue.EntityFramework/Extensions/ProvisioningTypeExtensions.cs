using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

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
                _ => throw new InvalidOperationException(),
            };
        }
    }
}
