using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System.Threading.Tasks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.Hosting
{
    public sealed class OnPremise : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public OnPremise(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/hosting-type-on-premise")
        {
            ClearClientApplication("99999-99");
            driver.Navigate().Refresh();
        }

        [Fact]
        public async Task OnPremise_CompleteAllFields()
        {
            var summary = MarketingPages.Hosting.OnPremiseActions.EnterSummary(500);
            var link = MarketingPages.Hosting.OnPremiseActions.EnterLink(1000);
            var hostingModel = MarketingPages.Hosting.OnPremiseActions.EnterHostingModel(1000);
            MarketingPages.Hosting.PublicCloudActions.ToggleHSCNCheckbox();

            MarketingPages.CommonActions.ClickSave();

            using var context = GetBCContext();
            var hosting = (await context.Solutions.SingleAsync(s => s.Id == "99999-99")).Hosting;

            hosting.Should().ContainEquivalentOf($"\"OnPremise\":{{\"Summary\":\"{summary}\",\"Link\":\"{link}\",\"HostingModel\":\"{hostingModel}\"");
        }

        [Fact]
        public void OnPremise_SectionComplete()
        {
            MarketingPages.Hosting.OnPremiseActions.EnterSummary(500);
            MarketingPages.Hosting.OnPremiseActions.EnterLink(1000);
            MarketingPages.Hosting.OnPremiseActions.EnterHostingModel(1000);
            MarketingPages.Hosting.PublicCloudActions.ToggleHSCNCheckbox();

            MarketingPages.CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("On premise").Should().BeTrue();
        }

        [Fact]
        public void OnPremise_SectionIncomplete()
        {
            MarketingPages.CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("On premise").Should().BeFalse();
        }
    }
}
