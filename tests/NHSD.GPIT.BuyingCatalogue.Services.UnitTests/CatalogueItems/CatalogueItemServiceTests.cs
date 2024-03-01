using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Services.CatalogueItems;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.CatalogueItems;

public static class CatalogueItemServiceTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(CatalogueItemService).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task GetCatalogueItemName_ReturnsCatalogueItemName(
        CatalogueItem catalogueItem,
        [Frozen] BuyingCatalogueDbContext context,
        CatalogueItemService service)
    {
        context.CatalogueItems.Add(catalogueItem);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var catalogueItemName = await service.GetCatalogueItemName(catalogueItem.Id);

        catalogueItemName.Should().Be(catalogueItem.Name);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task GetCatalogueItem_ReturnsCatalogueItem(
        CatalogueItem catalogueItem,
        [Frozen] BuyingCatalogueDbContext context,
        CatalogueItemService service)
    {
        context.CatalogueItems.Add(catalogueItem);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var result = await service.GetCatalogueItem(catalogueItem.Id);

        result.Id.Should().Be(catalogueItem.Id);
        result.Name.Should().Be(catalogueItem.Name);
    }
}
