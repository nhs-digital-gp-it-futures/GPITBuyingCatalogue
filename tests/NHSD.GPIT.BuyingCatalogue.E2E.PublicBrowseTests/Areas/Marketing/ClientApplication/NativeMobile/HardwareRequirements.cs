using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.NativeMobile
{
    public sealed class HardwareRequirements : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public HardwareRequirements(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/native-mobile/hardware-requirements")
        {
        }

        [Fact]
        public async Task HardwareRequirements_CompleteAllFields()
        {
            var hardwareRequirement = TextGenerators.TextInputAddText(CommonSelectors.Description, 300);

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();

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

        public void Dispose()
        {
            ClearClientApplication("99999-99");
        }
    }
}
