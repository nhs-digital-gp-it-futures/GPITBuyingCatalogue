using System;
using System.Collections.Generic;
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
                order.OrderRecipients.ForEach(r => r.SetQuantityForItem(x.CatalogueItemId, 1));
            });
            context.Orders.Add(order);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            await service.ResetItemQuantities(order.Id, orderItem.CatalogueItemId);
            var dbOrder = await context.Orders
                .Include(x => x.OrderRecipients)
                    .ThenInclude(x => x.OrderItemRecipients)
                .Include(x => x.OrderItems)
                .FirstAsync(x => x.Id == order.Id);

            var actual = dbOrder.OrderItems.FirstOrDefault(x => x.CatalogueItemId == orderItem.CatalogueItemId);

            actual.Should().NotBeNull();
            actual.Quantity.Should().BeNull();
            dbOrder.OrderRecipients.ForEach(r => r.GetQuantityForItem(actual.CatalogueItemId).Should().BeNull());
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
            context.ChangeTracker.Clear();

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
        public static async Task SetServiceRecipientQuantities_OrderItemInDatabase_UpdatesQuantities(
            IFixture fixture,
            [Frozen] BuyingCatalogueDbContext context,
            Order order,
            OrderQuantityService service)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            var solution = order.OrderItems.First();
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            order.OrderRecipients.ForEach(r => r.SetQuantityForItem(solution.CatalogueItemId, 1));
            context.Orders.Add(order);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var quantities = order.OrderRecipients
                .Select(r => new OrderItemRecipientQuantityDto() { OdsCode = r.OdsCode, Quantity = fixture.Create<int>() })
                .ToList();

            await service.SetServiceRecipientQuantities(order.Id, solution.CatalogueItemId, quantities);

            var dbOrder = await context.Orders
                .Include(x => x.OrderRecipients)
                    .ThenInclude(x => x.OrderItemRecipients)
                .Include(x => x.OrderItems)
                .FirstAsync(x => x.Id == order.Id);

            var actual = dbOrder.OrderItems.First(x => x.CatalogueItemId == solution.CatalogueItemId);

            foreach (var i in dbOrder.OrderRecipients)
            {
                var quantity = quantities.First(x => x.OdsCode == i.OdsCode);
                i.GetQuantityForItem(actual.CatalogueItemId).Should().Be(quantity.Quantity);
            }
        }
    }
}
