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
        public IActionResult DismissCookieBanner()
        {
            var cookieDataString = JsonSerializer.Serialize(
                    new CookieData
                    {
                        CreationDate = DateTime.UtcNow.Ticks,
                    });

            Response.Cookies.Append(
                Cookies.BuyingCatalogueConsent,
                cookieDataString,
                new CookieOptions { Expires = DateTime.Now.Add(cookieExpiration.ConsentExpiration), HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict, });

            if (Request.GetTypedHeaders().Referer is null)
                return Redirect("~/");

            return Redirect(Request.GetTypedHeaders().Referer.ToString());
        }
    }
}
