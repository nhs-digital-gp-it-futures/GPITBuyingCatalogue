using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Marketing.Dashboard
{
    public sealed class AboutSolution : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public AboutSolution(LocalWebApplicationFactory factory) : base(factory, "/marketing/supplier/solution/99999-99/section/solution-description")
        {
        }

        [Fact]
        public async Task AboutSolution_EditAllFieldsAsync()
        {
            var summary = MarketingPages.SolutionDescriptionActions.SummaryAddText(300);
            var description = MarketingPages.SolutionDescriptionActions.DescriptionAddText(1000);
            var link = MarketingPages.SolutionDescriptionActions.LinkAddText(1000);

            MarketingPages.CommonActions.ClickSave();

            using var context = GetBCContext();
            var solution = await context.Solutions.SingleAsync(s => s.Id == "99999-99");
            solution.Summary.Should().Be(summary);
            solution.FullDescription.Should().Be(description);
            solution.AboutUrl.Should().Be(link);
        }

        [Fact]
        public void AboutSolution_SectionMarkedAsComplete()
        {
            MarketingPages.SolutionDescriptionActions.SummaryAddText(300);
            MarketingPages.SolutionDescriptionActions.DescriptionAddText(1000);
            MarketingPages.SolutionDescriptionActions.LinkAddText(1000);

            MarketingPages.CommonActions.ClickSave();

            MarketingPages.DashboardActions.SectionMarkedComplete("Solution description").Should().BeTrue();
        }

        [Fact]
        public async Task AboutSolution_SectionMarkedAsIncompleteAsync()
        {
            using var context = GetBCContext();
            var solution = await context.Solutions.SingleAsync(s => s.Id == "99999-99");
            solution.Summary = string.Empty;
            solution.FullDescription = string.Empty;
            solution.AboutUrl = string.Empty;

            await context.SaveChangesAsync();

            MarketingPages.CommonActions.ClickGoBackLink();

            MarketingPages.DashboardActions.SectionMarkedComplete("Solution description").Should().BeFalse();
        }

        [Fact(Skip = "Validation not implemented")]
        public async Task AboutSolution_SummaryLeftEmpty()
        {
            using var context = GetBCContext();
            var solution = await context.Solutions.SingleAsync(s => s.Id == "99999-99");
            solution.Summary = string.Empty;

            await context.SaveChangesAsync();
            driver.Navigate().Refresh();

            MarketingPages.CommonActions.ClickSave();

            MarketingPages.SolutionDescriptionActions.ErrorMessageDisplayed().Should().BeTrue();
        }

        [Theory(Skip = "Validation not implemented")]
        [InlineData(301, 1000, 1000)]
        [InlineData(300, 1001, 1000)]
        [InlineData(300, 1000, 1001)]
        public void AboutSolution_ExceedsMaxLength(int summaryCount, int descriptionCount, int linkCount)
        {
            MarketingPages.SolutionDescriptionActions.SummaryAddText(summaryCount);
            MarketingPages.SolutionDescriptionActions.DescriptionAddText(descriptionCount);
            MarketingPages.SolutionDescriptionActions.LinkAddText(linkCount);

            MarketingPages.CommonActions.ClickSave();

            MarketingPages.SolutionDescriptionActions.ErrorMessageDisplayed().Should().BeTrue();
        }
    }
}
