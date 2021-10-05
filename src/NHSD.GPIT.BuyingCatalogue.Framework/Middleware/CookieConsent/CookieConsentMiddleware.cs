using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Serialization;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;

namespace NHSD.GPIT.BuyingCatalogue.Framework.Middleware.CookieConsent
{
    public class CookieConsentMiddleware
    {
        private readonly RequestDelegate next;

        public CookieConsentMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context, CookieExpirationSettings cookieExpirationSettings)
        {
            var (showBanner, useAnalytics) = ExtractCookieData(context.Request, cookieExpirationSettings.BuyingCatalogueCookiePolicyDate);

            context.Items[Cookies.ShowCookieBanner] = showBanner;

            if (!showBanner)
                context.Items[Cookies.UseAnalytics] = useAnalytics;

            await next.Invoke(context);
        }

        private static (bool ShowBanner, bool? UseAnalytics) ExtractCookieData(
            HttpRequest httpRequest,
            DateTime? buyingCatalogueCookiePolicyDate)
        {
            if (httpRequest is null)
                return (false, false);

            if (!httpRequest.Cookies.TryGetValue(Cookies.BuyingCatalogueConsent, out var consentCookieValue))
                return (true, null);

            var cookieData = ExtractCookieData(consentCookieValue);

            if (!cookieData.Analytics.HasValue)
                return (true, null);

            if (!buyingCatalogueCookiePolicyDate.HasValue)
                return (false, cookieData.Analytics);

            var showBanner = buyingCatalogueCookiePolicyDate.Value <= DateTime.UtcNow
                && new DateTime(cookieData.CreationDate.GetValueOrDefault()) < buyingCatalogueCookiePolicyDate.Value;

            return (showBanner, cookieData.Analytics);
        }

        private static CookieData ExtractCookieData(string input)
        {
            // Will force banner to be displayed and cookie rewritten with correct data when banner is dismissed again
            var forceBannerDisplay = new CookieData { CreationDate = 0 };

            if (input is null)
                return forceBannerDisplay;

            CookieData cookieData;
            try
            {
                cookieData = JsonDeserializer.Deserialize<CookieData>(input);
            }
            catch (JsonException)
            {
                return forceBannerDisplay;
            }

            return cookieData;
        }
    }
}
