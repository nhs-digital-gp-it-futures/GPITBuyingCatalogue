using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CatalogueSolutionsModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models
{
    public static class CatalogueSolutionsModelTests
    {
        [Fact]
        public static void NewCatalogueSolutionsModelWithSolution_StandardCall_ResultAsExpected()
        {
            var solutions = new List<CatalogueItem>()
            {
                new CatalogueItem() { Name = "Test 1" },
                new CatalogueItem() { Name = "Test 2" },
            };

            var actual = new CatalogueSolutionsModel(solutions);

            actual.Solutions.Should().NotBeNullOrEmpty();
            actual.Solutions[0].Name.Should().Be(solutions[0].Name);
            actual.Solutions[1].Name.Should().Be(solutions[1].Name);
        }

        [Fact]
        public static void NewCatalogueSolutionsModel_SetSolutionFunctionCall_ResultAsExpected()
        {
            var solutions = new List<CatalogueItem>()
            {
                new CatalogueItem() { Name = "Test 1" },
                new CatalogueItem() { Name = "Test 2" },
            };

            var actual = new CatalogueSolutionsModel();

            actual.SetSolutions(solutions);

            actual.Solutions.Should().NotBeNullOrEmpty();
            actual.Solutions[0].Name.Should().Be(solutions[0].Name);
            actual.Solutions[1].Name.Should().Be(solutions[1].Name);
        }

        [Fact]
        public static void NewCatalogueSolutionsModel_SetSolutionWithNull_ResultAsExpected()
        {
            var actual = new CatalogueSolutionsModel();

            actual.SetSolutions(null);

            actual.Solutions.Should().NotBeNull();
            actual.Solutions.Should().BeEmpty();
        }
    }
}
