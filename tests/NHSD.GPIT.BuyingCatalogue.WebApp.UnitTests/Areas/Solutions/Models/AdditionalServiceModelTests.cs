using System;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Models
{
    public static class AdditionalServiceModelTests
    {
        [Fact]
        public static void Constructor_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new AdditionalServiceModel(null));

            actual.ParamName.Should().Be("additionalService");
        }

        [Theory]
        [CommonAutoData]
        public static void ValidCatalogueItem_PropertiesCorrectlySet(
            CatalogueItem catalogueItem)
        {
            var model = new AdditionalServiceModel(catalogueItem);

            model.SolutionId.Should().Be(catalogueItem.Id);
            model.Description.Should().Be(catalogueItem.AdditionalService.FullDescription);
            model.Name.Should().Be(catalogueItem.Name);
            model.Prices.Should().BeEquivalentTo(catalogueItem.CataloguePrices.Select(cp => cp.ToString()).ToList());
        }
    }
}
