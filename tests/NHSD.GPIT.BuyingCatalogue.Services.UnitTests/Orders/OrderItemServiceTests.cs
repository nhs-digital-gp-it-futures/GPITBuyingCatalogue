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
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Orders
{
    public static class OrderItemServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderItemService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockInMemoryDbAutoData]
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
        [MockInMemoryDbAutoData]
        public static async Task AddOrderItems_NoOrder_NoActionTaken(
            string internalOrgId,
            CallOffId callOffId,
            List<CatalogueItemId> itemIds,
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] IOrderService mockOrderService,
            OrderItemService service)
        {
            itemIds.ForEach(x => context.CatalogueItems.Add(new CatalogueItem { Id = x, Name = $"{x}" }));
            await context.SaveChangesAsync();

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper());

            await service.AddOrderItems(internalOrgId, callOffId, itemIds);

            await mockOrderService.Received().GetOrderWithOrderItems(callOffId, internalOrgId);

            itemIds.ForEach(x =>
            {
                var actual = context.OrderItems.FirstOrDefault(o => o.OrderId == order.Id && o.CatalogueItemId == x);

                actual.Should().BeNull();
            });
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task AddOrderItems_AddsOrderItemsToDatabase(
            string internalOrgId,
            CallOffId callOffId,
            List<CatalogueItemId> itemIds,
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] IOrderService mockOrderService,
            OrderItemService service)
        {
            itemIds.ForEach(x => context.CatalogueItems.Add(new CatalogueItem { Id = x, Name = $"{x}", CatalogueItemType = CatalogueItemType.AdditionalService }));
            await context.SaveChangesAsync();

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            await service.AddOrderItems(internalOrgId, callOffId, itemIds);

            await mockOrderService.Received().GetOrderWithOrderItems(callOffId, internalOrgId);

            itemIds.ForEach(x =>
            {
                var actual = context.OrderItems.FirstOrDefault(o => o.OrderId == order.Id && o.CatalogueItemId == x);

                actual.Should().NotBeNull();
            });
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task AddOrderItems_WithParentId_AddsParent(
            string internalOrgId,
            CallOffId callOffId,
            List<CatalogueItemId> itemIds,
            OrderItem parentOrderItem,
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] IOrderService mockOrderService,
            OrderItemService service)
        {
            itemIds.ForEach(x => context.CatalogueItems.Add(new CatalogueItem { Id = x, Name = $"{x}", CatalogueItemType = CatalogueItemType.AssociatedService }));
            await context.SaveChangesAsync();

            order.OrderItems.First().CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            parentOrderItem.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;
            order.OrderItems.Add(parentOrderItem);
            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            await service.AddOrderItems(internalOrgId, callOffId, itemIds, parentOrderItem.Id);

            await mockOrderService.Received().GetOrderWithOrderItems(callOffId, internalOrgId);

            itemIds.ForEach(x =>
            {
                var actual = context.OrderItems.FirstOrDefault(o => o.OrderId == order.Id && o.CatalogueItemId == x);

                actual.Should().NotBeNull();
                actual!.ParentId.Should().Be(parentOrderItem.Id);
            });
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task AddOrderItems_WithSolutionItemParent(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            List<CatalogueItemId> itemIds,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] IOrderService mockOrderService,
            OrderItemService service)
        {
            var solutionId = itemIds.First();
            itemIds
                .ForEach(x => context.CatalogueItems.Add(
                    new CatalogueItem
                    {
                        Id = x,
                        Name = $"{x}",
                        CatalogueItemType = x == solutionId
                            ? CatalogueItemType.Solution
                            : CatalogueItemType.AdditionalService,
                    }));
            await context.SaveChangesAsync();

            order.OrderItems.Clear();
            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            await service.AddOrderItems(internalOrgId, callOffId, itemIds);

            await mockOrderService.Received().GetOrderWithOrderItems(callOffId, internalOrgId);

            var solution = context.OrderItems.FirstOrDefault(o => o.CatalogueItemId == solutionId);
            solution.Should().NotBeNull();
            solution!.ParentId.Should().BeNull();

            itemIds
                .Where(id => id != solutionId)
                .ForEach(x =>
            {
                var actual = context.OrderItems
                    .Include(item => item.Parent)
                    .FirstOrDefault(o => o.OrderId == order.Id && o.CatalogueItemId == x);

                actual.Should().NotBeNull();
                actual!.Parent!.CatalogueItemId.Should().Be(solutionId);
                actual.ParentId.Should().Be(solution.Id);
            });
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task AddOrderItems_WithExistingItems_AddsOrderItemsToDatabase(
            string internalOrgId,
            CallOffId callOffId,
            List<CatalogueItemId> itemIds,
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] IOrderService mockOrderService,
            OrderItemService service)
        {
            var solutionId = itemIds.First();

            itemIds.ForEach(x => context.CatalogueItems.Add(new CatalogueItem
            {
                Id = x,
                Name = $"{x}",
                CatalogueItemType = x == solutionId
                    ? CatalogueItemType.Solution
                    : CatalogueItemType.AdditionalService,
            }));

            await context.SaveChangesAsync();

            var existingSolution = order.OrderItems.First();
            existingSolution.CatalogueItemId = solutionId;
            existingSolution.CatalogueItem.Id = solutionId;
            existingSolution.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            await service.AddOrderItems(internalOrgId, callOffId, itemIds);

            await mockOrderService.Received().GetOrderWithOrderItems(callOffId, internalOrgId);

            itemIds.ForEach(x =>
            {
                var actual = context.OrderItems.FirstOrDefault(o => o.OrderId == order.Id && o.CatalogueItem.Id == x);

                if (x == solutionId)
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
        [MockInMemoryDbAutoData]
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
        [MockInMemoryDbAutoData]
        public static async Task DeleteOrderItems_NoOrder_NoActionTaken(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] IOrderService mockOrderService,
            OrderItemService service)
        {
            context.Orders.Add(order);

            await context.SaveChangesAsync();

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper());

            var itemIds = order.OrderItems.Select(x => x.Id).ToList();

            await service.DeleteOrderItems(internalOrgId, callOffId, itemIds);

            await mockOrderService.Received().GetOrderWithOrderItems(callOffId, internalOrgId);

            itemIds.ForEach(x =>
            {
                var actual = context.OrderItems.FirstOrDefault(o => o.Id == x);

                actual.Should().NotBeNull();
            });
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task DeleteOrderItems_WithOrder_ItemIdsMissing_NoActionTaken(
            string internalOrgId,
            CallOffId callOffId,
            Order order,
            List<int> itemIds,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] IOrderService mockOrderService,
            OrderItemService service)
        {
            context.Orders.Add(order);

            await context.SaveChangesAsync();

            mockOrderService.GetOrderWithOrderItems(callOffId, internalOrgId).Returns(new OrderWrapper(order));

            await service.DeleteOrderItems(internalOrgId, callOffId, itemIds);

            await mockOrderService.Received().GetOrderWithOrderItems(callOffId, internalOrgId);

            order.OrderItems.Select(x => x.Id).ForEach(x =>
            {
                var actual = context.OrderItems.FirstOrDefault(o => o.Id == x);

                actual.Should().NotBeNull();
            });
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetOrderItem_NonExistentOrderItem_ReturnsNull(
            string internalOrgId,
            Order order,
            OrderItem orderItem,
            [Frozen] BuyingCatalogueDbContext context,
            OrderItemService service)
        {
            orderItem.Id = 5;
            order.OrderItems.Add(orderItem);
            order.OrderingParty.InternalIdentifier = internalOrgId;

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            var result = await service.GetOrderItem(order.CallOffId, internalOrgId, 10);

            result.Should().BeNull();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetOrderItem_ExistentOrderItem_ReturnsOrderItem(
            string internalOrgId,
            int id,
            Order order,
            OrderItem orderItem,
            OrderItem parentOrderItem,
            [Frozen] BuyingCatalogueDbContext context,
            OrderItemService service)
        {
            orderItem.Id = id;
            orderItem.Parent = parentOrderItem;
            order.OrderItems.Add(orderItem);
            order.OrderingParty.InternalIdentifier = internalOrgId;

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            var result = await service.GetOrderItem(order.CallOffId, internalOrgId, id);

            result.Should().NotBeNull();
            result.Id.Should().Be(id);
            result.ParentId.Should().Be(parentOrderItem.Id);
            result.Parent.Id.Should().Be(parentOrderItem.Id);
            result.Parent.CatalogueItem.Should().NotBeNull();
            result.Services.Should().NotBeNull();
            result.OrderItemFunding.Should().NotBeNull();
            result.CatalogueItem.Should().NotBeNull();
            result.OrderItemPrice.Should().NotBeNull();
            result.OrderItemPrice.OrderItemPriceTiers.Should().NotBeNull();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task DetectChangesInFundingAndDelete_NotReadyForReview_OrderItemFundingUnchanged(
            Order order,
            OrderItemFunding funding,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] IOrderService mockOrderService,
            OrderItemService orderItemService)
        {
            var item = order.OrderItems.First();
            item.OrderItemFunding = funding;

            context.Orders.Add(order);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            mockOrderService.GetOrderWithOrderItems(order.CallOffId, order.OrderingParty.InternalIdentifier).Returns(new OrderWrapper(order));

            await orderItemService.DetectChangesInFundingAndDelete(order.CallOffId, order.OrderingParty.InternalIdentifier, item.Id);
            var actual = context.OrderItems
                .Include(i => i.OrderItemFunding)
                .FirstOrDefault(o => o.OrderId == item.OrderId && o.CatalogueItemId == item.CatalogueItemId);

            actual!.OrderItemFunding.OrderItemFundingType.Should().Be(funding.OrderItemFundingType);
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(FundingType.LocalFunding)]
        [MockInMemoryDbInlineAutoData(FundingType.Gpit)]
        [MockInMemoryDbInlineAutoData(FundingType.Pcarp)]
        public static async Task DetectChangesInFundingAndDelete_SingleFundingType_FrameworkSingleFundingType_OrderItemFundingUnchanged(
            FundingType fundingType,
            Order order,
            OrderItemFunding funding,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] IOrderService mockOrderService,
            OrderItemService orderItemService)
        {
            var item = order.OrderItems.First();
            funding.OrderItemFundingType = fundingType.AsOrderItemFundingType();
            item.OrderItemFunding = funding;

            order.OrderingParty.OrganisationType = OrganisationType.IB;
            order.SelectedFramework.FundingTypes = new List<FundingType> { fundingType };

            context.Orders.Add(order);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            mockOrderService.GetOrderWithOrderItems(order.CallOffId, order.OrderingParty.InternalIdentifier).Returns(new OrderWrapper(order));

            await orderItemService.DetectChangesInFundingAndDelete(order.CallOffId, order.OrderingParty.InternalIdentifier, item.Id);

            var actual = context.OrderItems
                .Include(i => i.OrderItemFunding)
                .FirstOrDefault(o => o.OrderId == item.OrderId && o.CatalogueItemId == item.CatalogueItemId);

            actual!.OrderItemFunding.OrderItemFundingType.Should().Be(fundingType.AsOrderItemFundingType());
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task DetectChangesInFundingAndDelete_GPPRactice_OrderItemFundingUnchanged(
            Order order,
            OrderItemFunding funding,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] IOrderService mockOrderService,
            OrderItemService orderItemService)
        {
            var item = order.OrderItems.First();
            funding.OrderItemFundingType = OrderItemFundingType.LocalFundingOnly;
            item.OrderItemFunding = funding;

            order.OrderingParty.OrganisationType = OrganisationType.GP;
            order.SelectedFramework.FundingTypes = new List<FundingType> { FundingType.Gpit };

            context.Orders.Add(order);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            mockOrderService.GetOrderWithOrderItems(order.CallOffId, order.OrderingParty.InternalIdentifier).Returns(new OrderWrapper(order));

            await orderItemService.DetectChangesInFundingAndDelete(order.CallOffId, order.OrderingParty.InternalIdentifier, item.Id);

            var actual = context.OrderItems
                .Include(i => i.OrderItemFunding)
                .FirstOrDefault(o => o.OrderId == item.OrderId && o.CatalogueItemId == item.CatalogueItemId);

            actual!.OrderItemFunding.OrderItemFundingType.Should().Be(OrderItemFundingType.LocalFundingOnly);
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(OrderItemFundingType.CentralFunding)]
        [MockInMemoryDbInlineAutoData(OrderItemFundingType.Gpit)]
        [MockInMemoryDbInlineAutoData(OrderItemFundingType.Pcarp)]
        [MockInMemoryDbInlineAutoData(OrderItemFundingType.MixedFunding)]
        [MockInMemoryDbInlineAutoData(OrderItemFundingType.NoFundingRequired)]
        [MockInMemoryDbInlineAutoData(OrderItemFundingType.None)]
        public static async Task DetectChangesInFundingAndDelete_FundingTypeChanged_SingleFundingTypeFramework_OrderItemFundingNull(
            OrderItemFundingType fundingType,
            Order order,
            OrderItemFunding funding,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] IOrderService mockOrderService,
            OrderItemService orderItemService)
        {
            var item = order.OrderItems.First();
            funding.OrderItemFundingType = fundingType;
            item.OrderItemFunding = funding;
            order.OrderingParty.OrganisationType = OrganisationType.IB;
            order.SelectedFramework.FundingTypes = new List<FundingType> { FundingType.LocalFunding };

            context.Orders.Add(order);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            mockOrderService.GetOrderWithOrderItems(order.CallOffId, order.OrderingParty.InternalIdentifier).Returns(new OrderWrapper(order));

            await orderItemService.DetectChangesInFundingAndDelete(order.CallOffId, order.OrderingParty.InternalIdentifier, item.Id);

            var actual = context.OrderItems.FirstOrDefault(o => o.OrderId == item.OrderId && o.CatalogueItemId == item.CatalogueItemId);

            actual!.OrderItemFunding.Should().BeNull();
        }

        [Theory]
        [MockInMemoryDbInlineAutoData(OrderItemFundingType.CentralFunding)]
        [MockInMemoryDbInlineAutoData(OrderItemFundingType.LocalFunding)]
        [MockInMemoryDbInlineAutoData(OrderItemFundingType.MixedFunding)]
        [MockInMemoryDbInlineAutoData(OrderItemFundingType.NoFundingRequired)]
        [MockInMemoryDbInlineAutoData(OrderItemFundingType.None)]
        public static async Task DetectChangesInFundingAndDelete_FundingTypeChanged_GPPractice_OrderItemFundingNull(
            OrderItemFundingType fundingType,
            Order order,
            OrderItemFunding funding,
            [Frozen] BuyingCatalogueDbContext context,
            [Frozen] IOrderService mockOrderService,
            OrderItemService orderItemService)
        {
            var item = order.OrderItems.First();
            funding.OrderItemFundingType = fundingType;
            item.OrderItemFunding = funding;

            order.OrderingParty.OrganisationType = OrganisationType.GP;
            order.SelectedFramework.FundingTypes = new List<FundingType> { FundingType.Pcarp };

            context.Orders.Add(order);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            mockOrderService.GetOrderWithOrderItems(order.CallOffId, order.OrderingParty.InternalIdentifier).Returns(new OrderWrapper(order));

            await orderItemService.DetectChangesInFundingAndDelete(order.CallOffId, order.OrderingParty.InternalIdentifier, item.Id);

            var actual = context.OrderItems.FirstOrDefault(o => o.OrderId == item.OrderId && o.CatalogueItemId == item.CatalogueItemId);

            actual!.OrderItemFunding.Should().BeNull();
        }

        [Theory]
        [MockInMemoryDbAutoData]
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

            await orderItemService.SetOrderItemEstimationPeriod(order.CallOffId, order.OrderingParty.InternalIdentifier, item.Id, price);

            var actual = context.OrderItems.FirstOrDefault(o => o.OrderId == item.OrderId && o.CatalogueItemId == item.CatalogueItemId);

            actual!.EstimationPeriod.Should().Be(TimeUnit.PerMonth);
        }

        [Theory]
        [MockInMemoryDbAutoData]
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

            await orderItemService.SetOrderItemEstimationPeriod(order.CallOffId, order.OrderingParty.InternalIdentifier, item.Id, price);

            var actual = context.OrderItems.FirstOrDefault(o => o.OrderId == item.OrderId && o.CatalogueItemId == item.CatalogueItemId);

            actual!.EstimationPeriod.Should().Be(TimeUnit.PerYear);
        }

        [Theory]
        [MockInMemoryDbAutoData]
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

            await orderItemService.SetOrderItemEstimationPeriod(order.CallOffId, order.OrderingParty.InternalIdentifier, item.Id, price);

            var actual = context.OrderItems.FirstOrDefault(o => o.OrderId == item.OrderId && o.CatalogueItemId == item.CatalogueItemId);

            actual!.EstimationPeriod.Should().Be(price.BillingPeriod);
        }
    }
}
