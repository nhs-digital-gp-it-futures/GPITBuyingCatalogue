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
using Moq;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
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
                .ReturnsAsync(new OrderWrapper());

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
                .ReturnsAsync(new OrderWrapper(order));

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
                .ReturnsAsync(new OrderWrapper(order));

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
                .ReturnsAsync(new OrderWrapper());

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
                .ReturnsAsync(new OrderWrapper(order));

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
        public static async Task DetectChangesInFundingAndDelete_NotReadyForReview_OrderItemFundingUnchanged(
            Order order,
            OrderItemFunding funding,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] Mock<IOrderService> mockOrderService,
            OrderItemService orderItemService)
        {
            var item = order.OrderItems.First();
            item.OrderItemFunding = funding;

            context.Orders.Add(order);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(order.CallOffId, order.OrderingParty.InternalIdentifier))
                .ReturnsAsync(new OrderWrapper(order));

            await orderItemService.DetectChangesInFundingAndDelete(order.CallOffId, order.OrderingParty.InternalIdentifier, item.CatalogueItemId);
            var actual = context.OrderItems
                .Include(i => i.OrderItemFunding)
                .FirstOrDefault(o => o.OrderId == item.OrderId && o.CatalogueItemId == item.CatalogueItemId);

            actual!.OrderItemFunding.OrderItemFundingType.Should().Be(funding.OrderItemFundingType);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task DetectChangesInFundingAndDelete_LocalFundingOnly_FrameworkLocalFundingOnly_OrderItemFundingUnchanged(
            Order order,
            OrderItemFunding funding,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] Mock<IOrderService> mockOrderService,
            OrderItemService orderItemService)
        {
            var item = order.OrderItems.First();
            funding.OrderItemFundingType = OrderItemFundingType.LocalFundingOnly;
            item.OrderItemFunding = funding;

            order.OrderingParty.OrganisationType = OrganisationType.IB;
            order.SelectedFramework.LocalFundingOnly = true;

            context.Orders.Add(order);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(order.CallOffId, order.OrderingParty.InternalIdentifier))
                .ReturnsAsync(new OrderWrapper(order));

            await orderItemService.DetectChangesInFundingAndDelete(order.CallOffId, order.OrderingParty.InternalIdentifier, item.CatalogueItemId);

            var actual = context.OrderItems
                .Include(i => i.OrderItemFunding)
                .FirstOrDefault(o => o.OrderId == item.OrderId && o.CatalogueItemId == item.CatalogueItemId);

            actual!.OrderItemFunding.OrderItemFundingType.Should().Be(OrderItemFundingType.LocalFundingOnly);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task DetectChangesInFundingAndDelete_LocalFundingOnly_GPPRactice_OrderItemFundingUnchanged(
            Order order,
            OrderItemFunding funding,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] Mock<IOrderService> mockOrderService,
            OrderItemService orderItemService)
        {
            var item = order.OrderItems.First();
            funding.OrderItemFundingType = OrderItemFundingType.LocalFundingOnly;
            item.OrderItemFunding = funding;

            order.OrderingParty.OrganisationType = OrganisationType.GP;
            order.SelectedFramework.LocalFundingOnly = false;

            context.Orders.Add(order);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(order.CallOffId, order.OrderingParty.InternalIdentifier))
                .ReturnsAsync(new OrderWrapper(order));

            await orderItemService.DetectChangesInFundingAndDelete(order.CallOffId, order.OrderingParty.InternalIdentifier, item.CatalogueItemId);

            var actual = context.OrderItems
                .Include(i => i.OrderItemFunding)
                .FirstOrDefault(o => o.OrderId == item.OrderId && o.CatalogueItemId == item.CatalogueItemId);

            actual!.OrderItemFunding.OrderItemFundingType.Should().Be(OrderItemFundingType.LocalFundingOnly);
        }

        [Theory]
        [InMemoryDbInlineAutoData(OrderItemFundingType.CentralFunding)]
        [InMemoryDbInlineAutoData(OrderItemFundingType.LocalFunding)]
        [InMemoryDbInlineAutoData(OrderItemFundingType.MixedFunding)]
        [InMemoryDbInlineAutoData(OrderItemFundingType.NoFundingRequired)]
        [InMemoryDbInlineAutoData(OrderItemFundingType.None)]
        public static async Task DetectChangesInFundingAndDelete_FundingTypeChanged_FrameworkLocalFundingOnly_OrderItemFundingNull(
            OrderItemFundingType fundingType,
            Order order,
            OrderItemFunding funding,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] Mock<IOrderService> mockOrderService,
            OrderItemService orderItemService)
        {
            var item = order.OrderItems.First();
            funding.OrderItemFundingType = fundingType;
            item.OrderItemFunding = funding;
            order.OrderingParty.OrganisationType = OrganisationType.IB;
            order.SelectedFramework.LocalFundingOnly = true;
            context.Orders.Add(order);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(order.CallOffId, order.OrderingParty.InternalIdentifier))
                .ReturnsAsync(new OrderWrapper(order));

            await orderItemService.DetectChangesInFundingAndDelete(order.CallOffId, order.OrderingParty.InternalIdentifier, item.CatalogueItemId);

            var actual = context.OrderItems.FirstOrDefault(o => o.OrderId == item.OrderId && o.CatalogueItemId == item.CatalogueItemId);

            actual!.OrderItemFunding.Should().BeNull();
        }

        [Theory]
        [InMemoryDbInlineAutoData(OrderItemFundingType.CentralFunding)]
        [InMemoryDbInlineAutoData(OrderItemFundingType.LocalFunding)]
        [InMemoryDbInlineAutoData(OrderItemFundingType.MixedFunding)]
        [InMemoryDbInlineAutoData(OrderItemFundingType.NoFundingRequired)]
        [InMemoryDbInlineAutoData(OrderItemFundingType.None)]
        public static async Task DetectChangesInFundingAndDelete_FundingTypeChanged_GPPRactice_OrderItemFundingNull(
            OrderItemFundingType fundingType,
            Order order,
            OrderItemFunding funding,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] Mock<IOrderService> mockOrderService,
            OrderItemService orderItemService)
        {
            var item = order.OrderItems.First();
            funding.OrderItemFundingType = fundingType;
            item.OrderItemFunding = funding;

            order.OrderingParty.OrganisationType = OrganisationType.GP;
            order.SelectedFramework.LocalFundingOnly = false;

            context.Orders.Add(order);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            mockOrderService
                .Setup(x => x.GetOrderWithOrderItems(order.CallOffId, order.OrderingParty.InternalIdentifier))
                .ReturnsAsync(new OrderWrapper(order));

            await orderItemService.DetectChangesInFundingAndDelete(order.CallOffId, order.OrderingParty.InternalIdentifier, item.CatalogueItemId);

            var actual = context.OrderItems.FirstOrDefault(o => o.OrderId == item.OrderId && o.CatalogueItemId == item.CatalogueItemId);

            actual!.OrderItemFunding.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetOrderItemEstimationPeriod_EstimationPeriodSetCorrectly_Patient(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            OrderItemService orderItemService)
        {
            var item = order.OrderItems.First();

            var price = item.CatalogueItem.CataloguePrices.First();

            price.ProvisioningType = ProvisioningType.Patient;

            item.EstimationPeriod = null;

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            await orderItemService.SetOrderItemEstimationPeriod(order.CallOffId, order.OrderingParty.InternalIdentifier, item.CatalogueItemId, price);

            var actual = context.OrderItems.FirstOrDefault(o => o.OrderId == item.OrderId && o.CatalogueItemId == item.CatalogueItemId);

            actual!.EstimationPeriod.Should().Be(TimeUnit.PerMonth);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetOrderItemEstimationPeriod_EstimationPeriodSetCorrectly_Declarative(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            OrderItemService orderItemService)
        {
            var item = order.OrderItems.First();

            var price = item.CatalogueItem.CataloguePrices.First();

            price.ProvisioningType = ProvisioningType.Declarative;

            item.EstimationPeriod = null;

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            await orderItemService.SetOrderItemEstimationPeriod(order.CallOffId, order.OrderingParty.InternalIdentifier, item.CatalogueItemId, price);

            var actual = context.OrderItems.FirstOrDefault(o => o.OrderId == item.OrderId && o.CatalogueItemId == item.CatalogueItemId);

            actual!.EstimationPeriod.Should().Be(TimeUnit.PerYear);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetOrderItemEstimationPeriod_EstimationPeriodSetCorrectly_PerServiceRecipient(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            OrderItemService orderItemService)
        {
            var item = order.OrderItems.First();

            var price = item.CatalogueItem.CataloguePrices.First();

            price.ProvisioningType = ProvisioningType.PerServiceRecipient;

            item.EstimationPeriod = null;

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            await orderItemService.SetOrderItemEstimationPeriod(order.CallOffId, order.OrderingParty.InternalIdentifier, item.CatalogueItemId, price);

            var actual = context.OrderItems.FirstOrDefault(o => o.OrderId == item.OrderId && o.CatalogueItemId == item.CatalogueItemId);

            actual!.EstimationPeriod.Should().Be(TimeUnit.PerYear);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetOrderItemEstimationPeriod_EstimationPeriodSetCorrectly_OnDemand(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            OrderItemService orderItemService)
        {
            var item = order.OrderItems.First();

            var price = item.CatalogueItem.CataloguePrices.First();

            price.ProvisioningType = ProvisioningType.OnDemand;

            item.EstimationPeriod = null;

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            await orderItemService.SetOrderItemEstimationPeriod(order.CallOffId, order.OrderingParty.InternalIdentifier, item.CatalogueItemId, price);

            var actual = context.OrderItems.FirstOrDefault(o => o.OrderId == item.OrderId && o.CatalogueItemId == item.CatalogueItemId);

            actual!.EstimationPeriod.Should().Be(price.BillingPeriod);
        }
    }
}
