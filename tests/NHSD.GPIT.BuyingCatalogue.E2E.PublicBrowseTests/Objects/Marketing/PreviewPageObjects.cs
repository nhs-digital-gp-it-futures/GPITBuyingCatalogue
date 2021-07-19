using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing
{
    internal static class PreviewPageObjects
    {
        internal static By ExpandingSections => By.CssSelector("details.nhsuk-details");

        internal static By BrowserBasedSectionTitles => ByExtensions.DataTestId("view-section-dl-browser-based", "dt");

        internal static By NativeMobileSectionTitles => ByExtensions.DataTestId("view-section-dl-native-mobile", "dt");

        internal static By NativeDesktopSectionTitles => ByExtensions.DataTestId("view-section-dl-native-desktop", "dt");

        internal static By OtherSectionTitles => ByExtensions.DataTestId("view-section-table-row-title");
    }
}
