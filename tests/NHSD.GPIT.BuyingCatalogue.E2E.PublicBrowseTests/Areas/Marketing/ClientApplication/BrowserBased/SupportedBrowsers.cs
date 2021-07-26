using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.BrowserBased
{
    public sealed class SupportedBrowsers : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public SupportedBrowsers(LocalWebApplicationFactory factory)
            : base(factory, "marketing/supplier/solution/99999-002/section/browser-based/supported-browsers")
        {
            AuthorityLogin();
        }

        [Fact]
        public async Task SupportedBrowser_SelectBrowser()
        {
            var browser = CommonActions.ClickCheckbox(CommonSelectors.CheckboxItem);

            CommonActions.ClickRadioButtonWithText("Yes");

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            (await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "002"))).ClientApplication.Should().ContainEquivalentOf(browser);
        }

        [Theory]
        [InlineData("Yes")]
        [InlineData("No")]
        public async Task SupportedBrowser_SelectMobileResponsive(string label)
        {
            CommonActions.ClickCheckbox(CommonSelectors.CheckboxItem);
            CommonActions.ClickRadioButtonWithText(label);

            CommonActions.ClickSave();

            string labelConvert = label == "Yes" ? "true" : "false";

            await using var context = GetEndToEndDbContext();
            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "002"))).ClientApplication;
            clientApplication.Should().ContainEquivalentOf(@$"MobileResponsive"":{labelConvert}");
        }

        public void Dispose()
        {
            ClearClientApplication(new CatalogueItemId(99999, "002"));
        }
    }
}
