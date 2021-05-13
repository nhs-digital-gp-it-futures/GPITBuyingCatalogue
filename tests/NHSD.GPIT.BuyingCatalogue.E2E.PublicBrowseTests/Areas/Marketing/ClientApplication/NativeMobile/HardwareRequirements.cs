using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using System.Threading.Tasks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.NativeMobile
{
    public sealed class HardwareRequirements : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public HardwareRequirements(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/native-mobile/hardware-requirements")
        {
            ClearClientApplication("99999-99");
        }

        [Fact]
        public async Task HardwareRequirements_CompleteAllFields()
        {
            var hardwareRequirement = TextGenerators.TextInputAddText(CommonSelectors.Description, 300);

            CommonActions.ClickSave();

            using var context = GetBCContext();

            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).ClientApplication;
            clientApplication.Should().ContainEquivalentOf($"\"NativeMobileHardwareRequirements\":\"{hardwareRequirement}\"");
        }

        [Fact]
        public void HardwareRequirements_SectionComplete()
        {
            TextGenerators.TextInputAddText(CommonSelectors.Description, 300);

            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Hardware requirements").Should().BeTrue();
        }

        [Fact]
        public void HardwareRequirements_SectionIncomplete()
        {
            CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Hardware requirements").Should().BeFalse();
        }
    }
}
