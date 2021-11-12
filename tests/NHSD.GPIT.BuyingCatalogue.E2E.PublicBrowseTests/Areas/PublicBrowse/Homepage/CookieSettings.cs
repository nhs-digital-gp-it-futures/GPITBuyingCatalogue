using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
        private const string OptInRadioButtonText = "Use cookies to measure my website use";
        private const string OptOutRadioButtonText = "Do not use cookies to measure my website use";

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
        [InlineData(true, OptInRadioButtonText)]
        [InlineData(false, OptOutRadioButtonText)]
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
        [InlineData(OptInRadioButtonText)]
        [InlineData(OptOutRadioButtonText)]
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

        [Fact]
        public void CookieSettings_PreviouslyOptedIn_DeletesNonMandatoryCookies()
        {
            Driver.Manage().Cookies.AddCookie(new Cookie("non-mandatory-cookie", "non-mandatory-cookie-value"));
            AddConsentCookie();

            Driver.Navigate().Refresh();

            CommonActions.ClickRadioButtonWithText(OptOutRadioButtonText);

            CommonActions.ClickSave();

            var updatedCookes = GetCookies();
            updatedCookes.Should().NotContain("non-mandatory-cookie");
        }

        public void Dispose()
        {
            Driver.Manage().Cookies.DeleteAllCookies();
        }

        private string[] GetCookies() => Driver.Manage().Cookies.AllCookies.Select(c => c.Name).ToArray();

        private void AddConsentCookie()
        {
            var cookieData = new CookieData { Analytics = true };
            var cookieContent = JsonSerializer.Serialize(cookieData, new JsonSerializerOptions { IgnoreNullValues = true });
            var cookie = new Cookie(CatalogueCookies.BuyingCatalogueConsent, HttpUtility.UrlEncode(cookieContent));

            Driver.Manage().Cookies.AddCookie(cookie);
        }
    }
}
