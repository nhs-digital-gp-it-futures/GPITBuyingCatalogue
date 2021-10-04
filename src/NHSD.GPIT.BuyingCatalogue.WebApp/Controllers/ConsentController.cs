using System;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Middleware.CookieConsent;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Controllers
{
    public sealed class ConsentController : Controller
    {
        private readonly CookieExpirationSettings cookieExpiration;

        public ConsentController(CookieExpirationSettings cookieSettings)
        {
            cookieExpiration = cookieSettings;
        }

        [HttpGet]
        public IActionResult AcceptCookies(bool agreeToAnalytics)
        {
            return SetCookie(agreeToAnalytics);
        }

        private IActionResult SetCookie(bool useAnalytics)
        {
            var cookieDataString = JsonSerializer.Serialize(
                new CookieData
                {
                    Analytics = useAnalytics,
                    CreationDate = DateTime.UtcNow.Ticks,
                });

            Response.Cookies.Append(
                Cookies.BuyingCatalogueConsent,
                cookieDataString,
                new CookieOptions
                {
                    Expires = DateTime.UtcNow.Add(cookieExpiration.ConsentExpiration),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                });

            return Redirect(Request.GetTypedHeaders().Referer is null ? "~/" : Request.GetTypedHeaders().Referer.ToString());
        }
    }
}
