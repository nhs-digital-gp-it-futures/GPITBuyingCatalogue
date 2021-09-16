using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public sealed class NativeDesktopMemoryAndStorage
    {
        public string MinimumCpu { get; set; }

        public string MinimumMemoryRequirement { get; set; }

        public string RecommendedResolution { get; set; }

        public string StorageRequirementsDescription { get; set; }

        public TaskProgress Status()
        {
            if (!string.IsNullOrWhiteSpace(MinimumMemoryRequirement)
                && !string.IsNullOrWhiteSpace(StorageRequirementsDescription)
                && !string.IsNullOrWhiteSpace(MinimumCpu))
            {
                return TaskProgress.Completed;
            }

            return TaskProgress.NotStarted;
        }
    }
}
