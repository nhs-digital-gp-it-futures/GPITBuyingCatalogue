using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System.Threading.Tasks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.Hosting
{
    public sealed class Hybrid : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public Hybrid(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/hosting-type-hybrid")
        {
            ClearHostingTypes("99999-99");
            driver.Navigate().Refresh();
        }

        [Fact]
        public async Task Hybrid_CompleteAllFields()
        {
            var summary = MarketingPages.Hosting.HybridActions.EnterSummary(500);
            var link = MarketingPages.Hosting.HybridActions.EnterLink(1000);
            var hostingModel = MarketingPages.Hosting.HybridActions.EnterHostingModel(1000);
            MarketingPages.Hosting.PublicCloudActions.ToggleHSCNCheckbox();

            MarketingPages.CommonActions.ClickSave();

            using var context = GetBCContext();
            var hosting = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).Hosting;

            hosting.Should().ContainEquivalentOf($"\"HybridHostingType\":{{\"Summary\":\"{summary}\",\"Link\":\"{link}\",\"HostingModel\":\"{hostingModel}\"");
        }

        [Fact]
        public void Hybrid_SectionComplete()
        {
            MarketingPages.Hosting.HybridActions.EnterSummary(500);
            MarketingPages.Hosting.HybridActions.EnterLink(1000);
            MarketingPages.Hosting.HybridActions.EnterHostingModel(1000);
            MarketingPages.Hosting.PublicCloudActions.ToggleHSCNCheckbox();

            MarketingPages.CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Hybrid").Should().BeTrue();
        }

        [Fact]
        public void Hybrid_SectionIncomplete()
        {
            MarketingPages.CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Hybrid").Should().BeFalse();
        }
    }
}
