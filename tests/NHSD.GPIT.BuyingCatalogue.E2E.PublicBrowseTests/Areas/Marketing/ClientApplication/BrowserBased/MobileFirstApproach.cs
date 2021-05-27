using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.BrowserBased
{
    // TODO: fix when page updated with new layout
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1000:Test classes must be public", Justification = "Disabled")]
    internal sealed class MobileFirstApproach : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public MobileFirstApproach(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/browser-based/mobile-first-approach")
        {
        }

        [Theory]
        [InlineData("Yes")]
        [InlineData("No")]
        public async Task MobileFirstApproach_SelectRadioButton(string label)
        {
            CommonActions.ClickRadioButtonWithText(label);

            CommonActions.ClickSave();

            string labelConvert = label == "Yes" ? "true" : "false";

            using var context = GetBCContext();
            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).ClientApplication;
            clientApplication.Should().ContainEquivalentOf(@$"MobileFirstDesign"":{ labelConvert }");
        }

        [Fact]
        public void MobileFirstApproach_SectionComplete()
        {
            CommonActions.ClickRadioButtonWithText("Yes");

            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Mobile first approach").Should().BeTrue();
        }

        [Fact]
        public void MobileFirstApproach_SectionIncomplete()
        {
            CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Mobile first approach").Should().BeFalse();
        }

        public void Dispose()
        {
            ClearClientApplication("99999-99");
        }
    }
}
