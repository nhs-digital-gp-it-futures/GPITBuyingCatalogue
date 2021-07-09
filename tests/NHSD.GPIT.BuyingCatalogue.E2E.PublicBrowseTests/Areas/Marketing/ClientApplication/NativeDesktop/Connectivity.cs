using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.NativeDesktop
{
    public sealed class Connectivity : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public Connectivity(LocalWebApplicationFactory factory)
            : base(factory, "marketing/supplier/solution/99999-99/section/native-desktop/connectivity")
        {
            Login();
        }

        [Fact]
        public async Task Connectivity_CompleteAllFields()
        {
            CommonActions.SelectDropdownItem(CommonSelectors.ConnectionSpeedSelect, 1);

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();

            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "99"))).ClientApplication;

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
            ClearClientApplication(new CatalogueItemId(99999, "99"));
        }
    }
}
