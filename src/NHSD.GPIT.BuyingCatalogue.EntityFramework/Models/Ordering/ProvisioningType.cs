using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering
{
    public class ProvisioningType : EnumerationBase
    {
        public static readonly ProvisioningType Patient = new(1, "Patient");
        public static readonly ProvisioningType Declarative = new(2, "Declarative");
        public static readonly ProvisioningType OnDemand = new(3, "On Demand");

        public ProvisioningType(int id, string name)
            : base(id, name)
        {
        }

        public TimeUnit InferEstimationPeriod(TimeUnit estimationPeriod)
        {
            if (Id == Patient.Id)
                return TimeUnit.PerMonth;
            else if (Id == Declarative.Id)
                return TimeUnit.PerYear;
            else if (Id == OnDemand.Id)
                return estimationPeriod;
            else
                throw new InvalidOperationException();
        }
    }
}
