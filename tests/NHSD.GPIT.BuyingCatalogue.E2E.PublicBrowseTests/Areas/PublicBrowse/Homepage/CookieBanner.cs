using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Web;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Middleware.CookieConsent;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using OpenQA.Selenium;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Homepage
{
    public sealed class CookieBanner
        : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private static readonly Dictionary<string, string> Parameters = new();

        public CookieBanner(LocalWebApplicationFactory factory)
            : base(
                factory,
                typeof(HomeController),
                nameof(HomeController.Index),
                Parameters)
        {
            Driver.Manage().Cookies.DeleteAllCookies();
        }

        [Fact]
        public void CookieBanner_NoCookie_ClickAcceptAnalytics_HidesBanner()
        {
            CommonActions.ElementIsDisplayed(Objects.Common.CookieBanner.Banner).Should().BeTrue();
            CommonActions.ElementExists(Objects.Common.CookieBanner.HotjarScript).Should().BeFalse();

            CommonActions.ClickLinkElement(Objects.Common.CookieBanner.AcceptAnalyticsLink);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(HomeController),
                    nameof(HomeController.Index))
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Common.CookieBanner.Banner).Should().BeFalse();

            CommonActions.ElementExists(Objects.Common.CookieBanner.AdobeAnalyticsScript).Should().BeTrue();
            CommonActions.ElementExists(Objects.Common.CookieBanner.AdobeAnalyticsDataScript).Should().BeTrue();
            CommonActions.ElementExists(Objects.Common.CookieBanner.HotjarScript).Should().BeTrue();

            Driver.Manage().Cookies.GetCookieNamed(Cookies.BuyingCatalogueConsent).Should().NotBeNull();
        }

        [Fact]
        public void CookieBanner_NoCookie_ClickRejectAnalytics_HidesBanner()
        {
            CommonActions.ElementIsDisplayed(Objects.Common.CookieBanner.Banner).Should().BeTrue();
            CommonActions.ElementExists(Objects.Common.CookieBanner.HotjarScript).Should().BeFalse();

            CommonActions.ClickLinkElement(Objects.Common.CookieBanner.RejectAnalyticsLink);

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(HomeController),
                    nameof(HomeController.Index))
                .Should()
                .BeTrue();

            CommonActions.ElementIsDisplayed(Objects.Common.CookieBanner.Banner).Should().BeFalse();

            CommonActions.ElementExists(Objects.Common.CookieBanner.AdobeAnalyticsScript).Should().BeFalse();
            CommonActions.ElementExists(Objects.Common.CookieBanner.AdobeAnalyticsDataScript).Should().BeFalse();
            CommonActions.ElementExists(Objects.Common.CookieBanner.HotjarScript).Should().BeFalse();

            Driver.Manage().Cookies.GetCookieNamed(Cookies.BuyingCatalogueConsent).Should().NotBeNull();
        }

        [Fact]
        public void CookieBanner_NoCookie_CookieBannerDisplayed()
        {
            CommonActions.ElementIsDisplayed(Objects.Common.CookieBanner.Banner).Should().BeTrue();
            CommonActions.ElementExists(Objects.Common.CookieBanner.HotjarScript).Should().BeFalse();

            NavigateToUrl(
                typeof(SolutionsController),
                nameof(SolutionsController.Index));

            CommonActions.ElementIsDisplayed(Objects.Common.CookieBanner.Banner).Should().BeTrue();

            CommonActions.ElementExists(Objects.Common.CookieBanner.AdobeAnalyticsScript).Should().BeFalse();
            CommonActions.ElementExists(Objects.Common.CookieBanner.AdobeAnalyticsDataScript).Should().BeFalse();
            CommonActions.ElementExists(Objects.Common.CookieBanner.HotjarScript).Should().BeFalse();
        }

        [Fact]
        public void CookieBanner_WithCookie_WithoutAnalytics_CookieBannerDisplayed()
        {
            AddConsentCookie(new CookieData { CreationDate = DateTime.UtcNow.Ticks });

            CommonActions.ElementIsDisplayed(Objects.Common.CookieBanner.Banner).Should().BeTrue();

            CommonActions.ElementExists(Objects.Common.CookieBanner.AdobeAnalyticsScript).Should().BeFalse();
            CommonActions.ElementExists(Objects.Common.CookieBanner.AdobeAnalyticsDataScript).Should().BeFalse();
            CommonActions.ElementExists(Objects.Common.CookieBanner.HotjarScript).Should().BeFalse();
        }

        public void Dispose()
        {
            Driver.Manage().Cookies.DeleteAllCookies();
        }

        private void AddConsentCookie(CookieData cookieData)
        {
            var cookieContent = JsonSerializer.Serialize(cookieData, new JsonSerializerOptions { IgnoreNullValues = true });
            var cookie = new Cookie(Cookies.BuyingCatalogueConsent, HttpUtility.UrlEncode(cookieContent));

            Driver.Manage().Cookies.AddCookie(cookie);
        }
    }
}
