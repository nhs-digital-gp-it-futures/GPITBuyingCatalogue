using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Services.ListPrice;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.ListPrice
{
    public static class DuplicateListPriceServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ListPriceService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task HasDuplicateTieredPrice_Unique_ReturnsFalse(
            Solution solution,
            CataloguePrice price,
            [Frozen] BuyingCatalogueDbContext dbContext,
            ListPriceService service)
        {
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
        [InMemoryDbAutoData]
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
        [InMemoryDbAutoData]
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
        [InMemoryDbAutoData]
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
                priceTier.Price,
                priceTier.LowerRange,
                priceTier.UpperRange);

            result.Should().BeFalse();
        }

        [Theory]
        [InMemoryDbAutoData]
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
                priceTier.Price,
                priceTier.LowerRange,
                priceTier.UpperRange);

            result.Should().BeTrue();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task HasDuplicatePriceTier_DifferentListPrice_ReturnsFalse(
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

            var firstPrice = new CataloguePrice
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
                CataloguePriceTiers = new HashSet<CataloguePriceTier>(),
            };

            var secondPrice = new CataloguePrice
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
                firstPrice,
                secondPrice,
            };
            dbContext.CatalogueItems.Add(solution.CatalogueItem);
            dbContext.SaveChanges();

            var result = await service.HasDuplicatePriceTier(
                solution.CatalogueItemId,
                firstPrice.CataloguePriceId,
                priceTier.Id,
                priceTier.Price,
                priceTier.LowerRange,
                priceTier.UpperRange);

            result.Should().BeFalse();
        }
    }
}
