using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
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
        public static async Task DeleteOrderItem_RemovesOrderItem(
            [Frozen] BuyingCatalogueDbContext context,
            Order order,
            OrderItem orderItem1,
            OrderItem orderItem2,
            OrderItem orderItem3,
            bool fundingSource,
            OrderItemService service)
        {
            order.FundingSourceOnlyGms = fundingSource;
            order.AddOrUpdateOrderItem(orderItem1);
            order.AddOrUpdateOrderItem(orderItem2);
            order.AddOrUpdateOrderItem(orderItem3);

            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();

            await service.DeleteOrderItem(order.CallOffId, orderItem2.CatalogueItemId);

            var updatedOrder = await context.Orders.FirstOrDefaultAsync();

            updatedOrder.Should().NotBeNull();
            updatedOrder.OrderItems.Should().Contain(o => o.CatalogueItemId == orderItem1.CatalogueItemId);
            updatedOrder.OrderItems.Should().NotContain(o => o.CatalogueItemId == orderItem2.CatalogueItemId);
            updatedOrder.OrderItems.Should().Contain(o => o.CatalogueItemId == orderItem3.CatalogueItemId);
            updatedOrder.FundingSourceOnlyGms.Should().Be(fundingSource);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task DeleteOrderItem_AllItemsRemoved_RemovesFundingSource(
            [Frozen] BuyingCatalogueDbContext context,
            Order order,
            OrderItem orderItem,
            bool fundingSource,
            OrderItemService service)
        {
            order.FundingSourceOnlyGms = fundingSource;
            order.AddOrUpdateOrderItem(orderItem);

            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();

            await service.DeleteOrderItem(order.CallOffId, orderItem.CatalogueItemId);

            var updatedOrder = await context.Orders.AsAsyncEnumerable().FirstOrDefaultAsync();

            updatedOrder.Should().NotBeNull();
            updatedOrder.OrderItems.Should().NotContain(o => o.CatalogueItemId == orderItem.CatalogueItemId);
            updatedOrder.FundingSourceOnlyGms.Should().BeNull();
        }
    }
}
