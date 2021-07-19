using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.PublicBrowse
{
    internal static class SolutionsObjects
    {
        public static By SolutionList => ByExtensions.DataTestId("solution-cards");

        public static By SolutionsInList => ByExtensions.DataTestId("solution-card");

        public static By SolutionName => ByExtensions.DataTestId("solution-card-name");

        public static By SolutionSupplierName => ByExtensions.DataTestId("solution-card-supplier");

        public static By SolutionSummary => ByExtensions.DataTestId("solution-card-summary");

        public static By SolutionCapabilityList => ByExtensions.DataTestId("capability-list");

        public static By SolutionCapabilityName => ByExtensions.DataTestId("capability-list-item");

        public static By FoundationSolutionIndicators => ByExtensions.DataTestId("solution-card-foundation");

        public static By CompareSolutions => ByExtensions.DataTestId("compare-button", "a");
    }
}
