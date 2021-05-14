using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System.Threading.Tasks;
using System;
using Xunit;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.BrowserBased
{
    public sealed class HardwareRequirements : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public HardwareRequirements(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/browser-based/hardware-requirements")
        {
        }

        [Fact]
        public async Task HarwareRequirements_CompleteAllFields()
        {
            var hardwareRequirement = TextGenerators.TextInputAddText(CommonSelectors.Description, 500);

            CommonActions.ClickSave();

            using var context = GetBCContext();

            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).ClientApplication;
            clientApplication.Should().ContainEquivalentOf($"\"HardwareRequirements\":\"{hardwareRequirement}\"");
        }

        [Fact]
        public void HarwareRequirements_SectionComplete()
        {
            TextGenerators.TextInputAddText(CommonSelectors.Description, 500);

            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Hardware requirements").Should().BeTrue();
        }

        [Fact]
        public void HarwareRequirements_SectionIncomplete()
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
