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
        [CommonAutoData]
        public static async Task OrderItemShouldHaveForcedFundingType_Solution_0Value_NofundingRequired(
            OrderItem item,
            OrderItemService service)
        {
            item.OrderItemPrice.OrderItemPriceTiers.ForEach(oipt => oipt.Price = 0);
            item.Quantity = 0;

            var (isForcedFundingResult, fundingTypeResult) = await service.OrderItemShouldHaveForcedFundingType(item);

            isForcedFundingResult.Should().BeTrue();
            fundingTypeResult.Should().Be(OrderItemFundingType.NoFundingRequired);
        }

        [Theory]
        [CommonAutoData]
        public static async Task OrderItemShouldHaveForcedFundingType_AssociatedService_HasValue_NotForcedFunding(
            OrderItem item,
            OrderItemService service)
        {
            item.CatalogueItem.CatalogueItemType = CatalogueItemType.AssociatedService;

            var (isForcedFundingResult, fundingTypeResult) = await service.OrderItemShouldHaveForcedFundingType(item);

            isForcedFundingResult.Should().BeFalse();
            fundingTypeResult.Should().Be(OrderItemFundingType.None);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task OrderItemShouldHaveForcedFundingType_Solution_HasValue_LocalFundingOnly(
            OrderItem item,
            OrderItemService service,
            [Frozen] BuyingCatalogueDbContext context)
        {
            context.Frameworks.Add(new EntityFramework.Catalogue.Models.Framework { Id = "LOCAL", LocalFundingOnly = true, Name = "local funding framework" });
            item.CatalogueItem.Solution = new Solution { CatalogueItemId = item.CatalogueItemId };
            item.CatalogueItem.Solution.FrameworkSolutions.Add(new FrameworkSolution { SolutionId = item.CatalogueItemId, FrameworkId = "LOCAL" });
            item.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            context.CatalogueItems.Add(item.CatalogueItem);

            await context.SaveChangesAsync();

            var (isForcedFundingResult, fundingTypeResult) = await service.OrderItemShouldHaveForcedFundingType(item);

            isForcedFundingResult.Should().BeTrue();
            fundingTypeResult.Should().Be(OrderItemFundingType.LocalFundingOnly);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task OrderItemShouldHaveForcedFundingType_Solution_HasValue_NotForcedFunding(
            OrderItem item,
            OrderItemService service,
            [Frozen] BuyingCatalogueDbContext context)
        {
            var frameworkId = "NotForcedFunding";

            context.Frameworks.Add(new EntityFramework.Catalogue.Models.Framework { Id = frameworkId, LocalFundingOnly = false, Name = "NFF framework" });
            item.CatalogueItem.Solution = new Solution { CatalogueItemId = item.CatalogueItemId };
            item.CatalogueItem.Solution.FrameworkSolutions.Add(new FrameworkSolution { SolutionId = item.CatalogueItemId, FrameworkId = frameworkId });
            item.CatalogueItem.CatalogueItemType = CatalogueItemType.Solution;
            context.CatalogueItems.Add(item.CatalogueItem);

            await context.SaveChangesAsync();

            var (isForcedFundingResult, fundingTypeResult) = await service.OrderItemShouldHaveForcedFundingType(item);

            isForcedFundingResult.Should().BeFalse();
            fundingTypeResult.Should().Be(OrderItemFundingType.None);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task OrderItemShouldHaveForcedFundingType_AdditionalService_HasValue_LocalFundingOnly(
            OrderItem item,
            OrderItemService service,
            [Frozen] BuyingCatalogueDbContext context)
        {
            var solutionId = new CatalogueItemId(1001, "001");
            var frameworkId = "LOCAL";

            context.Frameworks.Add(new EntityFramework.Catalogue.Models.Framework { Id = frameworkId, LocalFundingOnly = true, Name = "local funding framework" });

            var solution = new CatalogueItem
            {
                Id = solutionId,
                CatalogueItemType = CatalogueItemType.Solution,
                Name = "Solution",
                Solution = new Solution
                {
                    CatalogueItemId = solutionId,
                    FrameworkSolutions = new List<FrameworkSolution>
                    {
                        new FrameworkSolution
                        {
                            SolutionId = solutionId,
                            FrameworkId = frameworkId,
                        },
                    },
                },
            };
            context.CatalogueItems.Add(solution);

            item.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;

            item.CatalogueItem.AdditionalService = new AdditionalService
            {
                CatalogueItemId = item.CatalogueItemId,
                SolutionId = solutionId,
            };

            context.CatalogueItems.Add(item.CatalogueItem);

            await context.SaveChangesAsync();

            var (isForcedFundingResult, fundingTypeResult) = await service.OrderItemShouldHaveForcedFundingType(item);

            isForcedFundingResult.Should().BeTrue();
            fundingTypeResult.Should().Be(OrderItemFundingType.LocalFundingOnly);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task OrderItemShouldHaveForcedFundingType_AdditionalService_HasValue_NotForcedFunding(
            OrderItem item,
            OrderItemService service,
            [Frozen] BuyingCatalogueDbContext context)
        {
            var solutionId = new CatalogueItemId(1001, "001");
            var frameworkId = "NotForcedFunding";

            context.Frameworks.Add(new EntityFramework.Catalogue.Models.Framework { Id = frameworkId, LocalFundingOnly = false, Name = "NFF framework" });

            var solution = new CatalogueItem
            {
                Id = solutionId,
                CatalogueItemType = CatalogueItemType.Solution,
                Name = "Solution",
                Solution = new Solution
                {
                    CatalogueItemId = solutionId,
                    FrameworkSolutions = new List<FrameworkSolution>
                    {
                        new FrameworkSolution
                        {
                            SolutionId = solutionId,
                            FrameworkId = frameworkId,
                        },
                    },
                },
            };
            context.CatalogueItems.Add(solution);

            item.CatalogueItem.CatalogueItemType = CatalogueItemType.AdditionalService;

            item.CatalogueItem.AdditionalService = new AdditionalService
            {
                CatalogueItemId = item.CatalogueItemId,
                SolutionId = solutionId,
            };

            context.CatalogueItems.Add(item.CatalogueItem);

            await context.SaveChangesAsync();

            var (isForcedFundingResult, fundingTypeResult) = await service.OrderItemShouldHaveForcedFundingType(item);

            isForcedFundingResult.Should().BeFalse();
            fundingTypeResult.Should().Be(OrderItemFundingType.None);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SaveOrUpdateFundingIfItemIsLocalOrNoFunding_NoFunding_ShouldHaveForcedFunding_SetsForcedFunding(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            OrderItemService orderItemService)
        {
            var selectedFundingType = OrderItemFundingType.NoFundingRequired;

            var item = order.OrderItems.First();

            item.OrderItemFunding = null;
            item.OrderItemPrice.OrderItemPriceTiers.ForEach(oipt => oipt.Price = 0);

            context.Orders.Add(order);

            _ = await context.SaveChangesAsync();

            await orderItemService.SaveOrUpdateFundingIfItemIsLocalOrNoFunding(order.CallOffId, order.OrderingParty.InternalIdentifier, item.CatalogueItemId);

            var actual = context.OrderItems.FirstOrDefault(o => o.OrderId == item.OrderId && o.CatalogueItemId == item.CatalogueItemId);

            actual.OrderItemFunding.Should().NotBeNull();
            actual.OrderItemFunding.OrderItemFundingType.Should().Be(selectedFundingType);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SaveOrUpdateFundingIfItemIsLocalOrNoFunding_CurrentlyForcedFunding_ShouldNotHaveForcedFunding_DeletesFunding(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            OrderItemService orderItemService)
        {
            var selectedFundingType = OrderItemFundingType.NoFundingRequired;

            var item = order.OrderItems.First();

            item.OrderItemFunding.OrderItemFundingType = selectedFundingType;

            context.Orders.Add(order);

            _ = await context.SaveChangesAsync();

            await orderItemService.SaveOrUpdateFundingIfItemIsLocalOrNoFunding(order.CallOffId, order.OrderingParty.InternalIdentifier, item.CatalogueItemId);

            var actual = context.OrderItems.FirstOrDefault(o => o.OrderId == item.OrderId && o.CatalogueItemId == item.CatalogueItemId);

            actual.OrderItemFunding.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SaveOrUpdateFundingIfItemIsLocalOrNoFunding_NoFunding_ShouldNotHaveForcedFunding_NoFundingSet(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            OrderItemService orderItemService)
        {
            var item = order.OrderItems.First();

            item.OrderItemFunding = null;

            context.Orders.Add(order);

            _ = await context.SaveChangesAsync();

            await orderItemService.SaveOrUpdateFundingIfItemIsLocalOrNoFunding(order.CallOffId, order.OrderingParty.InternalIdentifier, item.CatalogueItemId);

            var actual = context.OrderItems.FirstOrDefault(o => o.OrderId == item.OrderId && o.CatalogueItemId == item.CatalogueItemId);

            actual.OrderItemFunding.Should().BeNull();
        }
    }
}
