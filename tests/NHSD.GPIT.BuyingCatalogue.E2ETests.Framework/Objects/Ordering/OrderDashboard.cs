using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering
{
    public static class OrderDashboard
    {
        public static By TaskList => By.ClassName("bc-c-task-list");

        public static By OrderDescriptionLink => By.LinkText("Order description");

        public static By OrderDescriptionStatus => By.Id("Order_description-status");

        public static By LastUpdatedEndNote => ByExtensions.DataTestId("last-updated-endnote");

        public static By SolutionSelectionLink => By.LinkText("Select solutions and services");

        public static By CallOffParty => ByExtensions.DataTestId("test-calloff-party");

        public static By SupplierContact => ByExtensions.DataTestId("test-supplier-contact");

        public static By Timescales => ByExtensions.DataTestId("test-timescales");

        public static By SolutionsAndServices => ByExtensions.DataTestId("test-solutions-and-services");

        public static By FundingSources => ByExtensions.DataTestId("test-funding-sources");

        public static By ImplementationMilestones => ByExtensions.DataTestId("test-implementation-milestones");

        public static By AssociatedServiceBilling => ByExtensions.DataTestId("test-associated-service-billing");

        public static By DataProcessingInformation => ByExtensions.DataTestId("test-data-processing-info");

        public static By ReviewOrder => ByExtensions.DataTestId("test-review-and-complete-order");
    }
}
