using System;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Serialization;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

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
            SetCookie(agreeToAnalytics);

            return Redirect(Request.GetTypedHeaders().Referer is null ? "~/" : Request.GetTypedHeaders().Referer.ToString());
        }

        [HttpGet("cookie-settings")]
        public IActionResult CookieSettings()
            => View(new CookieSettingsModel()
            {
                UseAnalytics = GetCookie()?.Analytics ?? true,
                BackLink = Url.Action(nameof(HomeController.PrivacyPolicy), typeof(HomeController).ControllerName()),
            });

        [HttpPost("cookie-settings")]
        public IActionResult CookieSettings(CookieSettingsModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            SetCookie(model.UseAnalytics!.Value);

            return RedirectToAction(nameof(HomeController.PrivacyPolicy), typeof(HomeController).ControllerName());
        }

        private CookieData GetCookie()
        {
            var cookieDataString = Request.Cookies[CatalogueCookies.BuyingCatalogueConsent];
            if (string.IsNullOrWhiteSpace(cookieDataString))
                return null;

            return JsonDeserializer.Deserialize<CookieData>(cookieDataString);
        }

        private void SetCookie(bool useAnalytics)
        {
            var cookieDataString = JsonSerializer.Serialize(
                new CookieData
                {
                    Analytics = useAnalytics,
                    CreationDate = DateTime.UtcNow.Ticks,
                });

            foreach (var cookie in Request.Cookies.Where(c => !CatalogueCookies.MandatoryCookies.Contains(c.Key)))
            {
                Response.Cookies.Delete(cookie.Key);
            }

            Response.Cookies.Append(
                CatalogueCookies.BuyingCatalogueConsent,
                cookieDataString,
                new CookieOptions
                {
                    Expires = DateTime.UtcNow.Add(cookieExpiration.ConsentExpiration),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                });
        }
    }
}
