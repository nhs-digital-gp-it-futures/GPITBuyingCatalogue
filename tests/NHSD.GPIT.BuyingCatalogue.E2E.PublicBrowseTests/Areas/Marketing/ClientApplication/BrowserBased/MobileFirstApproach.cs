using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.BrowserBased
{
    public sealed class MobileFirstApproach : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public MobileFirstApproach(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/browser-based/mobile-first-approach")
        {
            ClearClientApplication();

            driver.Navigate().Refresh();
        }

        [Theory]
        [InlineData("Yes")]
        [InlineData("No")]
        public async Task MobileFirstApproach_SelectRadioButton(string label)
        {
            MarketingPages.ClientApplicationTypeActions.ClickRadioButtonWithText(label);

            MarketingPages.CommonActions.ClickSave();

            string labelConvert = label == "Yes" ? "true" : "false";

            using var context = GetBCContext();
            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).ClientApplication;
            clientApplication.Should().ContainEquivalentOf(@$"MobileFirstDesign"":{ labelConvert }");
        }

        [Fact]
        public void MobileFirstApproach_SectionComplete()
        {
            MarketingPages.ClientApplicationTypeActions.ClickRadioButtonWithText("Yes");

            MarketingPages.CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Mobile first approach").Should().BeTrue();
        }

        [Fact]
        public void MobileFirstApproach_SectionIncomplete()
        {
            MarketingPages.CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Mobile first approach").Should().BeFalse();
        }
    }
}
