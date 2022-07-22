using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.TaskList
{
    public sealed class OrderTaskList
    {
        public TaskProgress DescriptionStatus { get; set; } = TaskProgress.NotStarted;

        public TaskProgress OrderingPartyStatus { get; set; } = TaskProgress.CannotStart;

        public TaskProgress SupplierStatus { get; set; } = TaskProgress.CannotStart;

        public TaskProgress CommencementDateStatus { get; set; } = TaskProgress.CannotStart;

        public TaskProgress SolutionOrService { get; set; } = TaskProgress.CannotStart;

        public TaskProgress FundingSource { get; set; } = TaskProgress.CannotStart;

        public TaskProgress ImplementationPlan { get; set; } = TaskProgress.CannotStart;

        public TaskProgress AssociatedServiceBilling { get; set; } = TaskProgress.CannotStart;

        public TaskProgress DataProcessingInformation { get; set; } = TaskProgress.CannotStart;

        public TaskProgress ReviewAndCompleteStatus { get; set; } = TaskProgress.CannotStart;
    }
}
