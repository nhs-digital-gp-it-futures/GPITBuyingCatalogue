using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System.Threading.Tasks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.NativeDesktop
{
    public sealed class ThirdPartyComponents : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public ThirdPartyComponents(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/native-desktop/third-party")
        {
            ClearClientApplication("99999-99");
            driver.Navigate().Refresh();
        }

        [Fact]
        public async Task ThirdPartyComponents_CompleteAllFields()
        {
            var thirdPartyComponent = TextGenerators.TextInputAddText(CommonSelectors.ThirdPartyComponentTextArea, 500);

            var deviceCapability = TextGenerators.TextInputAddText(CommonSelectors.DeviceCapabilityTextArea, 500);

            CommonActions.ClickSave();

            using var context = GetBCContext();

            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).ClientApplication;
            clientApplication.Should().ContainEquivalentOf($"ThirdPartyComponents\":\"{thirdPartyComponent}\"");
            clientApplication.Should().ContainEquivalentOf($"DeviceCapabilities\":\"{deviceCapability}\"");
        }

        [Fact]
        public void ThirdPartyComponents_SectionComplete()
        {
            TextGenerators.TextInputAddText(CommonSelectors.ThirdPartyComponentTextArea, 500);

            TextGenerators.TextInputAddText(CommonSelectors.DeviceCapabilityTextArea, 500);

            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Third-party components and device capabilities").Should().BeTrue();
        }

        [Fact]
        public void ThirdPartyComponents_SectionIncomplete()
        {
            CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Third-party components and device capabilities").Should().BeFalse();
        }
    }
}
