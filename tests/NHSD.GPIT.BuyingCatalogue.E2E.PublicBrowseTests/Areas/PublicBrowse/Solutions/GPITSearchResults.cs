using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using System;
using System.Linq;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solutions
{
    public sealed class GPITSearchResults : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public GPITSearchResults(LocalWebApplicationFactory factory) : base(factory, "/Solutions/Futures/SearchResults")
        {
        }

        [Fact]
        public void DFOCVCSolutions_ListOfSolutionsDisplayed()
        {
            PublicBrowsePages.SolutionsActions.ListOfSolutionsDisplayed().Should().BeTrue();
        }

        [Fact]
        public void DFOCVCSolutions_SolutionCardsDisplayRequiredSections()
        {
            PublicBrowsePages.SolutionsActions.ListOfSolutionsDisplayed().Should().BeTrue();
            var solutions = PublicBrowsePages.SolutionsActions.GetSolutionsInList();

            foreach (var solution in solutions)
            {
                SolutionsActions.SolutionListCardDisplaysAllRequiredSections(solution).Should().BeTrue();
            }
        }

        [Fact]
        public void DFOCVCSolutions_ClickSolution()
        {
            PublicBrowsePages.SolutionsActions.ListOfSolutionsDisplayed().Should().BeTrue();
            var solutions = PublicBrowsePages.SolutionsActions.GetSolutionsInList();
            var rng = new Random();
            var selectedSolution = solutions.ElementAt(rng.Next(solutions.Count()));

            SolutionsActions.ClickSolutionName(selectedSolution);

            PublicBrowsePages.SolutionAction.WaitUntilSolutionNameDisplayed();
        }
    }
}
