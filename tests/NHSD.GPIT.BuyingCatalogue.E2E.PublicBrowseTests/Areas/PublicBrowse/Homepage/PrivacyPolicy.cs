using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using OpenQA.Selenium;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Homepage
{
    public sealed class PrivacyPolicy : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly Dictionary<string, string> Parameters = new();

        public PrivacyPolicy(LocalWebApplicationFactory factory)
            : base(
                factory,
                typeof(HomeController),
                nameof(HomeController.PrivacyPolicy),
                Parameters)
        {
        }

        [Fact]
        public void PrivacyPolicy_ClickCookieSettings()
        {
            CommonActions.ClickLinkElement(By.LinkText("Cookies that measure website use"));

            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(ConsentController),
                    nameof(ConsentController.CookieSettings))
                .Should()
                .BeTrue();
        }
    }
}
