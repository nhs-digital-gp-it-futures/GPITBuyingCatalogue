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
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
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
            OrderTriageValue orderTriageValue,
            Organisation organisation,
            OrderService service)
        {
            await context.Organisations.AddAsync(organisation);
            await context.SaveChangesAsync();

            await service.CreateOrder(description, organisation.InternalIdentifier, orderTriageValue, false);

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
        public static async Task GetOrderForSummary_CompletedOrder_ReturnsExpectedResultsAsAtCompletionDate(
            Order order,
            Supplier supplier,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            const string junk = "Junk";

            order.SupplierId = supplier.Id;
            order.Supplier = supplier;

            context.Suppliers.Add(supplier);
            context.Orders.Add(order);

            await context.SaveChangesAsync();

            var result = await service.GetOrderForSummary(order.CallOffId, order.OrderingParty.InternalIdentifier);

            result.Supplier.Address.Should().BeEquivalentTo(supplier.Address);

            order.Complete();

            await context.SaveChangesAsync();

            supplier.Address.County += junk;
            supplier.Address.Country += junk;
            supplier.Address.Line1 += junk;
            supplier.Address.Line2 += junk;
            supplier.Address.Line3 += junk;
            supplier.Address.Line4 += junk;
            supplier.Address.Line5 += junk;
            supplier.Address.Postcode += junk;
            supplier.Address.Town += junk;

            await context.SaveChangesAsync();

            var actual = await service.GetOrderForSummary(order.CallOffId, order.OrderingParty.InternalIdentifier);

            actual.Supplier.Address.Should().NotBeEquivalentTo(supplier.Address);
            actual.Supplier.Address.County.Should().Be(supplier.Address.County.Replace(junk, string.Empty));
            actual.Supplier.Address.Country.Should().Be(supplier.Address.Country.Replace(junk, string.Empty));
            actual.Supplier.Address.Line1.Should().Be(supplier.Address.Line1.Replace(junk, string.Empty));
            actual.Supplier.Address.Line2.Should().Be(supplier.Address.Line2.Replace(junk, string.Empty));
            actual.Supplier.Address.Line3.Should().Be(supplier.Address.Line3.Replace(junk, string.Empty));
            actual.Supplier.Address.Line4.Should().Be(supplier.Address.Line4.Replace(junk, string.Empty));
            actual.Supplier.Address.Line5.Should().Be(supplier.Address.Line5.Replace(junk, string.Empty));
            actual.Supplier.Address.Postcode.Should().Be(supplier.Address.Postcode.Replace(junk, string.Empty));
            actual.Supplier.Address.Town.Should().Be(supplier.Address.Town.Replace(junk, string.Empty));
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

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetSolutionId_UpdatesDatabase(
            Order order,
            CatalogueItemId solutionId,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            order.SolutionId = null;
            order.Solution = null;

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            (await context.Orders.SingleAsync(x => x.Id == order.Id)).SolutionId.Should().BeNull();

            await service.SetSolutionId(order.OrderingParty.InternalIdentifier, order.CallOffId, solutionId);

            (await context.Orders.SingleAsync(x => x.Id == order.Id)).SolutionId.Should().Be(solutionId);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetDeliveryDate_UpdatesDatabase(
            Order order,
            DateTime deliveryDate,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            order.DeliveryDate = null;
            order.OrderItems.ForEach(x => x.OrderItemRecipients.ForEach(r => r.DeliveryDate = null));

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            var dbOrder = await context.Orders.SingleAsync(x => x.Id == order.Id);

            dbOrder.DeliveryDate.Should().BeNull();
            dbOrder.OrderItems.ForEach(x => x.OrderItemRecipients.ForEach(r => r.DeliveryDate.Should().BeNull()));

            await service.SetDeliveryDate(order.OrderingParty.InternalIdentifier, order.CallOffId, deliveryDate);

            dbOrder = await context.Orders.SingleAsync(x => x.Id == order.Id);

            dbOrder.DeliveryDate.Should().Be(deliveryDate);
            dbOrder.OrderItems.ForEach(x => x.OrderItemRecipients.ForEach(r => r.DeliveryDate.Should().Be(deliveryDate)));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetDeliveryDates_UpdatesDatabase(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            var initialDate = DateTime.Today;
            var newDate = initialDate.AddDays(1);

            order.DeliveryDate = initialDate;
            order.OrderItems.ForEach(x => x.OrderItemRecipients.ForEach(r => r.DeliveryDate = initialDate));

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            var dbOrder = await context.Orders.SingleAsync(x => x.Id == order.Id);

            dbOrder.DeliveryDate.Should().Be(initialDate);
            dbOrder.OrderItems.ForEach(x => x.OrderItemRecipients.ForEach(r => r.DeliveryDate.Should().Be(initialDate)));

            var orderItem = order.OrderItems.First();
            var catalogueItemId = orderItem.CatalogueItemId;

            var deliveryDates = orderItem.OrderItemRecipients
                .Select(x => new OrderDeliveryDateDto(x.OdsCode, newDate))
                .ToList();

            await service.SetDeliveryDates(order.Id, catalogueItemId, deliveryDates);

            dbOrder = await context.Orders.SingleAsync(x => x.Id == order.Id);
            orderItem = dbOrder.OrderItem(catalogueItemId);

            orderItem.OrderItemRecipients.ForEach(x => x.DeliveryDate.Should().Be(newDate));
        }
    }
}
