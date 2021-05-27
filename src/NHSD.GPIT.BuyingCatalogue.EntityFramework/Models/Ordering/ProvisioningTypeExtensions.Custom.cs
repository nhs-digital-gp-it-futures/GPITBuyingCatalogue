using System;
using static NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering.TimeUnit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering
{
    public static class ProvisioningTypeExtensions
    {
        public static TimeUnit? InferEstimationPeriod(this ProvisioningType provisioningType, TimeUnit? estimationPeriod)
        {
            return provisioningType switch
            {
                ProvisioningType.Patient => PerMonth,
                ProvisioningType.Declarative => PerYear,
                ProvisioningType.OnDemand => estimationPeriod,
                _ => throw new InvalidOperationException(),
            };
        }
    }
}
