using System;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering
{
    public class ProvisioningType
        : EnumerationBase
    {
        public static readonly ProvisioningType Patient = new(1, "Patient");
        public static readonly ProvisioningType Declarative = new(2, "Declarative");
        public static readonly ProvisioningType OnDemand = new(3, "On Demand");

        public ProvisioningType(int id, string name)
            : base(id, name)
        {
        }

        public static ProvisioningType Parse(string name)
        {
            if (name.Equals(nameof(Patient), System.StringComparison.InvariantCultureIgnoreCase))
                return Patient;
            else if (name.Equals(nameof(Declarative), System.StringComparison.InvariantCultureIgnoreCase))
                return Declarative;
            else if (name.Equals(nameof(OnDemand), System.StringComparison.InvariantCultureIgnoreCase))
                return OnDemand;

            throw new ArgumentException("Invalid ProvisioningType", nameof(name));
        }

        public TimeUnit InferEstimationPeriod(TimeUnit estimationPeriod)
        {
            if (this == Patient)
                return TimeUnit.PerMonth;
            else if (this == Declarative)
                return TimeUnit.PerYear;
            else if (this == OnDemand)
                return estimationPeriod;
            else
                throw new InvalidOperationException();
        }
    }
}
