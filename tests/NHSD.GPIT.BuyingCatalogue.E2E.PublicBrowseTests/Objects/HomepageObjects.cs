using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Objects
{
    internal static class HomepageObjects
    {
        public static By DFOCVCFrameworkTile => CustomBy.DataTestId("dfocvc-card", "a");

        internal static By GpitFrameworkTile => CustomBy.DataTestId("browse-promo", "a");
    }
}
