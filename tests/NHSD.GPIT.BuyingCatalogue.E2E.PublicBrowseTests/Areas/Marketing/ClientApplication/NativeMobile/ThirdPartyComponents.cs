using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System.Threading.Tasks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.NativeMobile
{
    public sealed class ThirdPartyComponents : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public ThirdPartyComponents(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/native-mobile/third-party")
        {
            ClearClientApplication("99999-99");
            driver.Navigate().Refresh();
        }

        [Fact]
        public async Task ThirdPartyComponents_CompleteAllFields()
        {
            var thirdPartyComponent = MarketingPages.ClientApplicationTypeActions.EnterThirdPartyComponents(500);

            var deviceCapability = MarketingPages.ClientApplicationTypeActions.EnterDeviceCapability(500);

            MarketingPages.CommonActions.ClickSave();

            using var context = GetBCContext();

            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).ClientApplication;
            clientApplication.Should().ContainEquivalentOf($"ThirdPartyComponents\":\"{thirdPartyComponent}\"");
            clientApplication.Should().ContainEquivalentOf($"DeviceCapabilities\":\"{deviceCapability}\"");
        }

        [Fact]
        public void ThirdPartyComponents_SectionComplete()
        {
            MarketingPages.ClientApplicationTypeActions.EnterThirdPartyComponents(500);

            MarketingPages.ClientApplicationTypeActions.EnterDeviceCapability(500);

            MarketingPages.CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Third-party components and device capabilities").Should().BeTrue();
        }

        [Fact]
        public void ThirdPartyComponents_SectionIncomplete()
        {
            MarketingPages.CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Third-party components and device capabilities").Should().BeFalse();
        }
    }
}
