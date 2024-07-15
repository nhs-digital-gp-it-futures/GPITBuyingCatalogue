using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.ListPriceModels
{
    public static class ManageListPricesModelTests
    {
        [Theory]
        [MockAutoData]
        public static void Constructing_AssignsProperties(
            CatalogueItem catalogueItem,
            ICollection<CataloguePrice> prices)
        {
            var flatPrices = prices.Where(p => p.CataloguePriceType == CataloguePriceType.Flat).ToList();
            var tieredPrices = prices.Where(p => p.CataloguePriceType == CataloguePriceType.Tiered).ToList();

            var model = new ManageListPricesModel(catalogueItem, prices);

            model.CatalogueItemId.Should().Be(catalogueItem.Id);
            model.CatalogueItemName.Should().Be(catalogueItem.Name);
            model.FlatPrices.Should().BeEquivalentTo(flatPrices);
            model.TieredPrices.Should().BeEquivalentTo(tieredPrices);
        }
    }
}
