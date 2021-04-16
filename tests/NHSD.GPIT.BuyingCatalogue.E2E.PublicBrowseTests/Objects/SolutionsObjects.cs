using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Objects
{
    internal static class SolutionsObjects
    {
        public static By SolutionList => CustomBy.DataTestId("solution-cards");

        public static By SolutionsInList => CustomBy.DataTestId("solution-card");

        public static By SolutionName => CustomBy.DataTestId("solution-card-name");

        public static By SolutionSupplierName => CustomBy.DataTestId("solution-card-supplier");

        public static By SolutionSummary => CustomBy.DataTestId("solution-card-summary");

        public static By SolutionCapabilityList => CustomBy.DataTestId("capability-list");

        public static By SolutionCapabilityName => CustomBy.DataTestId("capability-list-item");

        public static By FoundationSolutionIndicators => CustomBy.DataTestId("solution-card-foundation");

        public static By CompareSolutions => CustomBy.DataTestId("compare-button", "a");
    }
}
