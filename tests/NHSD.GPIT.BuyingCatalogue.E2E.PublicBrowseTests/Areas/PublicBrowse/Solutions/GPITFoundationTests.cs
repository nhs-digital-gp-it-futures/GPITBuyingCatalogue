using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.PublicBrowse.Areas.Solutions
{
    public sealed class GPITFoundationTests : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public GPITFoundationTests(LocalWebApplicationFactory factory)
            : base(factory, "solutions/futures/foundation")
        {
        }

        [Fact]
        public void GpitFoundationSolutions_ListOfSolutionsDisplayed()
        {
            PublicBrowsePages.SolutionsActions.ListOfSolutionsDisplayed().Should().BeTrue();
        }

        [Fact]
        public void GpitFoundationSolutions_SolutionCardsDisplayRequiredSections()
        {
            PublicBrowsePages.SolutionsActions.ListOfSolutionsDisplayed().Should().BeTrue();
            var solutions = PublicBrowsePages.SolutionsActions.GetSolutionsInList();

            foreach (var solution in solutions)
            {
                SolutionsActions.SolutionListCardDisplaysAllRequiredSections(solution).Should().BeTrue();
            }
        }
    }
}
