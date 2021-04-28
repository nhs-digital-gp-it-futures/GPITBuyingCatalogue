using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.Dashboard
{
    public sealed class Integrations : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public Integrations(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/integrations")
        {
            using var context = GetBCContext();
            var solution = context.Solutions.Single(s => s.Id == "99999-99");
            solution.IntegrationsUrl = string.Empty;
            context.SaveChanges();
        }

        [Fact]
        public async Task Integrations_AddUrl()
        {
            var link = MarketingPages.SolutionDescriptionActions.LinkAddText(1000);
            MarketingPages.CommonActions.ClickSave();

            using var context = GetBCContext();
            var solution = await context.Solutions.SingleAsync(s => s.Id == "99999-99");
            solution.IntegrationsUrl.Should().Be(link);
        }

        [Fact]
        public void Integrations_SectionMarkedAsComplete()
        {
            driver.Navigate().Refresh();

            var link = MarketingPages.SolutionDescriptionActions.LinkAddText(1000);
            MarketingPages.CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Integrations").Should().BeTrue();
        }

        [Fact]
        public void Integrations_SectionMarkedAsIncomplete()
        {
            driver.Navigate().Refresh();

            MarketingPages.CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Integrations").Should().BeFalse();
        }
    }
}
