using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public sealed class MobileMemoryAndStorage
    {
        public string Description { get; set; }

        public string MinimumMemoryRequirement { get; set; }

        public TaskProgress Status()
        {
            if (!string.IsNullOrWhiteSpace(Description) && !string.IsNullOrWhiteSpace(MinimumMemoryRequirement))
                return TaskProgress.Completed;

            return TaskProgress.NotStarted;
        }
    }
}
