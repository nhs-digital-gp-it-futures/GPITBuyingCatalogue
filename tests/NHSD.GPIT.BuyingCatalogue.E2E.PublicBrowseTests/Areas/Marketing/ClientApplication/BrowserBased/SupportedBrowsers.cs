using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using System.Threading.Tasks;
using System;
using Xunit;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.BrowserBased
{
    public sealed class SupportedBrowsers : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public SupportedBrowsers(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/browser-based/supported-browsers")
        {
        }

        [Fact]
        public async Task SupportedBrowser_SelectBrowser()
        {
            var browser = CommonActions.ClickCheckbox(CommonSelectors.BrowserCheckboxItem);

            CommonActions.ClickRadioButtonWithText("Yes");

            CommonActions.ClickSave();

            using var context = GetBCContext();
            (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).ClientApplication.Should().ContainEquivalentOf(browser);
        }

        [Theory]
        [InlineData("Yes")]
        [InlineData("No")]
        public async Task SupportedBrowser_SelectMobileResponsive(string label)
        {
            CommonActions.ClickCheckbox(CommonSelectors.BrowserCheckboxItem);
            CommonActions.ClickRadioButtonWithText(label);

            CommonActions.ClickSave();

            string labelConvert = label == "Yes" ? "true" : "false";

            using var context = GetBCContext();
            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).ClientApplication;
            clientApplication.Should().ContainEquivalentOf(@$"MobileResponsive"":{ labelConvert }");
        }

        public void Dispose()
        {
            ClearClientApplication("99999-99");
        }
    }
}
