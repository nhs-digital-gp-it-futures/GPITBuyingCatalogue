using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.BrowserBased
{
    public sealed class SupportedBrowsers : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public SupportedBrowsers(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/browser-based/supported-browsers")
        {
            ClearClientApplication();
        }

        [Fact(Skip = "Error thrown by Automapper when saving page")]
        public async Task SupportedBrowser_SelectBrowser()
        {
            var browser = MarketingPages.ClientApplicationTypeActions.ClickBrowserCheckbox();
            MarketingPages.ClientApplicationTypeActions.ClickRadioButtonWithText("Yes");

            MarketingPages.CommonActions.ClickSave();

            using var context = GetBCContext();
            (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).ClientApplication.Should().ContainEquivalentOf(browser);
        }

        [Theory(Skip = "Error thrown by Automapper when saving page")]
        [InlineData("Yes")]
        [InlineData("No")]
        public async Task SupportedBrowser_SelectMobileResponsive(string label)
        {
            MarketingPages.ClientApplicationTypeActions.ClickBrowserCheckbox();
            MarketingPages.ClientApplicationTypeActions.ClickRadioButtonWithText(label);

            MarketingPages.CommonActions.ClickSave();

            string labelConvert = label == "Yes" ? "true" : "false";

            using var context = GetBCContext();
            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).ClientApplication;
            clientApplication.Should().ContainEquivalentOf(@$"MobileResponsive"":{ labelConvert }");
        }
    }
}
