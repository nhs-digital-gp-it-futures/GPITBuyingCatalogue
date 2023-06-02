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

            var pageTitle = model.GetPageTitle();
            pageTitle.Title.Should().Be(SolutionsModel.NoSearchPageTitle.Title);
            pageTitle.Advice.Should().Be(SolutionsModel.NoSearchPageTitle.Advice);
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

            var pageTitle = model.GetPageTitle();
            pageTitle.Title.Should().Be(SolutionsModel.SearchNoResultsPageTitle.Title);
            pageTitle.Advice.Should().Be(SolutionsModel.SearchNoResultsPageTitle.Advice);
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

            var pageTitle = model.GetPageTitle();
            pageTitle.Title.Should().Be(SolutionsModel.SearchResultsPageTitle.Title);
            pageTitle.Advice.Should().Be(SolutionsModel.SearchResultsPageTitle.Advice);
        }
    }
}
