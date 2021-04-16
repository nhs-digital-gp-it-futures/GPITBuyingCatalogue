using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Actions;
using NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Areas.Solutions
{
    public sealed class GPITFoundationTests : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public GPITFoundationTests(LocalWebApplicationFactory factory) : base(factory, "solutions/futures/foundation")
        {
        }

        [Fact]
        public void GpitFoundationSolutions_ListOfSolutionsDisplayed()
        {
            Pages.SolutionsActions.ListOfSolutionsDisplayed().Should().BeTrue();
        }

        [Fact]
        public void GpitFoundationSolutions_SolutionCardsDisplayRequiredSections()
        {
            Pages.SolutionsActions.ListOfSolutionsDisplayed().Should().BeTrue();
            var solutions = Pages.SolutionsActions.GetSolutionsInList();

            foreach (var solution in solutions)
            {
                SolutionsActions.SolutionListCardDisplaysAllRequiredSections(solution).Should().BeTrue();
            }
        }
    }
}
