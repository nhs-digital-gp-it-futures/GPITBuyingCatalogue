using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Models.ListPriceModels
{
    public static class ListPriceTypeModelTests
    {
        [Theory]
        [MockAutoData]
        public static void Constructing_AssignsProperties(
            CatalogueItem catalogueItem)
        {
            var model = new ListPriceTypeModel(catalogueItem);

            model.CatalogueItemName.Should().Be(catalogueItem.Name);
        }
    }
}
