using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
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
            ICollection<CataloguePrice> prices,
            CatalogueItemId solutionId,
            CatalogueItemId additionalServiceId)
        {
            var flatPrices = prices.Where(p => p.CataloguePriceType == CataloguePriceType.Flat).ToList();
            var tieredPrices = prices.Where(p => p.CataloguePriceType == CataloguePriceType.Tiered).ToList();

            var model = new ManageListPricesModel(catalogueItem, prices)
            {
                SolutionType = SolutionType.GPIT,
                SolutionId = solutionId,
                AdditionalServiceId = additionalServiceId,
            };

            model.CatalogueItemId.Should().Be(catalogueItem.Id);
            model.CatalogueItemName.Should().Be(catalogueItem.Name);
            model.FlatPrices.Should().BeEquivalentTo(flatPrices);
            model.TieredPrices.Should().BeEquivalentTo(tieredPrices);
            model.SolutionType.Should().Be(SolutionType.GPIT);
            model.SolutionId.Should().Be(solutionId);
            model.AdditionalServiceId.Should().Be(additionalServiceId);
        }
    }
}
