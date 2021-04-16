using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Actions;
using NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using System;
using System.Linq;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Areas.Solutions
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
            Pages.SolutionsActions.ListOfSolutionsDisplayed().Should().BeTrue();
        }

        [Fact]
        public void DFOCVCSolutions_SolutionCardsDisplayRequiredSections()
        {
            Pages.SolutionsActions.ListOfSolutionsDisplayed().Should().BeTrue();
            var solutions = Pages.SolutionsActions.GetSolutionsInList();

            foreach (var solution in solutions)
            {
                SolutionsActions.SolutionListCardDisplaysAllRequiredSections(solution).Should().BeTrue();
            }
        }

        [Fact(Skip = "Need seeding data in IM db")]
        public void DFOCVCSolutions_ClickSolution()
        {
            var db = GetContext<BuyingCatalogueDbContext>();

            Pages.SolutionsActions.ListOfSolutionsDisplayed().Should().BeTrue();
            var solutions = Pages.SolutionsActions.GetSolutionsInList();
            var rng = new Random();
            var selectedSolution = solutions.ElementAt(rng.Next(solutions.Count()));

            SolutionsActions.ClickSolution(selectedSolution);
        }
    }
}
