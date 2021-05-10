using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System.Threading.Tasks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.Hosting
{
    public sealed class PrivateCloud : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public PrivateCloud(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/hosting-type-private-cloud")
        {
            ClearHostingTypes("99999-99");
            driver.Navigate().Refresh();
        }

        [Fact]
        public async Task PrivateCloud_CompleteAllFields()
        {
            var summary = MarketingPages.Hosting.PrivateCloudActions.EnterSummary(500);
            var link = MarketingPages.Hosting.PrivateCloudActions.EnterLink(1000);
            var hostingModel = MarketingPages.Hosting.PrivateCloudActions.EnterHostingModel(1000);
            MarketingPages.Hosting.PublicCloudActions.ToggleHSCNCheckbox();

            MarketingPages.CommonActions.ClickSave();

            using var context = GetBCContext();
            var hosting = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).Hosting;

            hosting.Should().ContainEquivalentOf($"\"PrivateCloud\":{{\"Summary\":\"{summary}\",\"Link\":\"{link}\",\"HostingModel\":\"{hostingModel}\"");
        }

        [Fact]
        public void PrivateCloud_SectionComplete()
        {
            MarketingPages.Hosting.PrivateCloudActions.EnterSummary(500);
            MarketingPages.Hosting.PrivateCloudActions.EnterLink(1000);
            MarketingPages.Hosting.PrivateCloudActions.EnterHostingModel(1000);
            MarketingPages.Hosting.PublicCloudActions.ToggleHSCNCheckbox();

            MarketingPages.CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Private cloud").Should().BeTrue();
        }

        [Fact]
        public void PrivateCloud_SectionIncomplete()
        {
            MarketingPages.CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Private cloud").Should().BeFalse();
        }
    }
}
