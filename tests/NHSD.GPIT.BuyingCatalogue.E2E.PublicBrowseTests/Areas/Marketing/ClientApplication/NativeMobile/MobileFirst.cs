using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System.Threading.Tasks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.NativeMobile
{
    public sealed class MobileFirst: TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public MobileFirst(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/native-mobile/mobile-first-approach")
        {
            ClearClientApplication("99999-99");
            driver.Navigate().Refresh();
        }

        [Theory]
        [InlineData("Yes")]
        [InlineData("No")]
        public async Task MobileFirst_CompleteRadioButton(string label)
        {
            MarketingPages.ClientApplicationTypeActions.ClickRadioButtonWithText(label);

            MarketingPages.CommonActions.ClickSave();

            using var context = GetBCContext();

            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).ClientApplication;

            clientApplication.Should().ContainEquivalentOf($"NativeMobileFirstDesign\":{(label == "Yes").ToString().ToLower()}");
        }

        [Fact(Skip = "Save failing on latest main")]
        public void MobileFirst_SectionComplete()
        {
            MarketingPages.ClientApplicationTypeActions.ClickRadioButtonWithText("Yes");

            MarketingPages.CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Mobile first approach").Should().BeTrue();
        }

        [Fact]
        public void MobileFirst_SectionIncomplete()
        {
            MarketingPages.CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Mobile first approach").Should().BeFalse();
        }
    }
}
