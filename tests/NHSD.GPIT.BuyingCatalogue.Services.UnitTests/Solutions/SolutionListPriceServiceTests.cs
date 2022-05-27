using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
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

            var result = await service.GetCatalogueItemWithListPrices(solution.CatalogueItemId);

            result.Should().NotBeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetSolutionWithListPrice_Invalid_ReturnsNull(
            Solution solution,
            SolutionListPriceService service)
        {
            var result = await service.GetCatalogueItemWithListPrices(solution.CatalogueItemId);

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
        public static Task UpdateListPrice_NullPricingUnit_ThrowsArgumentNullException(
            Solution solution,
            CataloguePrice price,
            SolutionListPriceService service)
            => Assert.ThrowsAsync<ArgumentNullException>(() => service.UpdateListPrice(
                solution.CatalogueItemId,
                price.CataloguePriceId,
                null,
                price.ProvisioningType,
                price.CataloguePriceCalculationType,
                price.TimeUnit,
                price.CataloguePriceQuantityCalculationType));

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

            await service.UpdateListPrice(
                solution.CatalogueItemId,
                price.CataloguePriceId,
                priceUpdates.PricingUnit,
                priceUpdates.ProvisioningType,
                priceUpdates.CataloguePriceCalculationType,
                priceUpdates.TimeUnit,
                priceUpdates.CataloguePriceQuantityCalculationType);

            price.ProvisioningType.Should().Be(priceUpdates.ProvisioningType);
            price.CataloguePriceCalculationType.Should().Be(priceUpdates.CataloguePriceCalculationType);
            price.TimeUnit.Should().Be(priceUpdates.TimeUnit);
            price.PricingUnit.RangeDescription.Should().Be(priceUpdates.PricingUnit.RangeDescription);
            price.PricingUnit.Definition.Should().Be(priceUpdates.PricingUnit.Definition);
            price.PricingUnit.Description.Should().Be(priceUpdates.PricingUnit.Description);
            price.CataloguePriceQuantityCalculationType.Should().Be(priceUpdates.CataloguePriceQuantityCalculationType);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static Task UpdateFlatListPrice_NullPricingUnit_ThrowsArgumentNullException(
            Solution solution,
            CataloguePrice cataloguePrice,
            decimal price,
            SolutionListPriceService service)
            => Assert.ThrowsAsync<ArgumentNullException>(() => service.UpdateListPrice(
                solution.CatalogueItemId,
                cataloguePrice.CataloguePriceId,
                null,
                cataloguePrice.ProvisioningType,
                cataloguePrice.CataloguePriceCalculationType,
                cataloguePrice.TimeUnit,
                cataloguePrice.CataloguePriceQuantityCalculationType,
                price));

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpdateFlatListPrice_Valid_Updates(
            Solution solution,
            CataloguePrice priceUpdates,
            decimal price,
            [Frozen] BuyingCatalogueDbContext dbContext,
            SolutionListPriceService service)
        {
            var cataloguePrice = new CataloguePrice
            {
                CataloguePriceCalculationType = CataloguePriceCalculationType.Volume,
                CataloguePriceType = CataloguePriceType.Flat,
                ProvisioningType = ProvisioningType.Declarative,
                TimeUnit = null,
                CurrencyCode = "GBP",
                PricingUnit = new PricingUnit
                {
                    Definition = "Definition",
                    Description = "Description",
                    RangeDescription = "Range",
                },
                CataloguePriceTiers = new HashSet<CataloguePriceTier>
                {
                    new()
                    {
                        Price = 3.14M,
                        LowerRange = 1,
                        UpperRange = null,
                    },
                },
            };

            solution.CatalogueItem.CataloguePrices = new HashSet<CataloguePrice>
            {
                cataloguePrice,
            };
            dbContext.CatalogueItems.Add(solution.CatalogueItem);
            dbContext.SaveChanges();

            await service.UpdateListPrice(
                solution.CatalogueItemId,
                cataloguePrice.CataloguePriceId,
                priceUpdates.PricingUnit,
                priceUpdates.ProvisioningType,
                priceUpdates.CataloguePriceCalculationType,
                priceUpdates.TimeUnit,
                priceUpdates.CataloguePriceQuantityCalculationType,
                price);

            cataloguePrice.ProvisioningType.Should().Be(priceUpdates.ProvisioningType);
            cataloguePrice.CataloguePriceCalculationType.Should().Be(priceUpdates.CataloguePriceCalculationType);
            cataloguePrice.TimeUnit.Should().Be(priceUpdates.TimeUnit);
            cataloguePrice.PricingUnit.RangeDescription.Should().Be(priceUpdates.PricingUnit.RangeDescription);
            cataloguePrice.PricingUnit.Definition.Should().Be(priceUpdates.PricingUnit.Definition);
            cataloguePrice.PricingUnit.Description.Should().Be(priceUpdates.PricingUnit.Description);
            cataloguePrice.CataloguePriceQuantityCalculationType.Should()
                .Be(priceUpdates.CataloguePriceQuantityCalculationType);
            cataloguePrice.CataloguePriceTiers.First().Price.Should().Be(price);
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

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpdateListPriceTier_Valid_Updates(
            Solution solution,
            CataloguePrice price,
            CataloguePriceTier tier,
            CataloguePriceTier updatedTier,
            [Frozen] BuyingCatalogueDbContext dbContext,
            SolutionListPriceService service)
        {
            price.CataloguePriceTiers.Add(tier);
            solution.CatalogueItem.CataloguePrices = new HashSet<CataloguePrice>
            {
                price,
            };

            dbContext.Add(solution.CatalogueItem);
            dbContext.SaveChanges();

            await service.UpdateListPriceTier(
                solution.CatalogueItemId,
                price.CataloguePriceId,
                tier.Id,
                updatedTier.Price,
                updatedTier.LowerRange,
                updatedTier.UpperRange);

            tier.Price.Should().Be(updatedTier.Price);
            tier.LowerRange.Should().Be(updatedTier.LowerRange);
            tier.UpperRange.Should().Be(updatedTier.UpperRange);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpdateTierPrice_Valid_Updates(
            Solution solution,
            CataloguePrice price,
            CataloguePriceTier tier,
            CataloguePriceTier updatedTier,
            [Frozen] BuyingCatalogueDbContext dbContext,
            SolutionListPriceService service)
        {
            price.CataloguePriceTiers.Add(tier);
            solution.CatalogueItem.CataloguePrices = new HashSet<CataloguePrice>
            {
                price,
            };

            dbContext.Add(solution.CatalogueItem);
            dbContext.SaveChanges();

            await service.UpdateTierPrice(
                solution.CatalogueItemId,
                price.CataloguePriceId,
                tier.Id,
                updatedTier.Price);

            tier.Price.Should().Be(updatedTier.Price);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task DeleteListPrice_Valid_Deletes(
            Solution solution,
            CataloguePrice price,
            [Frozen] BuyingCatalogueDbContext dbContext,
            SolutionListPriceService service)
        {
            solution.CatalogueItem.CataloguePrices = new HashSet<CataloguePrice>
            {
                price,
            };

            dbContext.Add(solution.CatalogueItem);
            dbContext.SaveChanges();

            solution.CatalogueItem.CataloguePrices.Contains(price).Should().BeTrue();

            await service.DeleteListPrice(
                solution.CatalogueItemId,
                price.CataloguePriceId);

            solution.CatalogueItem.CataloguePrices.Contains(price).Should().BeFalse();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task DeletePriceTier_Valid_Deletes(
            Solution solution,
            CataloguePrice price,
            CataloguePriceTier tier,
            [Frozen] BuyingCatalogueDbContext dbContext,
            SolutionListPriceService service)
        {
            price.CataloguePriceTiers.Add(tier);
            solution.CatalogueItem.CataloguePrices = new HashSet<CataloguePrice>
            {
                price,
            };

            dbContext.Add(solution.CatalogueItem);
            dbContext.SaveChanges();

            price.CataloguePriceTiers.Contains(tier).Should().BeTrue();

            await service.DeletePriceTier(
                solution.CatalogueItemId,
                price.CataloguePriceId,
                tier.Id);

            price.CataloguePriceTiers.Contains(tier).Should().BeFalse();
        }
    }
}
