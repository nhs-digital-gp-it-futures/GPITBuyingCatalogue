using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solutions
{
    public sealed class GPITFoundationTests : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public GPITFoundationTests(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(FuturesController),
                  nameof(FuturesController.Foundation),
                  null)
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
