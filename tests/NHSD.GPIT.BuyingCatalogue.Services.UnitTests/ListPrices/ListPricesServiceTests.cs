using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.Services.ListPrices;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.ListPrices
{
    public static class ListPricesServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ListPricesService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static Task SaveSolutionListPrice_NullModel_ThrowsException(ListPricesService service)
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() => service.SaveListPrice(default, null));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SaveSolutionListPrice_UpdatesDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            CatalogueItem catalogueItem,
            SaveListPriceModel model,
            ListPricesService service)
        {
            catalogueItem.CataloguePrices.Clear();
            context.CatalogueItems.Add(catalogueItem);
            context.PricingUnits.Add(model.PricingUnit);

            await context.SaveChangesAsync();

            await service.SaveListPrice(catalogueItem.Id, model);

            var actual = (await context.CatalogueItems.Include(ci => ci.CataloguePrices).SingleAsync()).CataloguePrices.Single();

            actual.CataloguePriceType.Should().Be(CataloguePriceType.Flat);
            actual.Price.Should().Be(model.Price);
            actual.PricingUnit.Should().Be(model.PricingUnit);
            actual.ProvisioningType.Should().Be(model.ProvisioningType);
            actual.TimeUnit.Should().Be(model.TimeUnit);
            actual.CurrencyCode.Should().Be("GBP");
        }

        [Theory]
        [CommonAutoData]
        public static Task UpdateListPrice_NullModel_ThrowsException(ListPricesService service)
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() => service.UpdateListPrice(default, null));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpdateListPrice_UpdatesDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            CatalogueItem catalogueItem,
            SaveListPriceModel sampleModel,
            ListPricesService service)
        {
            context.CatalogueItems.Add(catalogueItem);
            context.PricingUnits.Add(sampleModel.PricingUnit);

            await context.SaveChangesAsync();

            var model = new SaveListPriceModel
            {
                CataloguePriceId = catalogueItem.CataloguePrices.First().CataloguePriceId,
                Price = sampleModel.Price,
                PricingUnit = sampleModel.PricingUnit,
                ProvisioningType = sampleModel.ProvisioningType,
                TimeUnit = sampleModel.TimeUnit,
            };

            await service.UpdateListPrice(catalogueItem.Id, model);

            var actual = await context.CataloguePrices.AsAsyncEnumerable().Where(cp => cp.CataloguePriceId == model.CataloguePriceId).SingleAsync();

            actual.CataloguePriceType.Should().Be(CataloguePriceType.Flat);
            actual.Price.Should().Be(model.Price);
            actual.PricingUnit.TierName.Should().Be(model.PricingUnit.TierName);
            actual.PricingUnit.Description.Should().Be(model.PricingUnit.Description);
            actual.PricingUnit.Definition.Should().Be(model.PricingUnit.Definition);
            actual.ProvisioningType.Should().Be(model.ProvisioningType);
            actual.TimeUnit.Should().Be(model.TimeUnit);
            actual.CurrencyCode.Should().Be(catalogueItem.CataloguePrices.First().CurrencyCode);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task DeleteListPrice_DeletesFromDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            CatalogueItem catalogueItem,
            ListPricesService service)
        {
            context.CatalogueItems.Add(catalogueItem);
            await context.SaveChangesAsync();

            var cataloguePriceIdToRemove = catalogueItem.CataloguePrices.First().CataloguePriceId;

            await service.DeleteListPrice(catalogueItem.Id, cataloguePriceIdToRemove);

            var actual = await context.CataloguePrices.AsAsyncEnumerable().AnyAsync(cp => cp.CataloguePriceId == cataloguePriceIdToRemove);

            actual.Should().BeFalse();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetCatalogueItemWithPrices_RetrievesFromDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            CatalogueItem catalogueItem,
            ListPricesService service)
        {
            context.CatalogueItems.Add(catalogueItem);
            await context.SaveChangesAsync();

            var actual = await service.GetCatalogueItemWithPrices(catalogueItem.Id);

            actual.Should().NotBeNull();
            actual.CataloguePrices.Should().NotBeNullOrEmpty();
            actual.CataloguePrices.ForEach(p => p.PricingUnit.Should().NotBeNull());
        }
    }
}
