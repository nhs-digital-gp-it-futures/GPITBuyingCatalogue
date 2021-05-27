using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.BrowserBased
{
    // TODO: fix when page updated with new layout
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1000:Test classes must be public", Justification = "Disabled")]
    internal sealed class PlugInsOrExtensions : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public PlugInsOrExtensions(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/browser-based/plug-ins-or-extensions")
        {
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

            using var context = GetBCContext();
            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).ClientApplication;
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
            ClearClientApplication("99999-99");
        }
    }
}
