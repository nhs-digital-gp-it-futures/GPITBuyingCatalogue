using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.ListPriceModels
{
    public static class DeleteListPriceModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void Construct_ValidCatalogueItem_SetsPropertiesAsExpected(
            CatalogueItem catalogueItem)
        {
            var model = new DeleteListPriceModel(catalogueItem);

            model.ItemName.Should().Be(catalogueItem.Name);
        }

        [Fact]
        public static void FromCatalogueItem_NullCatalogueItem_ThrowsException()
        {
            var actual = Assert.Throws<ArgumentNullException>(() => new DeleteListPriceModel(null));

            actual.ParamName.Should().Be("item");
        }
    }
}
