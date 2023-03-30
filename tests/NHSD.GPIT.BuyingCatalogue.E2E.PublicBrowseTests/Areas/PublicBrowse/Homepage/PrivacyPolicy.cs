using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using OpenQA.Selenium;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Homepage
{
    [Collection(nameof(SharedContextCollection))]
    public sealed class PrivacyPolicy : AnonymousTestBase
    {
        private static readonly Dictionary<string, string> Parameters = new();

        public PrivacyPolicy(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
            : base(
                factory,
                typeof(HomeController),
                nameof(HomeController.PrivacyPolicy),
                Parameters,
                testOutputHelper)
        {
        }

        [Fact]
        public void PrivacyPolicy_ClickCookieSettings()
        {
            RunTest(() =>
            {
                CommonActions.ClickLinkElement(By.LinkText("Cookies that measure website use"));

                CommonActions.PageLoadedCorrectGetIndex(
                        typeof(ConsentController),
                        nameof(ConsentController.CookieSettings))
                    .Should()
                    .BeTrue();
            });
        }

        [Theory]
        [InlineData("first-contact-us")]
        [InlineData("second-contact-us")]
        public void PrivacyPolicy_ContactLinks_Click(string dataTestId)
        {
            RunTest(() =>
            {
                CommonActions.ClickLinkElement(ByExtensions.DataTestId(dataTestId));

                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(HomeController),
                    nameof(HomeController.ContactUs))
                    .Should()
                    .BeTrue();
            });
        }
    }
}
