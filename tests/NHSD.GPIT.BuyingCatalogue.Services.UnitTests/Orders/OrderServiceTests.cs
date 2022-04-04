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
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Orders
{
    public static class OrderServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task CreateOrder_UpdatesDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            string description,
            Organisation organisation,
            OrderService service)
        {
            await context.Organisations.AddAsync(organisation);
            await context.SaveChangesAsync();

            await service.CreateOrder(description, organisation.InternalIdentifier);

            var order = await context.Orders.Include(o => o.OrderingParty).SingleAsync();
            order.Description.Should().Be(description);
            order.OrderingParty.InternalIdentifier.Should().Be(organisation.InternalIdentifier);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task DeleteOrder_SoftDeletedOrder(
            [Frozen] BuyingCatalogueDbContext context,
            Order order,
            OrderService service)
        {
            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();

            await service.DeleteOrder(order.CallOffId, order.OrderingParty.InternalIdentifier);

            var updatedOrder = await context.Orders.FirstOrDefaultAsync();

            // Although soft deleted, there is a query filter on the context to exclude soft deleted orders
            updatedOrder.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetPagedOrders_ReturnsExpectedPageSize(
            Organisation organisation,
            List<Order> orders,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            organisation.Orders = orders;

            context.Orders.AddRange(orders);
            context.Organisations.Add(organisation);

            context.SaveChanges();

            var result = await service.GetPagedOrders(organisation.Id, new PageOptions("0", 2));

            result.Items.Count.Should().Be(2);
            result.Options.TotalNumberOfItems.Should().Be(orders.Count);

            var expectedNumberOfPages = (int)Math.Ceiling((double)orders.Count / result.Options.PageSize);
            result.Options.NumberOfPages
                .Should()
                .Be(expectedNumberOfPages);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetPagedOrders_SearchTerm_ReturnsExpectedResults(
            Organisation organisation,
            List<Order> orders,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            organisation.Orders = orders;

            context.Orders.AddRange(orders);
            context.Organisations.Add(organisation);

            context.SaveChanges();

            var order = orders.First();
            var searchTerm = order.CallOffId.ToString();

            var result = await service.GetPagedOrders(organisation.Id, new PageOptions("0", 2), searchTerm);

            result.Items.First().Should().BeEquivalentTo(order);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetOrdersBySearchTerm_CallOffId_ReturnsExpectedResults(
            Organisation organisation,
            List<Order> orders,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            organisation.Orders = orders;

            context.Organisations.Add(organisation);
            context.Orders.AddRange(orders);

            context.SaveChanges();

            var order = orders.First();
            var searchTerm = order.CallOffId.ToString()[5..];

            var results = await service.GetOrdersBySearchTerm(organisation.Id, searchTerm);

            results.Should().NotBeEmpty();

            var actual = results.First();
            actual.Category.Should().Be(order.CallOffId.ToString());
            actual.Title.Should().Be(order.Description);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetOrdersBySearchTerm_Description_ReturnsExpectedResults(
            Organisation organisation,
            List<Order> orders,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            organisation.Orders = orders;

            context.Organisations.Add(organisation);
            context.Orders.AddRange(orders);

            context.SaveChanges();

            var order = orders.First();
            var searchTerm = order.Description[..15];

            var results = await service.GetOrdersBySearchTerm(organisation.Id, searchTerm);

            results.Should().NotBeEmpty();

            var actual = results.First();
            actual.Category.Should().Be(order.CallOffId.ToString());
            actual.Title.Should().Be(order.Description);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetUserOrders_ReturnsExpectedResults(
            int userId,
            List<Order> orders,
            [Frozen] Mock<IIdentityService> mockIdentityService,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            mockIdentityService
                .Setup(x => x.GetUserId())
                .Returns(userId);

            context.Orders.AddRange(orders);
            context.SaveChanges();

            var results = await service.GetUserOrders(userId);

            results.Should().BeEquivalentTo(orders);
        }
    }
}
