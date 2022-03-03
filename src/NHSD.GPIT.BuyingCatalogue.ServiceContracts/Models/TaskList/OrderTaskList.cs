using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.TaskList
{
    public sealed class OrderTaskList
    {
        public TaskProgress DescriptionStatus { get; set; } = TaskProgress.NotStarted;

        public TaskProgress OrderingPartyStatus { get; set; } = TaskProgress.CannotStart;

        public TaskProgress SupplierStatus { get; set; } = TaskProgress.CannotStart;

        public TaskProgress CommencementDateStatus { get; set; } = TaskProgress.CannotStart;

        public TaskProgress FundingSourceStatus { get; set; } = TaskProgress.CannotStart;

        public TaskProgress ReviewAndCompleteStatus { get; set; } = TaskProgress.CannotStart;
    }
}
