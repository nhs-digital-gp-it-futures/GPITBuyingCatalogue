using System;
using static NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering.TimeUnit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering
{
    public partial class ProvisioningType
    {
        public TimeUnit InferEstimationPeriod(TimeUnit estimationPeriod)
        {
            return Name switch
            {
                "Patient" => GetTimeUnitByType(TimeUnitType.PerMonth),
                "Declarative" => GetTimeUnitByType(TimeUnitType.PerYear),
                "On Demand" => estimationPeriod,
                _ => throw new InvalidOperationException(),
            };
        }
    }
}
