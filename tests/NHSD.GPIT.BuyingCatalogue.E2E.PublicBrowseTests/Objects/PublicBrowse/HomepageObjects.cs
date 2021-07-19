using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.PublicBrowse
{
    internal static class HomepageObjects
    {
        public static By DFOCVCFrameworkTile => ByExtensions.DataTestId("dfocvc-card", "a");

        public static By BuyersGuideTile => ByExtensions.DataTestId("guidance-promo", "a");

        internal static By GpitFrameworkTile => ByExtensions.DataTestId("browse-promo", "a");

        internal static By OrderingTile => ByExtensions.DataTestId("order-form-promo", "a");

        internal static By HomePage => By.Id("maincontent");

        internal static By HomePageCrumb => ByExtensions.DataTestId("home-crumb");
    }
}
