using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels.OrderingInformationModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.ResultsModels.OrderingInformationModels;

public static class OrderingInformationItemTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        CatalogueItem catalogueItem,
        CompetitionCatalogueItemPrice price,
        int quantity)
    {
        var model = new OrderingInformationItem(catalogueItem, price, quantity);

        model.CatalogueItemType.Should().Be(catalogueItem.CatalogueItemType);
        model.CatalogueItemName.Should().Be(catalogueItem.Name);
        model.SupplierName.Should().Be(catalogueItem.Supplier.LegalName);
        model.Price.Should().Be(price);
        model.Quantity.Should().Be(quantity);
    }
}
