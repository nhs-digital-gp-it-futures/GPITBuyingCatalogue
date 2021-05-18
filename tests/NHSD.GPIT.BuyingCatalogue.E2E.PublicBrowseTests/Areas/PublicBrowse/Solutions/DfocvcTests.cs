using System;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.PublicBrowse.Areas.Solutions
{
    public sealed class DfocvcTests : TestBase, IClassFixture<LocalWebApplicationFactory>
    {
        public DfocvcTests(LocalWebApplicationFactory factory)
            : base(factory, "solutions/dfocvc")
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
