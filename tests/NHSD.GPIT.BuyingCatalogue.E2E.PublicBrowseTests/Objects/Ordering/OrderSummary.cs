using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Ordering
{
    public static class OrderSummary
    {
        public static By PrintPDFButton => By.LinkText("Print or save as PDF");

        public static By LastUpdatedEndNote => ByExtensions.DataTestId("last-updated-endnote");
    }
}
