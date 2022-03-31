using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Solutions
{
    public static class SolutionListPriceServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(SolutionListPriceService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetSolutionWithListPrice_Valid_Returns(
            Solution solution,
            [Frozen] BuyingCatalogueDbContext dbContext,
            SolutionListPriceService service)
        {
            dbContext.CatalogueItems.Add(solution.CatalogueItem);
            dbContext.SaveChanges();

            var result = await service.GetSolutionWithListPrices(solution.CatalogueItemId);

            result.Should().NotBeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetSolutionWithListPrice_Invalid_ReturnsNull(
            Solution solution,
            SolutionListPriceService service)
        {
            var result = await service.GetSolutionWithListPrices(solution.CatalogueItemId);

            result.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static Task AddListPrice_NullCataloguePrice_ThrowsArgumentNullException(
            Solution solution,
            SolutionListPriceService service)
            => Assert.ThrowsAsync<ArgumentNullException>(() => service.AddListPrice(solution.CatalogueItemId, null));

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddListPrice_Valid_AddsPrice(
            Solution solution,
            [Frozen] BuyingCatalogueDbContext dbContext,
            SolutionListPriceService service)
        {
            solution.CatalogueItem.CataloguePrices = new HashSet<CataloguePrice>();
            dbContext.CatalogueItems.Add(solution.CatalogueItem);
            dbContext.SaveChanges();

            var price = new CataloguePrice
            {
                CataloguePriceCalculationType = CataloguePriceCalculationType.Volume,
                CataloguePriceType = CataloguePriceType.Tiered,
                ProvisioningType = ProvisioningType.Declarative,
                TimeUnit = null,
                CurrencyCode = "GBP",
                PricingUnit = new PricingUnit
                {
                    Definition = "Definition",
                    Description = "Description",
                    RangeDescription = "Range",
                },
            };

            await service.AddListPrice(solution.CatalogueItemId, price);

            solution.CatalogueItem.CataloguePrices.Should().HaveCount(1);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static Task UpdateListPrice_NullCataloguePrice_ThrowsArgumentNullException(
            Solution solution,
            int cataloguePriceId,
            SolutionListPriceService service)
            => Assert.ThrowsAsync<ArgumentNullException>(() => service.UpdateListPrice(solution.CatalogueItemId, cataloguePriceId, null));

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpdateListPrice_Valid_Updates(
            Solution solution,
            CataloguePrice priceUpdates,
            [Frozen] BuyingCatalogueDbContext dbContext,
            SolutionListPriceService service)
        {
            var price = new CataloguePrice
            {
                CataloguePriceCalculationType = CataloguePriceCalculationType.Volume,
                CataloguePriceType = CataloguePriceType.Tiered,
                ProvisioningType = ProvisioningType.Declarative,
                TimeUnit = null,
                CurrencyCode = "GBP",
                PricingUnit = new PricingUnit
                {
                    Definition = "Definition",
                    Description = "Description",
                    RangeDescription = "Range",
                },
            };

            solution.CatalogueItem.CataloguePrices = new HashSet<CataloguePrice>
            {
                price,
            };
            dbContext.CatalogueItems.Add(solution.CatalogueItem);
            dbContext.SaveChanges();

            await service.UpdateListPrice(solution.CatalogueItemId, price.CataloguePriceId, priceUpdates);

            price.ProvisioningType.Should().Be(priceUpdates.ProvisioningType);
            price.CataloguePriceCalculationType.Should().Be(priceUpdates.CataloguePriceCalculationType);
            price.TimeUnit.Should().Be(priceUpdates.TimeUnit);
            price.PricingUnit.RangeDescription.Should().Be(priceUpdates.PricingUnit.RangeDescription);
            price.PricingUnit.Definition.Should().Be(priceUpdates.PricingUnit.Definition);
            price.PricingUnit.Description.Should().Be(priceUpdates.PricingUnit.Description);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetPublicationStatus_UpdatesStatus(
            Solution solution,
            [Frozen] BuyingCatalogueDbContext dbContext,
            SolutionListPriceService service)
        {
            var price = new CataloguePrice
            {
                CataloguePriceCalculationType = CataloguePriceCalculationType.Volume,
                CataloguePriceType = CataloguePriceType.Tiered,
                ProvisioningType = ProvisioningType.Declarative,
                PublishedStatus = PublicationStatus.Draft,
                TimeUnit = null,
                CurrencyCode = "GBP",
                PricingUnit = new PricingUnit
                {
                    Definition = "Definition",
                    Description = "Description",
                    RangeDescription = "Range",
                },
            };

            solution.CatalogueItem.CataloguePrices = new HashSet<CataloguePrice>
            {
                price,
            };
            dbContext.CatalogueItems.Add(solution.CatalogueItem);
            dbContext.SaveChanges();

            await service.SetPublicationStatus(solution.CatalogueItemId, price.CataloguePriceId, PublicationStatus.Published);

            price.PublishedStatus.Should().Be(PublicationStatus.Published);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static Task AddListPriceTier_NullTier_ThrowsArgumentNullException(
            Solution solution,
            int cataloguePriceId,
            SolutionListPriceService service)
            => Assert.ThrowsAsync<ArgumentNullException>(() => service.AddListPriceTier(solution.CatalogueItemId, cataloguePriceId, null));

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddListPriceTier_Valid_AddsTier(
            Solution solution,
            [Frozen] BuyingCatalogueDbContext dbContext,
            SolutionListPriceService service)
        {
            var price = new CataloguePrice
            {
                CataloguePriceCalculationType = CataloguePriceCalculationType.Volume,
                CataloguePriceType = CataloguePriceType.Tiered,
                ProvisioningType = ProvisioningType.Declarative,
                PublishedStatus = PublicationStatus.Draft,
                TimeUnit = null,
                CurrencyCode = "GBP",
                PricingUnit = new PricingUnit
                {
                    Definition = "Definition",
                    Description = "Description",
                    RangeDescription = "Range",
                },
            };

            solution.CatalogueItem.CataloguePrices = new HashSet<CataloguePrice>
            {
                price,
            };
            dbContext.CatalogueItems.Add(solution.CatalogueItem);
            dbContext.SaveChanges();

            var priceTier = new CataloguePriceTier
            {
                LowerRange = 1,
                UpperRange = null,
                Price = 3.12M,
            };

            await service.AddListPriceTier(solution.CatalogueItemId, price.CataloguePriceId, priceTier);

            price.CataloguePriceTiers.Should().HaveCount(1);
            price.CataloguePriceTiers.Should().Contain(priceTier);
        }
    }
}
