using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class SolutionSearchResultModelTest
    {
        [Theory]
        [CommonAutoData]
        public static void Constructor_Sets_Properties_NoLinks_Defaults_To_False(CatalogueItem catalogueItem)
        {
            var model = new SolutionSearchResultModel(catalogueItem);

            model.CatalogueItem.Should().Be(catalogueItem);
            model.NoLinks.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static void Constructor_Sets_Properties_NoLinks_True(CatalogueItem catalogueItem)
        {
            var model = new SolutionSearchResultModel(catalogueItem, true);

            model.CatalogueItem.Should().Be(catalogueItem);
            model.NoLinks.Should().BeTrue();
        }
    }
}
