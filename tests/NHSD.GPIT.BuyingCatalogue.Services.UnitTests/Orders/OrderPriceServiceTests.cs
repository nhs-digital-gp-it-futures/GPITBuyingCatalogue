using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using MoreLinq;
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

            await service.UpdatePrice(order.Id, solutionId, new List<OrderPricingTierDto>());

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
                .Single(x => x.OrderId == order.Id
                    && x.CatalogueItemId == solutionId);

            await service.UpdatePrice(order.Id, solutionId, new List<OrderPricingTierDto>());

            var actual = context.OrderItemPrices
                .Single(x => x.OrderId == order.Id
                    && x.CatalogueItemId == solutionId);

            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpdatePrice_OrderItemPriceInDatabase_PricingTierSupplied_UpdatesPrices(
            [Frozen] BuyingCatalogueDbContext context,
            Order order,
            List<OrderPricingTierDto> agreedPrices,
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
                .Single(x => x.OrderId == order.Id
                    && x.CatalogueItemId == solution.CatalogueItemId);

            for (var i = 0; i < actual.OrderItemPriceTiers.Count; i++)
            {
                var tier = actual.OrderItemPriceTiers.ElementAt(i);
                var agreedPrice = agreedPrices.Single(x => x.LowerRange == tier.LowerRange && x.UpperRange == tier.UpperRange);

                tier.Price.Should().Be(agreedPrice.Price);
            }
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetQuantity_OrderItemNotInDatabase_NoActionTaken(
            [Frozen] BuyingCatalogueDbContext context,
            int orderId,
            CatalogueItemId catalogueItemId,
            OrderPriceService service)
        {
            var expected = context.OrderItems
                .FirstOrDefault(x => x.OrderId == orderId
                    && x.CatalogueItemId == catalogueItemId);

            expected.Should().BeNull();

            await service.SetQuantity(orderId, catalogueItemId, 1);

            var actual = context.OrderItems
                .FirstOrDefault(x => x.OrderId == orderId
                    && x.CatalogueItemId == catalogueItemId);

            actual.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetQuantity_OrderItemInDatabase_UpdatesQuantity(
            [Frozen] BuyingCatalogueDbContext context,
            Order order,
            int quantity,
            OrderPriceService service)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderItems.First().Quantity = 1;

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var solutionId = order.OrderItems.First().CatalogueItemId;

            var expected = context.OrderItems
                .Single(x => x.OrderId == order.Id
                    && x.CatalogueItemId == solutionId);

            expected.Quantity.Should().Be(1);

            await service.SetQuantity(order.Id, solutionId, quantity);

            var actual = context.OrderItems
                .Single(x => x.OrderId == order.Id
                    && x.CatalogueItemId == solutionId);

            actual.Quantity.Should().Be(quantity);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static void SetQuantities_QuantitiesIsNull_ThrowsException(
            int orderId,
            CatalogueItemId catalogueItemId,
            OrderPriceService service)
        {
            FluentActions
                .Awaiting(() => service.SetQuantities(orderId, catalogueItemId, null))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetQuantities_OrderItemNotInDatabase_NoActionTaken(
            [Frozen] BuyingCatalogueDbContext context,
            int orderId,
            CatalogueItemId catalogueItemId,
            List<OrderPricingTierQuantityDto> quantities,
            OrderPriceService service)
        {
            var expected = context.OrderItems
                .FirstOrDefault(x => x.OrderId == orderId
                    && x.CatalogueItemId == catalogueItemId);

            expected.Should().BeNull();

            await service.SetQuantities(orderId, catalogueItemId, quantities);

            var actual = context.OrderItems
                .FirstOrDefault(x => x.OrderId == orderId
                    && x.CatalogueItemId == catalogueItemId);

            actual.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetQuantities_OrderItemInDatabase_UpdatesQuantities(
            [Frozen] BuyingCatalogueDbContext context,
            Order order,
            List<OrderPricingTierQuantityDto> quantities,
            OrderPriceService service)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderItems.First().OrderItemRecipients.ForEach(x => x.Quantity = 1);

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var solutionId = order.OrderItems.First().CatalogueItemId;

            var expected = context.OrderItems
                .Single(x => x.OrderId == order.Id
                    && x.CatalogueItemId == solutionId);

            for (var i = 0; i < expected.OrderItemRecipients.Count; i++)
            {
                expected.OrderItemRecipients.ElementAt(i).Quantity.Should().Be(1);
                quantities[i].OdsCode = expected.OrderItemRecipients.ElementAt(i).OdsCode;
            }

            await service.SetQuantities(order.Id, solutionId, quantities);

            var actual = context.OrderItems
                .Single(x => x.OrderId == order.Id
                    && x.CatalogueItemId == solutionId);

            for (var i = 0; i < actual.OrderItemRecipients.Count; i++)
            {
                var recipient = actual.OrderItemRecipients.ElementAt(i);
                var quantity = quantities.Single(x => x.OdsCode == recipient.OdsCode);

                recipient.Quantity.Should().Be(quantity.Quantity);
            }
        }
    }
}
