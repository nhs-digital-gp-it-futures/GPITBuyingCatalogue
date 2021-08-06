using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.PublicBrowse;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestBases;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.PublicBrowse.Solutions
{
    public sealed class GPITSearchResults : AnonymousTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private static readonly Dictionary<string, string> Parameters = new()
        {
            { "Capabilities", string.Empty },
        };

        public GPITSearchResults(LocalWebApplicationFactory factory)
            : base(
                  factory,
                  typeof(FuturesController),
                  nameof(FuturesController.SearchResults),
                  Parameters)
        {
        }

        [Fact]
        public void GPITSearchResults_ListOfSolutionsDisplayed()
        {
            PublicBrowsePages.SolutionsActions.ListOfSolutionsDisplayed().Should().BeTrue();
        }

        [Fact]
        public void GPITSearchResults_SolutionCardsDisplayRequiredSections()
        {
            PublicBrowsePages.SolutionsActions.ListOfSolutionsDisplayed().Should().BeTrue();
            var solutions = PublicBrowsePages.SolutionsActions.GetSolutionsInList();

            foreach (var solution in solutions)
            {
                SolutionsActions.SolutionListCardDisplaysAllRequiredSections(solution).Should().BeTrue();
            }
        }

        [Fact]
        public void GPITSearchResults_ClickSolution()
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
