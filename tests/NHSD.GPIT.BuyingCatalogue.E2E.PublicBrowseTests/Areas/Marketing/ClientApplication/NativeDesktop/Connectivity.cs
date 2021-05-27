using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.NativeDesktop
{
    // TODO: fix when page updated with new layout
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1000:Test classes must be public", Justification = "Disabled")]
    internal sealed class Connectivity : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public Connectivity(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/native-desktop/connectivity")
        {
        }

        [Fact]
        public async Task Connectivity_CompleteAllFields()
        {
            CommonActions.SelectDropdownItem(CommonSelectors.ConnectionSpeedSelect, 1);

            CommonActions.ClickSave();

            using var context = GetBCContext();

            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).ClientApplication;

            clientApplication.Should().ContainEquivalentOf("NativeDesktopMinimumConnectionSpeed\":\"0.5Mbps\"");
        }

        [Fact]
        public void Connectivity_SectionComplete()
        {
            CommonActions.SelectDropdownItem(CommonSelectors.ConnectionSpeedSelect, 1);

            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Connectivity").Should().BeTrue();
        }

        [Fact]
        public void Connectivity_SectionIncomplete()
        {
            CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Connectivity").Should().BeFalse();
        }

        public void Dispose()
        {
            ClearClientApplication("99999-99");
        }
    }
}
