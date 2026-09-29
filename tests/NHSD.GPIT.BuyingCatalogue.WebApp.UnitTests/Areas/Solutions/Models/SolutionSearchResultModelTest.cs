using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class SolutionSearchResultModelTest
    {
        [Theory]
        [MockAutoData]
        public static void Constructor_Sets_Properties_NoLinks_Defaults_To_False(CatalogueItem catalogueItem)
        {
            var model = new SolutionSearchResultModel(catalogueItem);

            model.CatalogueItem.Should().Be(catalogueItem);
            model.NoLinks.Should().BeFalse();
            model.Filters.Should().BeNull();
        }

        [Theory]
        [MockAutoData]
        public static void Constructor_Sets_Properties_NoLinks_True(CatalogueItem catalogueItem, RequestedFilters filters)
        {
            var model = new SolutionSearchResultModel(catalogueItem, true) { Filters = filters };

            model.CatalogueItem.Should().Be(catalogueItem);
            model.NoLinks.Should().BeTrue();
            model.Filters.Should().BeEquivalentTo(filters);
        }
    }
}
