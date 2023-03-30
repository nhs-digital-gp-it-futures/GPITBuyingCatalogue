using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NHSD.GPIT.BuyingCatalogue.Framework.Serialization;
using NHSD.GPIT.BuyingCatalogue.WebApp.ActionFilters;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using OpenQA.Selenium;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Homepage
{
    [Collection(nameof(SharedContextCollection))]
    public sealed class CookieSettings : AuthorityTestBase, IDisposable
    {
        private const string OptInRadioButtonText = "Use cookies to measure my website use";
        private const string OptOutRadioButtonText = "Do not use cookies to measure my website use";

        private static readonly Dictionary<string, string> Parameters = new();

        public CookieSettings(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
            : base(
                factory,
                typeof(ConsentController),
                nameof(ConsentController.CookieSettings),
                Parameters,
                testOutputHelper)
        {
        }

        [Theory]
        [InlineData(true, OptInRadioButtonText)]
        [InlineData(false, OptOutRadioButtonText)]
        public void CookieSettings_SetsCookieData(bool expectedSetting, string radioButtonText)
        {
            RunTest(() =>
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
            });
        }

        [Theory]
        [InlineData(OptInRadioButtonText)]
        [InlineData(OptOutRadioButtonText)]
        public void CookieSettings_DoesNotDeleteMandatoryCookies(string radioButtonText)
        {
            RunTest(() =>
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
            });
        }

        [Fact]
        public void CookieSettings_PreviouslyOptedIn_DeletesNonMandatoryCookies()
        {
            RunTest(() =>
            {
                Driver.Manage().Cookies.AddCookie(new Cookie("non-mandatory-cookie", "non-mandatory-cookie-value"));
                AddConsentCookie();

                Driver.Navigate().Refresh();

                CommonActions.ClickRadioButtonWithText(OptOutRadioButtonText);

                CommonActions.ClickSave();

                var updatedCookes = GetCookies();
                updatedCookes.Should().NotContain("non-mandatory-cookie");
            });
        }

        public void Dispose()
        {
            Driver.Manage().Cookies.DeleteAllCookies();
        }

        private string[] GetCookies() => Driver.Manage().Cookies.AllCookies.Select(c => c.Name).ToArray();

        private void AddConsentCookie()
        {
            var cookieData = new CookieData { Analytics = true };
            var cookieContent = JsonSerializer.Serialize(cookieData, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
            var cookie = new Cookie(CatalogueCookies.BuyingCatalogueConsent, HttpUtility.UrlEncode(cookieContent));

            Driver.Manage().Cookies.AddCookie(cookie);
        }
    }
}
