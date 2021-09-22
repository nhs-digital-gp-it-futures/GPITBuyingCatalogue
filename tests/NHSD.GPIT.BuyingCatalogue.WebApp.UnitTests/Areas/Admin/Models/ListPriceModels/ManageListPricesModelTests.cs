using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.ListPriceModels
{
    public static class ManageListPricesModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void Construct_ValidCatalogueItem_SetsPropertiesAsExpected(
            CatalogueItem catalogueItem)
        {
            var manageListPricesModel = new ManageListPricesModel(catalogueItem);

            manageListPricesModel.CataloguePrices.Should().BeEquivalentTo(catalogueItem.CataloguePrices);
            manageListPricesModel.CatalogueItemId.Should().Be(catalogueItem.Id);
            manageListPricesModel.CatalogueName.Should().Be(catalogueItem.Name);
        }
    }
}
