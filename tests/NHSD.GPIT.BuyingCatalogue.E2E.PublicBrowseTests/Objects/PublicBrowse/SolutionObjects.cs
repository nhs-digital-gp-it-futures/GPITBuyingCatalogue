using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.PublicBrowse
{
    internal static class SolutionObjects
    {
        internal static By SolutionName => CustomBy.DataTestId("view-solution-page-solution-name");
        
        internal static By SolutionDetailTableRow => By.ClassName("nhsuk-summary-list__row");
        
        internal static By SummaryAndDescription => By.TagName("p");
        
        internal static By FeatureList => By.TagName("ul");
        
        internal static By CapabilitiesContent => By.CssSelector("tbody tr td:nth-child(1)");

        internal static By FlatPriceTable => CustomBy.DataTestId("flat-list-price-table");

        internal static By PriceColumn => CustomBy.DataTestId("price");
    }
}

