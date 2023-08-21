using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Orders
{
    public static class OrderPriceServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderPriceService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static void UpsertPrice_RecipientIsNull_ThrowsException(OrderPriceService service)
        {
            FluentActions
                .Awaiting(() => service.UpsertPrice(0, null, new List<PricingTierDto>()))
                .Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("price");
        }

        [Theory]
        [InMemoryDbAutoData]
        public static void UpsertPrice_AgreedPricesIsNull_ThrowsException(CataloguePrice price, OrderPriceService service)
        {
            FluentActions
                .Awaiting(() => service.UpsertPrice(0, price, null))
                .Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("agreedPrices");
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpsertPrice_OrderItemNotInDatabase_NoActionTaken(
            [Frozen] BuyingCatalogueDbContext context,
            Order order,
            CataloguePrice price,
            OrderPriceService service)
        {
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            await service.UpsertPrice(order.Id, price, new List<PricingTierDto>());

            var actual = context.OrderItemPrices
                .FirstOrDefault(x => x.OrderId == order.Id
                    && x.CatalogueItemId == price.CatalogueItemId);

            actual.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpsertPrice_OrderItemInDatabase_AddsPrice(
            [Frozen] BuyingCatalogueDbContext context,
            Order order,
            OrderPriceService service)
        {
            var orderItem = order.OrderItems.First();

            orderItem.OrderItemPrice = null;

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var price = orderItem.CatalogueItem.CataloguePrices.First();

            await service.UpsertPrice(order.Id, price, new List<PricingTierDto>());

            var actual = context.OrderItemPrices
                .FirstOrDefault(x => x.OrderId == order.Id
                    && x.CatalogueItemId == price.CatalogueItemId);

            actual.Should().NotBeNull();
            actual!.OrderId.Should().Be(order.Id);
            actual.CatalogueItemId.Should().Be(price.CatalogueItemId);
            actual.CataloguePriceId.Should().Be(price.CataloguePriceId);
            actual.CataloguePriceCalculationType.Should().Be(price.CataloguePriceCalculationType);
            actual.CataloguePriceType.Should().Be(price.CataloguePriceType);
            actual.CurrencyCode.Should().Be(price.CurrencyCode);
            actual.Description.Should().Be(price.PricingUnit.Description);
            actual.BillingPeriod.Should().Be(price.TimeUnit);
            actual.ProvisioningType.Should().Be(price.ProvisioningType);
            actual.RangeDescription.Should().Be(price.PricingUnit.RangeDescription);

            foreach (var tier in actual.OrderItemPriceTiers)
            {
                var pricingTier = price.CataloguePriceTiers.First(x => x.LowerRange == tier.LowerRange && x.UpperRange == tier.UpperRange);

                tier.ListPrice.Should().Be(pricingTier.Price);
                tier.Price.Should().Be(pricingTier.Price);
            }
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpsertPrice_OrderItemInDatabase_WithExistingPrice_RemovesExistingPriceAndTiers(
            CatalogueItem existingCatalogueItem,
            CataloguePrice existingCataloguePrice,
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] Mock<IOrderQuantityService> mockOrderQuantityService,
            OrderPriceService service)
        {
            existingCataloguePrice.CatalogueItemId = existingCatalogueItem.Id;

            var orderItem = order.OrderItems.First();

            orderItem.OrderItemPrice.CataloguePriceId = existingCataloguePrice.CataloguePriceId;

            context.CatalogueItems.Add(existingCatalogueItem);
            context.CataloguePrices.Add(existingCataloguePrice);
            context.Orders.Add(order);

            await context.SaveChangesAsync();

            var existingPrice = context.OrderItemPrices
                .FirstOrDefault(x => x.OrderId == order.Id
                    && x.CatalogueItemId == orderItem.CatalogueItemId);

            existingPrice.Should().NotBeNull();

            var price = orderItem.CatalogueItem.CataloguePrices.First();

            mockOrderQuantityService
                .Setup(x => x.ResetItemQuantities(order.Id, orderItem.CatalogueItemId))
                .Verifiable();

            await service.UpsertPrice(order.Id, price, new List<PricingTierDto>());

            mockOrderQuantityService.VerifyAll();

            var actual = context.OrderItemPrices
                .FirstOrDefault(x => x.OrderId == order.Id
                    && x.CatalogueItemId == orderItem.CatalogueItemId);

            actual.Should().NotBeNull();
            actual.Should().NotBe(existingPrice);

            actual!.OrderId.Should().Be(order.Id);
            actual.CatalogueItemId.Should().Be(price.CatalogueItemId);
            actual.CataloguePriceId.Should().Be(price.CataloguePriceId);
            actual.CataloguePriceCalculationType.Should().Be(price.CataloguePriceCalculationType);
            actual.CataloguePriceType.Should().Be(price.CataloguePriceType);
            actual.CurrencyCode.Should().Be(price.CurrencyCode);
            actual.Description.Should().Be(price.PricingUnit.Description);
            actual.BillingPeriod.Should().Be(price.TimeUnit);
            actual.ProvisioningType.Should().Be(price.ProvisioningType);
            actual.RangeDescription.Should().Be(price.PricingUnit.RangeDescription);

            foreach (var tier in actual.OrderItemPriceTiers)
            {
                var pricingTier = price.CataloguePriceTiers.First(x => x.LowerRange == tier.LowerRange && x.UpperRange == tier.UpperRange);

                tier.ListPrice.Should().Be(pricingTier.Price);
                tier.Price.Should().Be(pricingTier.Price);
            }
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpsertPrice_WithAgreedPrices_OrderItemInDatabase_AddsPrice(
            [Frozen] BuyingCatalogueDbContext context,
            Order order,
            OrderPriceService service)
        {
            var orderItem = order.OrderItems.First();

            orderItem.OrderItemPrice = null;

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var price = orderItem.CatalogueItem.CataloguePrices.First();

            var agreedPrices = price.CataloguePriceTiers
                .Select(x => new PricingTierDto
                {
                    Price = x.Price / 2,
                    LowerRange = x.LowerRange,
                    UpperRange = x.UpperRange,
                })
                .ToList();

            await service.UpsertPrice(order.Id, price, agreedPrices);

            var actual = context.OrderItemPrices
                .FirstOrDefault(x => x.OrderId == order.Id
                    && x.CatalogueItemId == price.CatalogueItemId);

            actual.Should().NotBeNull();
            actual!.OrderId.Should().Be(order.Id);
            actual.CatalogueItemId.Should().Be(price.CatalogueItemId);
            actual.CataloguePriceCalculationType.Should().Be(price.CataloguePriceCalculationType);
            actual.CataloguePriceType.Should().Be(price.CataloguePriceType);
            actual.CurrencyCode.Should().Be(price.CurrencyCode);
            actual.Description.Should().Be(price.PricingUnit.Description);
            actual.BillingPeriod.Should().Be(price.TimeUnit);
            actual.ProvisioningType.Should().Be(price.ProvisioningType);
            actual.RangeDescription.Should().Be(price.PricingUnit.RangeDescription);

            foreach (var tier in actual.OrderItemPriceTiers)
            {
                var pricingTier = price.CataloguePriceTiers.First(x => x.LowerRange == tier.LowerRange && x.UpperRange == tier.UpperRange);
                var agreedPrice = agreedPrices.First(x => x.LowerRange == tier.LowerRange && x.UpperRange == tier.UpperRange);

                tier.ListPrice.Should().Be(pricingTier.Price);
                tier.Price.Should().Be(agreedPrice.Price);
            }
        }

        [Theory]
        [InMemoryDbAutoData]
        public static void UpdatePrice_AgreedPricesIsNull_ThrowsException(
            CatalogueItemId catalogueItemId,
            OrderPriceService service)
        {
            FluentActions
                .Awaiting(() => service.UpdatePrice(0, catalogueItemId, null))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpdatePrice_OrderItemPriceNotInDatabase_NoActionTaken(
            [Frozen] BuyingCatalogueDbContext context,
            Order order,
            OrderPriceService service)
        {
            order.OrderItems.ForEach(x =>
            {
                x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;
                x.OrderItemPrice = null;
            });

            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            var solutionId = order.GetSolution().CatalogueItemId;

            await service.UpdatePrice(order.Id, solutionId, new List<PricingTierDto>());

            var actual = context.OrderItemPrices
                .FirstOrDefault(x => x.OrderId == order.Id
                    && x.CatalogueItemId == solutionId);

            actual.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpdatePrice_OrderItemPriceInDatabase_NoPricingTiers_NoActionTaken(
            [Frozen] BuyingCatalogueDbContext context,
            Order order,
            OrderPriceService service)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            var solutionId = order.GetSolution().CatalogueItemId;

            var expected = context.OrderItemPrices
                .First(x => x.OrderId == order.Id
                    && x.CatalogueItemId == solutionId);

            await service.UpdatePrice(order.Id, solutionId, new List<PricingTierDto>());

            var actual = context.OrderItemPrices
                .First(x => x.OrderId == order.Id
                    && x.CatalogueItemId == solutionId);

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpdatePrice_OrderItemPriceInDatabase_PricingTierSupplied_UpdatesPrices(
            [Frozen] BuyingCatalogueDbContext context,
            Order order,
            List<PricingTierDto> agreedPrices,
            OrderPriceService service)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            var solution = order.GetSolution();

            for (var i = 0; i < agreedPrices.Count; i++)
            {
                var tier = solution.OrderItemPrice.OrderItemPriceTiers.ElementAt(i);

                agreedPrices[i].LowerRange = tier.LowerRange;
                agreedPrices[i].UpperRange = tier.UpperRange;
            }

            await service.UpdatePrice(order.Id, solution.CatalogueItemId, agreedPrices);

            var actual = context.OrderItemPrices
                .First(x => x.OrderId == order.Id
                    && x.CatalogueItemId == solution.CatalogueItemId);

            for (var i = 0; i < actual.OrderItemPriceTiers.Count; i++)
            {
                var tier = actual.OrderItemPriceTiers.ElementAt(i);
                var agreedPrice = agreedPrices.First(x => x.LowerRange == tier.LowerRange && x.UpperRange == tier.UpperRange);

                tier.Price.Should().Be(agreedPrice.Price);
            }
        }
    }
}
