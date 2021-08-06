using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.ClientApplication.BrowserBased
{
    public sealed class ConnectivityAndResolution : TestBase, IClassFixture<LocalWebApplicationFactory>, IDisposable
    {
        public ConnectivityAndResolution(LocalWebApplicationFactory factory)
            : base(factory, "marketing/supplier/solution/99999-002/section/browser-based/connectivity-and-resolution")
        {
            AuthorityLogin();
        }

        [Fact]
        public async Task ConnectivityAndResolution_SelectBothFields()
        {
            CommonActions.SelectDropDownItem(CommonSelectors.ConnectionSpeedSelect, 1);
            CommonActions.SelectDropDownItem(CommonSelectors.ResolutionSelect, 1);

            CommonActions.ClickSave();

            await using var context = GetEndToEndDbContext();
            var clientApplication = (await context.Solutions.SingleAsync(s => s.Id == new CatalogueItemId(99999, "002"))).ClientApplication;
            clientApplication.Should().ContainEquivalentOf(@$"""MinimumConnectionSpeed"":""0.5Mbps""");
            clientApplication.Should().ContainEquivalentOf(@$"""MinimumDesktopResolution"":""16:9 - 640 x 360""");
        }

        [Fact]
        public void ConnectivityAndResolution_SectionComplete()
        {
            CommonActions.SelectDropDownItem(CommonSelectors.ConnectionSpeedSelect, 1);
            CommonActions.SelectDropDownItem(CommonSelectors.ResolutionSelect, 1);

            CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Connectivity and resolution").Should().BeTrue();
        }

        [Fact]
        public void ConnectivityAndResolution_SectionIncomplete()
        {
            CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Connectivity and resolution").Should().BeFalse();
        }

        public void Dispose()
        {
            ClearClientApplication(new CatalogueItemId(99999, "002"));
        }
    }
}
