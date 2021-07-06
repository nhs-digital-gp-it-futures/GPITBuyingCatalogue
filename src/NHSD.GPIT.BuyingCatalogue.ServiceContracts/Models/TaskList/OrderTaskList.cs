using System.Diagnostics.CodeAnalysis;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.TaskList
{
    public sealed class OrderTaskList
    {
        public TaskListStatuses DescriptionStatus { get; set; } = TaskListStatuses.Incomplete;

        public TaskListStatuses OrderingPartyStatus { get; set; }

        public TaskListStatuses SupplierStatus { get; set; }

        public TaskListStatuses CommencementDateStatus { get; set; }

        public TaskListStatuses CatalogueSolutionsStatus { get; set; }

        public TaskListStatuses AdditionalServiceStatus { get; set; }

        public TaskListStatuses AssociatedServiceStatus { get; set; }

        public TaskListStatuses FundingSourceStatus { get; set; }

        public TaskListStatuses ReviewAndCompleteStatus { get; set; }
    }
}
