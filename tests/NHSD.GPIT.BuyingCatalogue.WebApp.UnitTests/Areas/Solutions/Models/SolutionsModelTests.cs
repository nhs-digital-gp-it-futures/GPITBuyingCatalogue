using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class SolutionsModelTests
    {
        [Theory]
        [InlineData(SolutionType.GPIT)]
        [InlineData(SolutionType.CommunityPharmacy)]
        public static void GetPageTitle_ReturnsExpectedTitle(
            SolutionType solutionType)
        {
            var model = new SolutionsModel { SolutionType = solutionType };

            var pageTitle = model.GetPageTitle();
            pageTitle.Title.Should().Be(solutionType.Description());
            pageTitle.Caption.Should().BeNullOrEmpty();
            pageTitle.Advice.Should().BeNullOrEmpty();
            pageTitle.AdditionalAdvice.Should().BeNullOrEmpty();
        }

        [Fact]
        public static void SolutionsModel_SearchCriteriaApplied_False()
        {
            var model = new SolutionsModel()
            {
                ResultsModel = new SolutionsResultsModel()
                {
                    Filters = new RequestedFilters(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty),
                },
            };

            model.SearchCriteriaApplied.Should().BeFalse();
        }

        [Theory]
        [MockAutoData]
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
