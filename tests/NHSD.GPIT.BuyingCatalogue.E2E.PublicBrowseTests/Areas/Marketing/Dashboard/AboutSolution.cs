using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.Dashboard
{
    public sealed class AboutSolution : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public AboutSolution(LocalWebApplicationFactory factory) : base(factory, "/marketing/supplier/solution/99999-99")
        {
        }

        [Fact]
        public async System.Threading.Tasks.Task AboutSolution_EditSummaryAsync()
        {
            MarketingPages.DashboardActions.ClickSection("Solution description");

            //Given the Supplier has entered data into any field
            var summary = MarketingPages.SolutionDescriptionActions.SummaryAddText(300);
            var description = MarketingPages.SolutionDescriptionActions.DescriptionAddText(1000);
            var link = MarketingPages.SolutionDescriptionActions.LinkAddText(1000);

            MarketingPages.SolutionDescriptionActions.ClickSave();

            var context = GetBCContext();
            var solution = await context.Solutions.SingleAsync(s => s.Id == "99999-99");
            solution.Summary.Should().Be(summary);
            solution.FullDescription.Should().Be(description);
            solution.AboutUrl.Should().Be(link);
        }
    }
}
