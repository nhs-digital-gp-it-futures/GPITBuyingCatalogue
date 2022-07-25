namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.TaskList
{
    public sealed class OrderTaskListCompletedSections
    {
        public bool OrderContactDetailsCompleted { get; set; }

        public bool SupplierSelected { get; set; }

        public bool SupplierContactSelected { get; set; }

        public bool TimeScalesCompleted { get; set; }

        public bool SolutionsSelected { get; set; }

        public bool SolutionsCompleted { get; set; }

        public bool HasAssociatedServices { get; set; }

        public bool FundingInProgress { get; set; }

        public bool FundingCompleted { get; set; }

        public bool HasImplementationPlan { get; set; }

        public bool OrderCompleted { get; set; }
    }
}
