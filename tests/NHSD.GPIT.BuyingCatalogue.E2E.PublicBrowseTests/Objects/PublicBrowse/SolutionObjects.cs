using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.PublicBrowse
{
    internal static class SolutionObjects
    {    
        internal static By ImplementationName => By.TagName("h1");
      
        internal static By SolutionName => CustomBy.DataTestId("view-solution-page-solution-name");
        
        internal static By SolutionDetailTableRow => By.ClassName("nhsuk-summary-list__row");
        
        internal static By SummaryAndDescription => By.TagName("p");

        internal static By NhsSolutionEpics => CustomBy.DataTestId("nhs-defined-epics", "li");

        internal static By SupplierSolutionEpics => CustomBy.DataTestId("supplier-defined-epics", "li");

        internal static By CapabilitiesContent => By.CssSelector("tbody tr td:nth-child(1)");

        internal static By FlatPriceTable => CustomBy.DataTestId("flat-list-price-table");

        internal static By PriceColumn => CustomBy.DataTestId("price");

        internal static By SolutionEpicLink => By.LinkText("Check Epics");
          
    }
}
