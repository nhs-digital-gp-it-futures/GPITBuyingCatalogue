namespace NHSD.GPIT.BuyingCatalogue.Framework.Constants
{
    public static class CatalogueCookies
    {
        public const string BuyingCatalogueConsent = "buyingcatalogue-cookie-consent";

        public const string BuyingCatalogueConsentExpiration = "cookieExpiration:buyingcatalogue-cookie-consent-expiration";

        public const string ShowCookieBanner = nameof(ShowCookieBanner);

        public const string UseAnalytics = nameof(UseAnalytics);

        public static readonly string[] MandatoryCookies = new[]
        {
            "io",
            "token",
            ".AspNetCore.Antiforgery",
            ".AspNetCore.Identity.Application",
            "user-session",
            "antiforgery",
        };
    }
}
