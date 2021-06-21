using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.PublicBrowse
{
    internal static class SolutionObjects
    {
        public static By SolutionName => CustomBy.DataTestId("view-solution-page-solution-name");
        public static By SolutionDetailTableRow => By.ClassName("nhsuk-summary-list__row");
        public static By SummaryAndDescription => By.TagName("p");
        public static By FeatureList => By.TagName("ul");
        public static By CapabilitiesContent => By.CssSelector("tbody tr td:nth-child(1)");
    }
}

