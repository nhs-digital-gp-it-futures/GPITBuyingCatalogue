﻿using System;
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
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
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

            var order = await context.Orders.Include(o => o.OrderingParty).FirstAsync();

            order.OrderNumber.Should().Be(1);
            order.Revision.Should().Be(1);
            order.Description.Should().Be(description);
            order.OrderingParty.InternalIdentifier.Should().Be(organisation.InternalIdentifier);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AmendOrder_UpdatesDatabase(
            Order order,
            [Frozen] BuyingCatalogueDbContext context,
            OrderService service)
        {
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var result = await service.AmendOrder(order.OrderingParty.InternalIdentifier, order.CallOffId);

            result.OrderNumber.Should().Be(order.OrderNumber);
            result.Revision.Should().Be(order.CallOffId.Revision + 1);
            result.AssociatedServicesOnly.Should().Be(order.AssociatedServicesOnly);
            result.CommencementDate.Should().Be(order.CommencementDate);
            result.Description.Should().Be(order.Description);
            result.InitialPeriod.Should().Be(order.InitialPeriod);
            result.MaximumTerm.Should().Be(order.MaximumTerm);
            result.OrderingPartyId.Should().Be(order.OrderingPartyId);
            result.OrderTriageValue.Should().Be(order.OrderTriageValue);
            result.SelectedFrameworkId.Should().Be(order.SelectedFrameworkId);
            result.SupplierId.Should().Be(order.SupplierId);

            result.OrderingPartyContact.Id.Should().NotBe(order.OrderingPartyContact.Id);
            result.SupplierContact.Id.Should().NotBe(order.SupplierContact.Id);

            result.OrderingPartyContact.FirstName.Should().Be(order.OrderingPartyContact.FirstName);
            result.OrderingPartyContact.LastName.Should().Be(order.OrderingPartyContact.LastName);
            result.OrderingPartyContact.Department.Should().Be(order.OrderingPartyContact.Department);
            result.OrderingPartyContact.Email.Should().Be(order.OrderingPartyContact.Email);
            result.OrderingPartyContact.Phone.Should().Be(order.OrderingPartyContact.Phone);

            result.SupplierContact.FirstName.Should().Be(order.SupplierContact.FirstName);
            result.SupplierContact.LastName.Should().Be(order.SupplierContact.LastName);
            result.SupplierContact.Department.Should().Be(order.SupplierContact.Department);
            result.SupplierContact.Email.Should().Be(order.SupplierContact.Email);
            result.SupplierContact.Phone.Should().Be(order.SupplierContact.Phone);
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

            var result = (await service.GetOrderForSummary(order.CallOffId, order.OrderingParty.InternalIdentifier)).Order;

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

            var actual = (await service.GetOrderForSummary(order.CallOffId, order.OrderingParty.InternalIdentifier)).Order;

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

            (PagedList<Order> pagedOrders, IEnumerable<CallOffId> orderIds) = await service.GetPagedOrders(organisation.Id, new PageOptions("0", 2));

            orderIds.Should().BeEquivalentTo(orders.Select(x => x.CallOffId));

            pagedOrders.Items.Count.Should().Be(2);
            pagedOrders.Options.TotalNumberOfItems.Should().Be(orders.Count);

            var expected = (int)Math.Ceiling((double)orders.Count / pagedOrders.Options.PageSize);

            pagedOrders.Options.NumberOfPages.Should().Be(expected);
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

            result.Orders.Items.First().Should().BeEquivalentTo(order);
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

            (await context.Orders.FirstAsync(x => x.Id == order.Id)).SolutionId.Should().BeNull();

            await service.SetSolutionId(order.OrderingParty.InternalIdentifier, order.CallOffId, solutionId);

            (await context.Orders.FirstAsync(x => x.Id == order.Id)).SolutionId.Should().Be(solutionId);
        }
    }
}
