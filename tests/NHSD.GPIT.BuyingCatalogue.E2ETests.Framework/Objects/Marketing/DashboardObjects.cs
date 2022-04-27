using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Marketing
{
    public static class DashboardObjects
    {
        public static By SectionTitle => ByExtensions.DataTestId("dashboard-section-title");

        public static By Sections => By.CssSelector("li.bc-c-section-list__item");

        public static By SectionStatus => ByExtensions.DataTestId("dashboard-section-status");
    }
}
