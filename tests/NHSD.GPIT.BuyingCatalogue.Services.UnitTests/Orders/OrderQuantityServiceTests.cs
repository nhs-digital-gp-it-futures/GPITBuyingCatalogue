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
    public static class OrderQuantityServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderQuantityService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task ResetItemQuantities_OrderItemNotInDatabase_NoActionTaken(
            int orderId,
            CatalogueItemId catalogueItemId,
            [Frozen] BuyingCatalogueDbContext context,
            OrderQuantityService service)
        {
            var expected = context.OrderItems
                .FirstOrDefault(x => x.OrderId == orderId
                    && x.CatalogueItemId == catalogueItemId);

            expected.Should().BeNull();

            await service.ResetItemQuantities(orderId, catalogueItemId);

            var actual = context.OrderItems
                .FirstOrDefault(x => x.OrderId == orderId
                    && x.CatalogueItemId == catalogueItemId);

            actual.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task ResetItemQuantities_OrderItemInDatabase_ExpectedResult(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            OrderQuantityService service)
        {
            var orderItem = order.OrderItems.First();

            order.OrderItems = new List<OrderItem> { orderItem };
            order.OrderItems.ForEach(x =>
            {
                x.Quantity = 1;
                x.OrderItemRecipients.ForEach(r => r.Quantity = 1);
            });

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            var expected = context.OrderItems
                .FirstOrDefault(x => x.OrderId == order.Id
                    && x.CatalogueItemId == orderItem.CatalogueItemId);

            expected.Should().NotBeNull();
            expected!.Quantity.Should().Be(1);
            expected.OrderItemRecipients.ForEach(x => x.Quantity.Should().Be(1));

            await service.ResetItemQuantities(order.Id, orderItem.CatalogueItemId);

            var actual = context.OrderItems
                .FirstOrDefault(x => x.OrderId == order.Id
                    && x.CatalogueItemId == orderItem.CatalogueItemId);

            actual.Should().NotBeNull();
            actual!.Quantity.Should().BeNull();
            actual.OrderItemRecipients.ForEach(x => x.Quantity.Should().BeNull());
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetOrderItemQuantity_OrderItemNotInDatabase_NoActionTaken(
            [Frozen] BuyingCatalogueDbContext context,
            int orderId,
            CatalogueItemId catalogueItemId,
            OrderQuantityService service)
        {
            var expected = context.OrderItems
                .FirstOrDefault(x => x.OrderId == orderId
                    && x.CatalogueItemId == catalogueItemId);

            expected.Should().BeNull();

            await service.SetOrderItemQuantity(orderId, catalogueItemId, 1);

            var actual = context.OrderItems
                .FirstOrDefault(x => x.OrderId == orderId
                    && x.CatalogueItemId == catalogueItemId);

            actual.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetOrderItemQuantity_OrderItemInDatabase_UpdatesQuantity(
            [Frozen] BuyingCatalogueDbContext context,
            Order order,
            int quantity,
            OrderQuantityService service)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderItems.First().Quantity = 1;

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var solutionId = order.OrderItems.First().CatalogueItemId;

            var expected = context.OrderItems
                .First(x => x.OrderId == order.Id
                    && x.CatalogueItemId == solutionId);

            expected.Quantity.Should().Be(1);

            await service.SetOrderItemQuantity(order.Id, solutionId, quantity);

            var actual = context.OrderItems
                .First(x => x.OrderId == order.Id
                    && x.CatalogueItemId == solutionId);

            actual.Quantity.Should().Be(quantity);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static void SetServiceRecipientQuantities_QuantitiesIsNull_ThrowsException(
            int orderId,
            CatalogueItemId catalogueItemId,
            OrderQuantityService service)
        {
            FluentActions
                .Awaiting(() => service.SetServiceRecipientQuantities(orderId, catalogueItemId, null))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetServiceRecipientQuantities_OrderItemNotInDatabase_NoActionTaken(
            [Frozen] BuyingCatalogueDbContext context,
            int orderId,
            CatalogueItemId catalogueItemId,
            List<OrderItemRecipientQuantityDto> quantities,
            OrderQuantityService service)
        {
            var expected = context.OrderItems
                .FirstOrDefault(x => x.OrderId == orderId
                    && x.CatalogueItemId == catalogueItemId);

            expected.Should().BeNull();

            await service.SetServiceRecipientQuantities(orderId, catalogueItemId, quantities);

            var actual = context.OrderItems
                .FirstOrDefault(x => x.OrderId == orderId
                    && x.CatalogueItemId == catalogueItemId);

            actual.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetServiceRecipientQuantities_OrderItemInDatabase_UpdatesQuantities(
            [Frozen] BuyingCatalogueDbContext context,
            Order order,
            List<OrderItemRecipientQuantityDto> quantities,
            OrderQuantityService service)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderItems.First().OrderItemRecipients.ForEach(x => x.Quantity = 1);

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var solutionId = order.OrderItems.First().CatalogueItemId;

            var expected = context.OrderItems
                .First(x => x.OrderId == order.Id
                    && x.CatalogueItemId == solutionId);

            for (var i = 0; i < expected.OrderItemRecipients.Count; i++)
            {
                expected.OrderItemRecipients.ElementAt(i).Quantity.Should().Be(1);
                quantities[i].OdsCode = expected.OrderItemRecipients.ElementAt(i).OdsCode;
            }

            await service.SetServiceRecipientQuantities(order.Id, solutionId, quantities);

            var actual = context.OrderItems
                .First(x => x.OrderId == order.Id
                    && x.CatalogueItemId == solutionId);

            for (var i = 0; i < actual.OrderItemRecipients.Count; i++)
            {
                var recipient = actual.OrderItemRecipients.ElementAt(i);
                var quantity = quantities.First(x => x.OdsCode == recipient.OdsCode);

                recipient.Quantity.Should().Be(quantity.Quantity);
            }
        }
    }
}
