using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.BrowserBased
{
    public sealed class PlugInsOrExtensions : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public PlugInsOrExtensions(LocalWebApplicationFactory factory)
            : base(factory, "marketing/supplier/solution/99999-99/section/browser-based/plug-ins-or-extensions")
        {
            Login();
        }

        [Theory]
        [InlineData("Yes")]
        [InlineData("No")]
        public async Task PlugInsOrExtensions_SelectRadioButton(string label)
        {
            CommonActions.ClickRadioButtonWithText(label);
            var additional = TextGenerators.TextInputAddText(CommonSelectors.AdditionalInfoTextArea, 100);

            CommonActions.ClickSave();

            string labelConvert = label == "Yes" ? "true" : "false";

            await using var context = GetEndToEndDbContext();
            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "99"))).ClientApplication;
            clientApplication.Should().ContainEquivalentOf(@$"Plugins"":{{""Required"":{labelConvert},""AdditionalInformation"":""{additional}""}}");
        }

        [Fact]
        public void PlugInsOrExtensions_SectionComplete()
        {
            CommonActions.ClickRadioButtonWithText("Yes");
            TextGenerators.TextInputAddText(CommonSelectors.AdditionalInfoTextArea, 100);

            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Plug-ins or extensions required").Should().BeTrue();
        }

        [Fact]
        public void PlugInsOrExtensions_SectionIncomplete()
        {
            CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Plug-ins or extensions required").Should().BeFalse();
        }

        public void Dispose()
        {
            ClearClientApplication(new CatalogueItemId(99999, "99"));
        }
    }
}
