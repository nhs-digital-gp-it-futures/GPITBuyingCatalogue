using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Serialization;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters
{
    public class CookieConsentActionFilter : IAsyncActionFilter
    {
        private readonly CookieExpirationSettings cookieExpirationSettings;

        public CookieConsentActionFilter(CookieExpirationSettings cookieExpirationSettings)
        {
            this.cookieExpirationSettings = cookieExpirationSettings ?? throw new ArgumentNullException(nameof(cookieExpirationSettings));
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            var (showBanner, useAnalytics) = ExtractCookieData(context.HttpContext.Request, cookieExpirationSettings.BuyingCatalogueCookiePolicyDate);

            context.HttpContext.Items[CatalogueCookies.ShowCookieBanner] = showBanner;

            if (!showBanner)
                context.HttpContext.Items[CatalogueCookies.UseAnalytics] = useAnalytics;

            await next();
        }

        private static (bool ShowBanner, bool? UseAnalytics) ExtractCookieData(
            HttpRequest httpRequest,
            DateTime? buyingCatalogueCookiePolicyDate)
        {
            if (httpRequest is null)
                return (false, false);

            if (!httpRequest.Cookies.TryGetValue(CatalogueCookies.BuyingCatalogueConsent, out var consentCookieValue))
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
