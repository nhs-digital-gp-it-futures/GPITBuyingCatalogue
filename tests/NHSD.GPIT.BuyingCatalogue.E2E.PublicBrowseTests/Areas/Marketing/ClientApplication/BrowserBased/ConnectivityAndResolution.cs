using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System.Threading.Tasks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.BrowserBased
{
    public sealed class ConnectivityAndResolution : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public ConnectivityAndResolution(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/browser-based/connectivity-and-resolution")
        {
            ClearClientApplication("99999-99");

            driver.Navigate().Refresh();
        }

        [Fact]
        public async Task ConnectivityAndResolution_SelectBothFields()
        {
            CommonActions.SelectDropdownItem(CommonSelectors.ConnectionSpeedSelect, 1);
            CommonActions.SelectDropdownItem(CommonSelectors.ResolutionSelect, 1);

            CommonActions.ClickSave();

            using var context = GetBCContext();
            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).ClientApplication;
            clientApplication.Should().ContainEquivalentOf(@$"""MinimumConnectionSpeed"":""0.5Mbps""");
            clientApplication.Should().ContainEquivalentOf(@$"""MinimumDesktopResolution"":""16:9 - 640 x 360""");
        }

        [Fact]
        public void ConnectivityAndResolution_SectionComplete()
        {
            CommonActions.SelectDropdownItem(CommonSelectors.ConnectionSpeedSelect, 1);
            CommonActions.SelectDropdownItem(CommonSelectors.ResolutionSelect, 1);

            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Connectivity and resolution").Should().BeTrue();
        }

        [Fact]
        public void ConnectivityAndResolution_SectionIncomplete()
        {
            CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Connectivity and resolution").Should().BeFalse();
        }
    }
}
