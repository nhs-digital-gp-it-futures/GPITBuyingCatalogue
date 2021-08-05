﻿namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public sealed class NativeDesktopMemoryAndStorage
    {
        public string MinimumCpu { get; set; }

        public string MinimumMemoryRequirement { get; set; }

        public string RecommendedResolution { get; set; }

        public string StorageRequirementsDescription { get; set; }

        public bool IsValid() =>
            !string.IsNullOrWhiteSpace(MinimumMemoryRequirement)
            && !string.IsNullOrWhiteSpace(StorageRequirementsDescription)
            && !string.IsNullOrWhiteSpace(MinimumCpu);
    }
}
