using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common
{
    public static class CookieBanner
    {
        public static By AcceptAnalyticsLink => By.Id("accept-analytics");

        public static By AdobeAnalyticsScript => By.Id("adobe-analytics");

        public static By AdobeAnalyticsDataScript => By.Id("adobe-analytics-data");

        public static By Banner => By.Id("cookie-banner");

        public static By HotjarScript => By.Id("hotjar");

        public static By RejectAnalyticsLink => By.Id("reject-analytics");
    }
}
