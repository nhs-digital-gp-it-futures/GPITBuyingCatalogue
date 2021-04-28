using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.Dashboard
{
    public sealed class ImplementationTimescales : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public ImplementationTimescales(LocalWebApplicationFactory factory) : base(factory, "marketing/supplier/solution/99999-99/section/implementation-timescales")
        {
            using var context = GetBCContext();
            var solution = context.Solutions.Single(s => s.Id == "99999-99");
            solution.ImplementationDetail = string.Empty;
            context.SaveChanges();
        }

        [Fact]
        public async Task ImplementationTimescales_EnterDescription()
        {
            var implementation = MarketingPages.SolutionDescriptionActions.DescriptionAddText(1000);
            MarketingPages.CommonActions.ClickSave();

            using var context = GetBCContext();
            var solution = await context.Solutions.SingleAsync(s => s.Id == "99999-99");
            solution.ImplementationDetail.Should().Be(implementation);
        }

        [Fact]
        public void ImplementationTimescales_MarkedAsComplete()
        {
            MarketingPages.SolutionDescriptionActions.DescriptionAddText(1000);
            MarketingPages.CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Implementation timescales").Should().BeTrue();
        }

        [Fact]
        public void ImplementationTimescales_MarkedAsIncomplete()
        {
            MarketingPages.CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Implementation timescales").Should().BeFalse();
        }
    }
}
