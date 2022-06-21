using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using LinqKit;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Orders
{
    public static class OrderItemServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderItemService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static void AddOrderItems_ItemIdsAreNull_ThrowsException(
            string internalOrgId,
            CallOffId callOffId,
            OrderItemService service)
        {
            FluentActions
                .Awaiting(() => service.AddOrderItems(internalOrgId, callOffId, null))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddOrderItems_NoOrder_NoActionTaken(
            string internalOrgId,
            CallOffId callOffId,
            List<CatalogueItemId> itemIds,
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] Mock<IOrderService> mockOrderService,
            OrderItemService service)
        {
            itemIds.ForEach(x => context.CatalogueItems.Add(new CatalogueItem { Id = x, Name = $"{x}" }));
            await context.SaveChangesAsync();

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync((Order)null);

            await service.AddOrderItems(internalOrgId, callOffId, itemIds);

            mockOrderService.VerifyAll();

            itemIds.ForEach(x =>
            {
                var actual = context.OrderItems.FirstOrDefault(o => o.OrderId == order.Id && o.CatalogueItemId == x);

                actual.Should().BeNull();
            });
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddOrderItems_AddsOrderItemsToDatabase(
            string internalOrgId,
            CallOffId callOffId,
            List<CatalogueItemId> itemIds,
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] Mock<IOrderService> mockOrderService,
            OrderItemService service)
        {
            itemIds.ForEach(x => context.CatalogueItems.Add(new CatalogueItem { Id = x, Name = $"{x}" }));
            await context.SaveChangesAsync();

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(order);

            await service.AddOrderItems(internalOrgId, callOffId, itemIds);

            mockOrderService.VerifyAll();

            itemIds.ForEach(x =>
            {
                var actual = context.OrderItems.FirstOrDefault(o => o.OrderId == order.Id && o.CatalogueItemId == x);

                actual.Should().NotBeNull();
            });
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddOrderItems_WithExistingItems_AddsOrderItemsToDatabase(
            string internalOrgId,
            CallOffId callOffId,
            List<CatalogueItemId> itemIds,
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] Mock<IOrderService> mockOrderService,
            OrderItemService service)
        {
            itemIds.ForEach(x => context.CatalogueItems.Add(new CatalogueItem { Id = x, Name = $"{x}" }));

            await context.SaveChangesAsync();

            order.OrderItems.First().CatalogueItem.Id = itemIds.First();

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(order);

            await service.AddOrderItems(internalOrgId, callOffId, itemIds);

            mockOrderService.VerifyAll();

            itemIds.ForEach(x =>
            {
                var actual = context.OrderItems.FirstOrDefault(o => o.OrderId == order.Id && o.CatalogueItem.Id == x);

                if (x == itemIds.First())
                {
                    actual.Should().BeNull();
                }
                else
                {
                    actual.Should().NotBeNull();
                }
            });
        }

        [Theory]
        [InMemoryDbAutoData]
        public static void DeleteOrderItems_ItemIdsAreNull_ThrowsException(
            string internalOrgId,
            CallOffId callOffId,
            OrderItemService service)
        {
            FluentActions
                .Awaiting(() => service.DeleteOrderItems(internalOrgId, callOffId, null))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task DeleteOrderItems_NoOrder_NoActionTaken(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] Mock<IOrderService> mockOrderService,
            OrderItemService service)
        {
            context.Orders.Add(order);

            await context.SaveChangesAsync();

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync((Order)null);

            var itemIds = order.OrderItems.Select(x => x.CatalogueItemId).ToList();

            await service.DeleteOrderItems(internalOrgId, callOffId, itemIds);

            mockOrderService.VerifyAll();

            itemIds.ForEach(x =>
            {
                var actual = context.OrderItems.FirstOrDefault(o => o.OrderId == order.Id && o.CatalogueItemId == x);

                actual.Should().NotBeNull();
            });
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task DeleteOrderItems_WithOrder_ItemIdsMissing_NoActionTaken(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            List<CatalogueItemId> itemIds,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] Mock<IOrderService> mockOrderService,
            OrderItemService service)
        {
            context.Orders.Add(order);

            await context.SaveChangesAsync();

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(callOffId, internalOrgId))
                .ReturnsAsync(order);

            await service.DeleteOrderItems(internalOrgId, callOffId, itemIds);

            mockOrderService.VerifyAll();

            order.OrderItems.Select(x => x.CatalogueItemId).ForEach(x =>
            {
                var actual = context.OrderItems.FirstOrDefault(o => o.OrderId == order.Id && o.CatalogueItemId == x);

                actual.Should().NotBeNull();
            });
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetOrderItemFunding_NoFunding_ShouldHaveForcedFunding_SetsForcedFunding(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] Mock<IOrderItemFundingService> mockOrderItemFundingService,
            OrderItemService orderItemService)
        {
            const OrderItemFundingType expected = OrderItemFundingType.NoFundingRequired;

            var item = order.OrderItems.First();

            item.OrderItemFunding = null;
            item.OrderItemPrice.OrderItemPriceTiers.ForEach(x => x.Price = 0);

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            mockOrderItemFundingService
                .Setup(x => x.GetFundingType(item))
                .ReturnsAsync(expected);

            await orderItemService.SetOrderItemFunding(order.CallOffId, order.OrderingParty.InternalIdentifier, item.CatalogueItemId);

            mockOrderItemFundingService.VerifyAll();

            var actual = context.OrderItems.FirstOrDefault(o => o.OrderId == item.OrderId && o.CatalogueItemId == item.CatalogueItemId);

            actual?.OrderItemFunding.Should().NotBeNull();
            actual?.OrderItemFunding.OrderItemFundingType.Should().Be(expected);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetOrderItemFunding_ForcedFunding_ShouldNotHaveForcedFunding_DeletesFunding(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] Mock<IOrderItemFundingService> mockOrderItemFundingService,
            OrderItemService orderItemService)
        {
            var item = order.OrderItems.First();

            item.OrderItemFunding.OrderItemFundingType = OrderItemFundingType.NoFundingRequired;

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            mockOrderItemFundingService
                .Setup(x => x.GetFundingType(item))
                .ReturnsAsync(OrderItemFundingType.None);

            await orderItemService.SetOrderItemFunding(order.CallOffId, order.OrderingParty.InternalIdentifier, item.CatalogueItemId);

            mockOrderItemFundingService.VerifyAll();

            var actual = context.OrderItems.FirstOrDefault(o => o.OrderId == item.OrderId && o.CatalogueItemId == item.CatalogueItemId);

            actual?.OrderItemFunding.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetOrderItemFunding_NoFunding_ShouldNotHaveForcedFunding_NoFundingSet(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] Mock<IOrderItemFundingService> mockOrderItemFundingService,
            OrderItemService orderItemService)
        {
            var item = order.OrderItems.First();

            item.OrderItemFunding = null;

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            mockOrderItemFundingService
                .Setup(x => x.GetFundingType(item))
                .ReturnsAsync(OrderItemFundingType.None);

            await orderItemService.SetOrderItemFunding(order.CallOffId, order.OrderingParty.InternalIdentifier, item.CatalogueItemId);

            mockOrderItemFundingService.VerifyAll();

            var actual = context.OrderItems.FirstOrDefault(o => o.OrderId == item.OrderId && o.CatalogueItemId == item.CatalogueItemId);

            actual?.OrderItemFunding.Should().BeNull();
        }
    }
}
