using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common
{
    internal static class CookieBanner
    {
        internal static By AcceptAnalyticsLink => By.Id("accept-analytics");

        internal static By Banner => By.Id("cookie-banner");

        internal static By HotjarScript => By.Id("hotjar");

        internal static By RejectAnalyticsLink => By.Id("reject-analytics");
    }
}
