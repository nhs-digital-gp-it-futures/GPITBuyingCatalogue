using System;
using System.Collections.Generic;
using System.Web;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Middleware.CookieConsent;
using NHSD.GPIT.BuyingCatalogue.Framework.Serialization;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using OpenQA.Selenium;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Homepage
{
    public sealed class CookieSettings : AuthorityTestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        private static readonly Dictionary<string, string> Parameters = new();

        public CookieSettings(LocalWebApplicationFactory factory)
            : base(
                factory,
                typeof(ConsentController),
                nameof(ConsentController.CookieSettings),
                Parameters)
        {
        }

        [Theory]
        [InlineData(true, "Use cookies to measure my website use")]
        [InlineData(false, "Do not use cookies to measure my website use")]
        public void CookieSettings_SetsCookieData(bool expectedSetting, string radioButtonText)
        {
            CommonActions.ClickRadioButtonWithText(radioButtonText);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(HomeController),
                    nameof(HomeController.PrivacyPolicy))
                .Should()
                .BeTrue();

            var cookie = Driver.Manage().Cookies.GetCookieNamed(CatalogueCookies.BuyingCatalogueConsent);
            var decodedValue = HttpUtility.UrlDecode(cookie.Value);
            var cookieData = JsonDeserializer.Deserialize<CookieData>(decodedValue);

            cookieData.Analytics.Should().Be(expectedSetting);
        }

        [Theory]
        [InlineData("Use cookies to measure my website use")]
        [InlineData("Do not use cookies to measure my website use")]
        public void CookieSettings_DoesNotDeleteMandatoryCookies(string radioButtonText)
        {
            Driver.Manage().Cookies.AddCookie(new Cookie("user-session", "user-session-value"));

            CommonActions.ClickRadioButtonWithText(radioButtonText);

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(HomeController),
                    nameof(HomeController.PrivacyPolicy))
                .Should()
                .BeTrue();

            var cookie = Driver.Manage().Cookies.AllCookies.Should().Contain(c => c.Name == "user-session");
        }

        public void Dispose()
        {
            Driver.Manage().Cookies.DeleteAllCookies();
        }
    }
}
