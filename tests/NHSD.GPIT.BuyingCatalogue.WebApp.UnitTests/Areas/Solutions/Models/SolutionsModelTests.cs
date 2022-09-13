using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class SolutionsModelTests
    {
        [Fact]
        public static void SolutionsModel_NoSearch_ReturnsTitleAndAdvice()
        {
            var model = new SolutionsModel()
            {
                SearchSummary = new ServiceContracts.Models.CatalogueFilterSearchSummary(),
            };

            model.TitleText.Should().Be(SolutionsModel.TitleNoSearch);
            model.AdviceText.Should().Be(SolutionsModel.AdviceTextNosearch);
        }

        [Fact]
        public static void SolutionsModel_SearchNoResults_ReturnsTitleAndAdvice()
        {
            var model = new SolutionsModel()
            {
                SearchSummary = new ServiceContracts.Models.CatalogueFilterSearchSummary()
                {
                    SearchTerm = "test",
                },
                CatalogueItems = new List<CatalogueItem>(),
            };

            model.TitleText.Should().Be(SolutionsModel.TitleSearchNoResults);
            model.AdviceText.Should().Be(SolutionsModel.AdviceTextSearchNoresults);
        }

        [Theory]
        [CommonAutoData]
        public static void SolutionsModel_SearchWithResults_ReturnsTitleAndAdvice(
            IList<CatalogueItem> solutions)
        {
            var model = new SolutionsModel()
            {
                SearchSummary = new ServiceContracts.Models.CatalogueFilterSearchSummary()
                {
                    SearchTerm = "test",
                },
                CatalogueItems = solutions,
            };

            model.TitleText.Should().Be(SolutionsModel.TitleSearchResults);
            model.AdviceText.Should().Be(SolutionsModel.AdviceTextSearchResults);
        }
    }
}
