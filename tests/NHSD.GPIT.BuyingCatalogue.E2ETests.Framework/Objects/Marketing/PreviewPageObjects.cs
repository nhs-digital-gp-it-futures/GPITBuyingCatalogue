using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Marketing
{
    public static class PreviewPageObjects
    {
        public static By ExpandingSections => By.CssSelector("details.nhsuk-details");

        public static By BrowserBasedSectionTitles => ByExtensions.DataTestId("view-section-dl-browser-based", "dt");

        public static By NativeMobileSectionTitles => ByExtensions.DataTestId("view-section-dl-native-mobile", "dt");

        public static By NativeDesktopSectionTitles => ByExtensions.DataTestId("view-section-dl-native-desktop", "dt");

        public static By OtherSectionTitles => ByExtensions.DataTestId("view-section-table-row-title");
    }
}
