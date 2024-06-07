using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
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
                ResultsModel = new SolutionsResultsModel()
                {
                    CatalogueItems = new List<CatalogueItem>(),
                },
            };

            var pageTitle = model.GetPageTitle();
            pageTitle.Title.Should().Be(SolutionsModel.SearchResultsPageTitle.Title);
            pageTitle.Caption.Should().BeNullOrEmpty();
            pageTitle.Advice.Should().Be(SolutionsModel.SearchResultsPageTitle.Advice);
            pageTitle.AdditionalAdvice.Should().BeNullOrEmpty();
        }

        [Theory]
        [CommonAutoData]
        public static void SolutionsModel_SearchWithResults_ReturnsTitleAndAdvice(
            IList<CatalogueItem> solutions)
        {
            var model = new SolutionsModel()
            {
                ResultsModel = new SolutionsResultsModel()
                {
                    CatalogueItems = solutions,
                },
            };

            var pageTitle = model.GetPageTitle();
            pageTitle.Title.Should().Be(SolutionsModel.SearchResultsPageTitle.Title);
            pageTitle.Caption.Should().BeNullOrEmpty();
            pageTitle.Advice.Should().Be(SolutionsModel.SearchResultsPageTitle.Advice);
            pageTitle.AdditionalAdvice.Should().BeNullOrEmpty();
        }

        [Fact]
        public static void SolutionsModel_SearchCriteriaApplied_False()
        {
            var model = new SolutionsModel()
            {
                ResultsModel = new SolutionsResultsModel()
                {
                    Filters = new RequestedFilters(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty),
                },
            };

            model.SearchCriteriaApplied.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void SolutionsModel_SearchCriteriaApplied_True(
            RequestedFilters filters)
        {
            var model = new SolutionsModel()
            {
                ResultsModel = new SolutionsResultsModel()
                {
                    Filters = filters,
                },
            };

            model.SearchCriteriaApplied.Should().BeTrue();
        }
    }
}
