using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    [ExcludeFromCodeCoverage]
    public class NativeDesktopMemoryAndStorage
    {
        public string MinimumMemoryRequirement { get; set; }

        public string StorageRequirementsDescription { get; set; }

        public string MinimumCpu { get; set; }

        public string RecommendedResolution { get; set; }
    }
}
