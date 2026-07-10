using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
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
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Orders
{
    public static class OrderQuantityServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderQuantityService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task ResetItemQuantities_OrderItemInDatabase_ExpectedResult(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            OrderQuantityService service)
        {
            var orderItem = order.OrderItems.First();

            order.OrderItems = new List<OrderItem> { orderItem };
            order.OrderItems.ForEach(x =>
            {
                order.FlattenedRecipients.ForEach(r => r.SetQuantityForItem(x, 1));
            });
            context.Orders.Add(order);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            await service.ResetItemQuantities(orderItem.Id);
            Order dbOrder = await context.Orders
                .Include(x => x.OrderSublocations)
                .ThenInclude(y => y.SublocationRecipients)
                .ThenInclude(z => z.OrderItemSublocationRecipients)
                .Include(x => x.OrderItems)
                .FirstAsync(x => x.Id == order.Id);

            var actual = dbOrder.OrderItems.FirstOrDefault(x => x.Id == orderItem.Id);

            actual.Should().NotBeNull();
            dbOrder.FlattenedRecipients.ForEach(r => r.GetQuantityForItem(actual.Id).Should().BeNull());
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static void SetServiceRecipientQuantities_QuantitiesIsNull_ThrowsException(
            int orderId,
            int orderItemId,
            OrderQuantityService service)
        {
            FluentActions
                .Awaiting(() => service.SetServiceRecipientQuantities(orderId, orderItemId, null))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task SetServiceRecipientQuantities_OrderItemInDatabase_UpdatesQuantities(
            IFixture fixture,
            [Frozen] BuyingCatalogueDbContext context,
            Order order,
            OrderSublocation sublocation,
            OrderSublocationRecipient recipient,
            OrderQuantityService service)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            var solution = order.OrderItems.First();
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            sublocation.SublocationRecipients = new List<OrderSublocationRecipient> { recipient };
            order.OrderSublocations = new List<OrderSublocation> { sublocation };
            order.FlattenedRecipients.ForEach(r =>
            {
                r.OrderItemSublocationRecipients.Add(
                    new OrderItemSublocationRecipient
                    {
                        OrderId = order.Id,
                        RecipientOdsCode = r.RecipientOdsCode,
                        OrderItem = solution,
                        Quantity = 1,
                    });
            });

            context.Orders.Add(order);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            List<OrderItemRecipientQuantityDto> quantities = order.FlattenedRecipients
                .Select(r =>
                    new OrderItemRecipientQuantityDto
                    {
                        ParentSublocationOdsCode = r.ParentSublocationOdsCode,
                        RecipientOdsCode = r.RecipientOdsCode,
                        Quantity = fixture.Create<int>(),
                    })
                .ToList();

            await service.SetServiceRecipientQuantities(order.Id, solution.Id, quantities);

            Order dbOrder = await context.Orders
                .Include(x => x.OrderSublocations)
                .ThenInclude(y => y.SublocationRecipients)
                .ThenInclude(z => z.OrderItemSublocationRecipients)
                .Include(x => x.OrderItems)
                .FirstAsync(x => x.Id == order.Id);

            var actual = dbOrder.OrderItems.First(x => x.Id == solution.Id);

            foreach (OrderSublocationRecipient i in dbOrder.FlattenedRecipients)
            {
                OrderItemRecipientQuantityDto quantity = quantities.First(x =>
                    x.RecipientOdsCode == i.RecipientOdsCode
                    && x.ParentSublocationOdsCode == i.ParentSublocationOdsCode);
                i.GetQuantityForItem(actual.Id).Should().Be(quantity.Quantity);
                i.GetQuantityForItem(actual.CatalogueItemId).Should().Be(quantity.Quantity);
            }
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task SetServiceRecipientQuantitiesToSameValue_OrderItemInDatabase_UpdatesQuantities(
            int quantity,
            [Frozen] BuyingCatalogueDbContext context,
            Order order,
            OrderSublocation sublocation,
            OrderSublocationRecipient recipient,
            OrderQuantityService service)
        {
            order.OrderItems.ForEach(x => x.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService);
            var solution = order.OrderItems.First();
            solution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            sublocation.SublocationRecipients = new List<OrderSublocationRecipient> { recipient };
            order.OrderSublocations = new List<OrderSublocation> { sublocation };
            order.FlattenedRecipients.ForEach(r =>
            {
                r.OrderItemSublocationRecipients.Add(
                    new OrderItemSublocationRecipient
                    {
                        OrderId = order.Id,
                        RecipientOdsCode = r.RecipientOdsCode,
                        OrderItem = solution,
                    });
            });
            context.Orders.Add(order);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            await service.SetServiceRecipientQuantities(order.Id, solution.CatalogueItemId, quantity);

            Order dbOrder = await context.Orders
                .Include(x => x.OrderSublocations)
                .ThenInclude(y => y.SublocationRecipients)
                .ThenInclude(z => z.OrderItemSublocationRecipients)
                .Include(x => x.OrderItems)
                .FirstAsync(x => x.Id == order.Id);

            dbOrder.FlattenedRecipients.SelectMany(or => or.OrderItemSublocationRecipients)
                .Where(oir => oir.OrderItem.CatalogueItemId == solution.CatalogueItemId)
                .All(oir => oir.Quantity == quantity)
                .Should()
                .BeTrue();
        }
    }
}
