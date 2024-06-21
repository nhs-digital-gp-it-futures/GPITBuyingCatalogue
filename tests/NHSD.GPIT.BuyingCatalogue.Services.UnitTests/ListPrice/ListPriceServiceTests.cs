using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Services.ListPrice;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.ListPrice
{
    public static class ListPriceServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ListPriceService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task HasDuplicateTieredPrice_Unique_ReturnsFalse(
            Solution solution,
            CataloguePrice price,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ListPriceService service)
        {
            price.CataloguePriceType = CataloguePriceType.Tiered;

            dbContext.CatalogueItems.Add(solution.CatalogueItem);
            dbContext.SaveChanges();

            var result = await service.HasDuplicateTieredPrice(
                solution.CatalogueItemId,
                price.CataloguePriceId,
                price.ProvisioningType,
                price.CataloguePriceCalculationType,
                price.PricingUnit.Description,
                price.PricingUnit.RangeDescription);

            result.Should().BeFalse();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task HasDuplicateTieredPrice_Self_ReturnsFalse(
            Solution solution,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ListPriceService service)
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

            var result = await service.HasDuplicateTieredPrice(
                solution.CatalogueItemId,
                price.CataloguePriceId,
                price.ProvisioningType,
                price.CataloguePriceCalculationType,
                price.PricingUnit.Description,
                price.PricingUnit.RangeDescription);

            result.Should().BeFalse();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task HasDuplicateTieredPrice_Duplicate_ReturnsTrue(
            Solution solution,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ListPriceService service)
        {
            var existingPrice = new CataloguePrice
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

            var newPrice = new CataloguePrice
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
                existingPrice,
            };
            dbContext.CatalogueItems.Add(solution.CatalogueItem);
            dbContext.SaveChanges();

            var result = await service.HasDuplicateTieredPrice(
                solution.CatalogueItemId,
                newPrice.CataloguePriceId,
                newPrice.ProvisioningType,
                newPrice.CataloguePriceCalculationType,
                newPrice.PricingUnit.Description,
                newPrice.PricingUnit.RangeDescription);

            result.Should().BeTrue();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task HasDuplicatePriceTier_Unique_ReturnsFalse(
            Solution solution,
            CataloguePriceTier priceTier,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ListPriceService service)
        {
            dbContext.CatalogueItems.Add(solution.CatalogueItem);
            dbContext.SaveChanges();

            var result = await service.HasDuplicatePriceTier(
                solution.CatalogueItemId,
                null,
                null,
                priceTier.LowerRange,
                priceTier.UpperRange);

            result.Should().BeFalse();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task HasDuplicatePriceTier_Duplicate_ReturnsTrue(
            Solution solution,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ListPriceService service)
        {
            var existingPriceTier = new CataloguePriceTier
            {
                LowerRange = 1,
                Price = 3.21M,
            };

            var priceTier = new CataloguePriceTier
            {
                LowerRange = 1,
                Price = 3.21M,
            };

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
                CataloguePriceTiers = new HashSet<CataloguePriceTier>
                {
                    existingPriceTier,
                },
            };

            solution.CatalogueItem.CataloguePrices = new HashSet<CataloguePrice>
            {
                price,
            };
            dbContext.CatalogueItems.Add(solution.CatalogueItem);
            dbContext.SaveChanges();

            var result = await service.HasDuplicatePriceTier(
                solution.CatalogueItemId,
                price.CataloguePriceId,
                priceTier.Id,
                priceTier.LowerRange,
                priceTier.UpperRange);

            result.Should().BeTrue();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task HasDuplicateFlatPrice_Duplicate_ReturnsTrue(
            Solution solution,
            CataloguePrice price,
            CataloguePriceTier tier,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ListPriceService service)
        {
            price.CataloguePriceTiers = new HashSet<CataloguePriceTier> { tier };
            price.CataloguePriceType = CataloguePriceType.Flat;
            price.CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed;

            solution.CatalogueItem.CataloguePrices.Add(price);

            dbContext.CatalogueItems.Add(solution.CatalogueItem);
            dbContext.SaveChanges();

            var result = await service.HasDuplicateFlatPrice(
                solution.CatalogueItemId,
                null,
                price.ProvisioningType,
                price.CataloguePriceCalculationType,
                tier.Price,
                price.PricingUnit.Description);

            result.Should().BeTrue();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task HasDuplicateFlatPrice_Self_ReturnsFalse(
            Solution solution,
            CataloguePrice price,
            CataloguePriceTier tier,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ListPriceService service)
        {
            price.CataloguePriceTiers = new HashSet<CataloguePriceTier> { tier };
            price.CataloguePriceType = CataloguePriceType.Flat;
            price.CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed;

            solution.CatalogueItem.CataloguePrices.Add(price);

            dbContext.CatalogueItems.Add(solution.CatalogueItem);
            dbContext.SaveChanges();

            var result = await service.HasDuplicateFlatPrice(
                solution.CatalogueItemId,
                price.CataloguePriceId,
                price.ProvisioningType,
                price.CataloguePriceCalculationType,
                tier.Price,
                price.PricingUnit.Description);

            result.Should().BeFalse();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task HasDuplicateFlatPrice_Unique_ReturnsFalse(
            Solution solution,
            CataloguePrice price,
            CataloguePriceTier tier,
            CataloguePrice newPrice,
            CataloguePriceTier newTier,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ListPriceService service)
        {
            price.CataloguePriceTiers = new HashSet<CataloguePriceTier> { tier };
            price.CataloguePriceType = CataloguePriceType.Flat;
            price.CataloguePriceCalculationType = CataloguePriceCalculationType.SingleFixed;

            solution.CatalogueItem.CataloguePrices.Add(price);

            dbContext.CatalogueItems.Add(solution.CatalogueItem);
            dbContext.SaveChanges();

            var result = await service.HasDuplicateFlatPrice(
                solution.CatalogueItemId,
                null,
                newPrice.ProvisioningType,
                price.CataloguePriceCalculationType,
                newTier.Price,
                newPrice.PricingUnit.Description);

            result.Should().BeFalse();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetSolutionWithListPrice_Valid_Returns(
            Solution solution,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ListPriceService service)
        {
            dbContext.CatalogueItems.Add(solution.CatalogueItem);
            dbContext.SaveChanges();

            var result = await service.GetCatalogueItemWithListPrices(solution.CatalogueItemId);

            result.Should().NotBeNull();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetSolutionWithListPrice_Invalid_ReturnsNull(
            Solution solution,
            ListPriceService service)
        {
            var result = await service.GetCatalogueItemWithListPrices(solution.CatalogueItemId);

            result.Should().BeNull();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static Task AddListPrice_NullCataloguePrice_ThrowsArgumentNullException(
            Solution solution,
            ListPriceService service)
            => Assert.ThrowsAsync<ArgumentNullException>(() => service.AddListPrice(solution.CatalogueItemId, null));

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task AddListPrice_Valid_AddsPrice(
            Solution solution,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ListPriceService service)
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
        [MockInMemoryDbAutoData]
        public static Task UpdateListPrice_NullPricingUnit_ThrowsArgumentNullException(
            Solution solution,
            CataloguePrice price,
            ListPriceService service)
            => Assert.ThrowsAsync<ArgumentNullException>(() => service.UpdateListPrice(
                solution.CatalogueItemId,
                price.CataloguePriceId,
                null,
                price.ProvisioningType,
                price.CataloguePriceCalculationType,
                price.TimeUnit,
                price.CataloguePriceQuantityCalculationType));

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task UpdateListPrice_Valid_Updates(
            Solution solution,
            CataloguePrice priceUpdates,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ListPriceService service)
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
        [MockInMemoryDbAutoData]
        public static Task UpdateFlatListPrice_NullPricingUnit_ThrowsArgumentNullException(
            Solution solution,
            CataloguePrice cataloguePrice,
            decimal price,
            ListPriceService service)
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
        [MockInMemoryDbAutoData]
        public static async Task UpdateFlatListPrice_Valid_Updates(
            Solution solution,
            CataloguePrice priceUpdates,
            decimal price,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ListPriceService service)
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
        [MockInMemoryDbAutoData]
        public static async Task SetPublicationStatus_UpdatesStatus(
            Solution solution,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ListPriceService service)
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
        [MockInMemoryDbAutoData]
        public static Task AddListPriceTier_NullTier_ThrowsArgumentNullException(
            Solution solution,
            int cataloguePriceId,
            ListPriceService service)
            => Assert.ThrowsAsync<ArgumentNullException>(() => service.AddListPriceTier(solution.CatalogueItemId, cataloguePriceId, null));

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task AddListPriceTier_Valid_AddsTier(
            Solution solution,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ListPriceService service)
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
        [MockInMemoryDbAutoData]
        public static async Task UpdateListPriceTier_Valid_Updates(
            Solution solution,
            CataloguePrice price,
            CataloguePriceTier tier,
            CataloguePriceTier updatedTier,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ListPriceService service)
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
        [MockInMemoryDbAutoData]
        public static async Task UpdateTierPrice_Valid_Updates(
            Solution solution,
            CataloguePrice price,
            CataloguePriceTier tier,
            CataloguePriceTier updatedTier,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ListPriceService service)
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
        [MockInMemoryDbAutoData]
        public static async Task DeleteListPrice_Valid_Deletes(
            Solution solution,
            CataloguePrice price,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ListPriceService service)
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
        [MockInMemoryDbAutoData]
        public static async Task DeletePriceTier_Valid_Deletes(
            Solution solution,
            CataloguePrice price,
            CataloguePriceTier tier,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ListPriceService service)
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
