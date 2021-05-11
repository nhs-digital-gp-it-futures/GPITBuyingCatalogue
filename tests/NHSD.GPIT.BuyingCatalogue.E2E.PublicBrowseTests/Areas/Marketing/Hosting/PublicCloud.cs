using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System.Threading.Tasks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.Hosting
{
    public sealed class PublicCloud : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public PublicCloud(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/hosting-type-public-cloud")
        {
            ClearHostingTypes("99999-99");
            driver.Navigate().Refresh();
        }

        [Fact]
        public async Task PublicCloud_CompleteAllFields()
        {
            var summary = MarketingPages.Hosting.PublicCloudActions.EnterSummary(500);
            var link = MarketingPages.Hosting.PublicCloudActions.EnterLink(1000);
            MarketingPages.Hosting.PublicCloudActions.ToggleHSCNCheckbox();

            MarketingPages.CommonActions.ClickSave();

            using var context = GetBCContext();
            var hosting = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).Hosting;

            hosting.Should().ContainEquivalentOf($"\"PublicCloud\":{{\"Summary\":\"{summary}\",\"Link\":\"{link}\"");
        }

        [Fact]
        public void PublicCloud_SectionComplete()
        {
            MarketingPages.Hosting.PublicCloudActions.EnterSummary(500);
            MarketingPages.Hosting.PublicCloudActions.EnterLink(1000);
            MarketingPages.Hosting.PublicCloudActions.ToggleHSCNCheckbox();

            MarketingPages.CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Public cloud").Should().BeTrue();
        }

        [Fact]
        public void PublicCloud_SectionIncomplete()
        {
            MarketingPages.CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Public cloud").Should().BeFalse();
        }
    }
}
