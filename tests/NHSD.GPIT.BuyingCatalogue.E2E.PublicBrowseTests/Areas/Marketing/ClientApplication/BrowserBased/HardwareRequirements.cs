using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.BrowserBased
{
    public sealed class HardwareRequirements : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public HardwareRequirements(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/browser-based/hardware-requirements")
        {
            ClearClientApplication("99999-99");
        }

        [Fact]
        public async Task HarwareRequirements_CompleteAllFields()
        {
            var hardwareRequirement = MarketingPages.AboutSupplierActions.DescriptionAddText(1000);

            MarketingPages.CommonActions.ClickSave();

            using var context = GetBCContext();

            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).ClientApplication;
            clientApplication.Should().ContainEquivalentOf($"\"HardwareRequirements\":\"{hardwareRequirement}\"");
        }

        [Fact]
        public void HarwareRequirements_SectionComplete()
        {
            MarketingPages.AboutSupplierActions.DescriptionAddText(1000);

            MarketingPages.CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Hardware requirements").Should().BeTrue();
        }

        [Fact]
        public void HarwareRequirements_SectionIncomplete()
        {
            MarketingPages.CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Hardware requirements").Should().BeFalse();
        }
    }
}
