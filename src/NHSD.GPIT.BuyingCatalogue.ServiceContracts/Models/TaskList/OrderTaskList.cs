using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.TaskList
{
    public sealed class OrderTaskList
    {
        public TaskProgress DescriptionStatus { get; set; } = TaskProgress.NotStarted;

        public TaskProgress OrderingPartyStatus { get; set; }

        public TaskProgress SupplierStatus { get; set; }

        public TaskProgress CommencementDateStatus { get; set; }

        public TaskProgress CatalogueSolutionsStatus { get; set; }

        public TaskProgress AdditionalServiceStatus { get; set; }

        public TaskProgress AssociatedServiceStatus { get; set; }

        public TaskProgress FundingSourceStatus { get; set; }

        public TaskProgress ReviewAndCompleteStatus { get; set; }
    }
}
