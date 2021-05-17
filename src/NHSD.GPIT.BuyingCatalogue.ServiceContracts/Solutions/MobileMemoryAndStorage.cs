using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    [ExcludeFromCodeCoverage]
    public class MobileMemoryAndStorage
    {
        public string Description { get; set; }

        public string MinimumMemoryRequirement { get; set; }

        public virtual bool IsValid() =>
            !string.IsNullOrWhiteSpace(Description) &&
            !string.IsNullOrWhiteSpace(MinimumMemoryRequirement);
    }
}
