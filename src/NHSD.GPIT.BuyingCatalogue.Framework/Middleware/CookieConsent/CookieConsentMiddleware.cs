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
            var showConsent = ShowCookieConsent(context.Request, cookieExpirationSettings.BuyingCatalogueCookiePolicyDate);

            if (context.Items.TryGetValue(Cookies.ShowConsentCookie, out _))
            {
                context.Items[Cookies.ShowConsentCookie] = showConsent;
            }
            else
            {
                context.Items.Add(Cookies.ShowConsentCookie, showConsent);
            }

            await next.Invoke(context);
        }

        private static bool ShowCookieConsent(HttpRequest httpRequest, DateTime? buyingCatalogueCookiePolicyDate)
        {
            if (httpRequest is null)
                return false;

            if (!httpRequest.Cookies.TryGetValue(Cookies.BuyingCatalogueConsent, out var consentCookieValue))
                return true;

            if (!buyingCatalogueCookiePolicyDate.HasValue)
                return false;

            return buyingCatalogueCookiePolicyDate.Value <= DateTime.UtcNow
                && ExtractCookieCreationDate(consentCookieValue) is var creationDate
                && creationDate < buyingCatalogueCookiePolicyDate.Value;
        }

        private static DateTime ExtractCookieCreationDate(string input)
        {
            // Will force banner to be displayed and cookie rewritten with correct data when banner is dismissed again
            static DateTime ForceBannerDisplay() => DateTime.MinValue;

            if (input is null)
                return ForceBannerDisplay();

            CookieData cookieData;
            try
            {
                cookieData = JsonDeserializer.Deserialize<CookieData>(input);
            }
            catch (JsonException)
            {
                return ForceBannerDisplay();
            }

            var ticks = cookieData?.CreationDate.GetValueOrDefault() ?? 0;
            if (ticks < 1)
                return ForceBannerDisplay();

            return new DateTime(ticks);
        }
    }
}
