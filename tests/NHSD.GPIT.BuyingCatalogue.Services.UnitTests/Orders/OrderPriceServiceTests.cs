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
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
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
        public static void AddPrice_RecipientIsNull_ThrowsException(OrderPriceService service)
        {
            FluentActions
                .Awaiting(() => service.AddPrice(0, null, new List<OrderPricingTierDto>()))
                .Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("price");
        }

        [Theory]
        [InMemoryDbAutoData]
        public static void AddPrice_AgreedPricesIsNull_ThrowsException(CataloguePrice price, OrderPriceService service)
        {
            FluentActions
                .Awaiting(() => service.AddPrice(0, price, null))
                .Should().ThrowAsync<ArgumentNullException>()
                .WithParameterName("agreedPrices");
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddPrice_OrderItemNotInDatabase_NoActionTaken(
            [Frozen] BuyingCatalogueDbContext context,
            Order order,
            CataloguePrice price,
            OrderPriceService service)
        {
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            await service.AddPrice(order.Id, price, new List<OrderPricingTierDto>());

            var actual = context.OrderItemPrices
                .FirstOrDefault(x => x.OrderId == order.Id
                    && x.CatalogueItemId == price.CatalogueItemId);

            actual.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddPrice_OrderItemInDatabase_AddsPrice(
            [Frozen] BuyingCatalogueDbContext context,
            Order order,
            OrderPriceService service)
        {
            var orderItem = order.OrderItems.First();

            orderItem.OrderItemPrice = null;

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var price = orderItem.CatalogueItem.CataloguePrices.First();

            await service.AddPrice(order.Id, price, new List<OrderPricingTierDto>());

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
            actual.EstimationPeriod.Should().Be(price.TimeUnit);
            actual.ProvisioningType.Should().Be(price.ProvisioningType);
            actual.RangeDescription.Should().Be(price.PricingUnit.RangeDescription);

            foreach (var tier in actual.OrderItemPriceTiers)
            {
                var pricingTier = price.CataloguePriceTiers.Single(x => x.LowerRange == tier.LowerRange && x.UpperRange == tier.UpperRange);

                tier.ListPrice.Should().Be(pricingTier.Price);
                tier.Price.Should().Be(pricingTier.Price);
            }
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddPrice_WithAgreedPrices_OrderItemInDatabase_AddsPrice(
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
                .Select(x => new OrderPricingTierDto
                {
                    Price = x.Price / 2,
                    LowerRange = x.LowerRange,
                    UpperRange = x.UpperRange,
                })
                .ToList();

            await service.AddPrice(order.Id, price, agreedPrices);

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
            actual.EstimationPeriod.Should().Be(price.TimeUnit);
            actual.ProvisioningType.Should().Be(price.ProvisioningType);
            actual.RangeDescription.Should().Be(price.PricingUnit.RangeDescription);

            foreach (var tier in actual.OrderItemPriceTiers)
            {
                var pricingTier = price.CataloguePriceTiers.Single(x => x.LowerRange == tier.LowerRange && x.UpperRange == tier.UpperRange);
                var agreedPrice = agreedPrices.Single(x => x.LowerRange == tier.LowerRange && x.UpperRange == tier.UpperRange);

                tier.ListPrice.Should().Be(pricingTier.Price);
                tier.Price.Should().Be(agreedPrice.Price);
            }
        }
    }
}
