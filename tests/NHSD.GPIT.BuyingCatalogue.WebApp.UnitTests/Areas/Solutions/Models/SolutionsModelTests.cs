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
        public static void SolutionsModel_SearchNoResults_ReturnsTitleAndAdvice()
        {
            var model = new SolutionsModel()
            {
                CatalogueItems = new List<CatalogueItem>(),
            };

            var pageTitle = model.GetPageTitle();
            pageTitle.Title.Should().Be(SolutionsModel.SearchResultsPageTitle.Title);
            pageTitle.Caption.Should().BeNullOrEmpty();
            pageTitle.Advice.Should().Be(SolutionsModel.SearchResultsPageTitle.Advice);
            pageTitle.AdditionalAdvice.Should().BeNullOrEmpty();
        }

        [Fact]
        public static void SolutionsModel_SearchNoResultsFilter_ReturnsTitle_Caption_And_Advice()
        {
            var filterName = "Filter Name";

            var model = new SolutionsModel()
            {
                FilterName = filterName,
                CatalogueItems = new List<CatalogueItem>(),
            };

            var pageTitle = model.GetPageTitle();
            pageTitle.Title.Should().Be(SolutionsModel.SearchNoResultsFilterPageTitle.Title);
            pageTitle.Caption.Should().Be(filterName);
            pageTitle.Advice.Should().Be(SolutionsModel.SearchNoResultsFilterPageTitle.Advice);
            pageTitle.AdditionalAdvice.Should().BeNullOrEmpty();
        }

        [Theory]
        [CommonAutoData]
        public static void SolutionsModel_SearchWithResults_ReturnsTitleAndAdvice(
            IList<CatalogueItem> solutions)
        {
            var model = new SolutionsModel()
            {
                CatalogueItems = solutions,
            };

            var pageTitle = model.GetPageTitle();
            pageTitle.Title.Should().Be(SolutionsModel.SearchResultsPageTitle.Title);
            pageTitle.Caption.Should().BeNullOrEmpty();
            pageTitle.Advice.Should().Be(SolutionsModel.SearchResultsPageTitle.Advice);
            pageTitle.AdditionalAdvice.Should().BeNullOrEmpty();
        }

        [Theory]
        [CommonAutoData]
        public static void SolutionsModel_SearchWithResultsFilter_ReturnsTitle_Caption_And_Advice(
            IList<CatalogueItem> solutions)
        {
            var filterName = "Filter Name";

            var model = new SolutionsModel()
            {
                FilterName = filterName,
                CatalogueItems = solutions,
            };

            var pageTitle = model.GetPageTitle();
            pageTitle.Title.Should().Be(SolutionsModel.SearchResultsFilterPageTitle.Title);
            pageTitle.Caption.Should().Be(filterName);
            pageTitle.Advice.Should().Be(SolutionsModel.SearchResultsFilterPageTitle.Advice);
            pageTitle.AdditionalAdvice.Should().BeNullOrEmpty();
        }
    }
}
