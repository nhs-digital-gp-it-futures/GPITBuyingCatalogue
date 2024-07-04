using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Orders
{
    public static class OrderAdminServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderAdminService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockInMemoryDbInlineAutoData("0")]
        [MockInMemoryDbInlineAutoData("1")]
        public static async Task GetPagedOrders_ReturnsExpectedPageSize(
            string page,
            Organisation organisation,
            List<Order> orders,
            [Frozen] BuyingCatalogueDbContext context,
            OrderAdminService service)
        {
            organisation.Orders = orders;

            context.Orders.AddRange(orders);
            context.Organisations.Add(organisation);

            context.SaveChanges();

            var pagedOrders = await service.GetPagedOrders(new PageOptions(page, 2));

            pagedOrders.Items.Count.Should().Be(2);
            pagedOrders.Options.TotalNumberOfItems.Should().Be(orders.Count);

            var expected = (int)Math.Ceiling((double)orders.Count / pagedOrders.Options.PageSize);

            pagedOrders.Options.NumberOfPages.Should().Be(expected);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetPagedOrders_SearchTerm_CallOffIDReturnsExpectedResults(
            Organisation organisation,
            List<Order> orders,
            [Frozen] BuyingCatalogueDbContext context,
            OrderAdminService service)
        {
            organisation.Orders = orders;

            context.Orders.AddRange(orders);
            context.Organisations.Add(organisation);
            context.SaveChanges();

            var order = orders.First();
            var searchTerm = order.CallOffId.ToString();
            var searchTermType = "Call-off ID";

            var result = await service.GetPagedOrders(new PageOptions("0", 2), searchTerm, searchTermType);

            result.Items.First().CallOffId.Should().BeEquivalentTo(order.CallOffId);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetPagedOrders_Framework_ReturnsExpectedResults(
            Organisation organisation,
            EntityFramework.Catalogue.Models.Framework framework,
            List<Order> orders,
            [Frozen] BuyingCatalogueDbContext context,
            OrderAdminService service)
        {
            for (int i = 1; i < orders.Count; i++)
            {
                orders[i].SelectedFramework = framework;
                orders[i].SelectedFrameworkId = framework.Id;
            }

            organisation.Orders = orders;

            context.Frameworks.Add(framework);
            context.Orders.AddRange(orders);
            context.Organisations.Add(organisation);

            context.SaveChanges();

            var result = await service.GetPagedOrders(new PageOptions("0", 2), framework: framework.Id);

            result.Items.Should().NotBeNull();
            result.Items.Count.Should().Be(orders.Count - 1);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetPagedOrders_TerminatedStatus_ReturnsExpectedResults(
            Organisation organisation,
            List<Order> orders,
            [Frozen] BuyingCatalogueDbContext context,
            OrderAdminService service)
        {
            for (int i = 1; i < orders.Count; i++)
            {
                orders[i].IsTerminated = true;
            }

            organisation.Orders = orders;

            context.Orders.AddRange(orders);
            context.Organisations.Add(organisation);

            context.SaveChanges();

            var result = await service.GetPagedOrders(new PageOptions("0", 2), status: OrderStatus.Terminated);

            result.Items.Should().NotBeNull();
            result.Items.Count.Should().Be(orders.Count - 1);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetPagedOrders_DeletedStatus_ReturnsExpectedResults(
            Organisation organisation,
            List<Order> orders,
            [Frozen] BuyingCatalogueDbContext context,
            OrderAdminService service)
        {
            for (int i = 1; i < orders.Count; i++)
            {
                orders[i].IsDeleted = true;
            }

            organisation.Orders = orders;

            context.Orders.AddRange(orders);
            context.Organisations.Add(organisation);

            context.SaveChanges();

            var result = await service.GetPagedOrders(new PageOptions("0", 2), status: OrderStatus.Deleted);

            result.Items.Should().NotBeNull();
            result.Items.Count.Should().Be(orders.Count - 1);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetPagedOrders_InProgressStatus_ReturnsExpectedResults(
            Organisation organisation,
            List<Order> orders,
            [Frozen] BuyingCatalogueDbContext context,
            OrderAdminService service,
            DateTime completedDate)
        {
            orders.First().Completed = completedDate;
            for (int i = 1; i < orders.Count; i++)
            {
                orders[i].Completed = null;
            }

            organisation.Orders = orders;

            context.Orders.AddRange(orders);
            context.Organisations.Add(organisation);

            context.SaveChanges();

            var result = await service.GetPagedOrders(new PageOptions("0", 2), status: OrderStatus.InProgress);

            result.Items.Should().NotBeNull();
            result.Items.Count.Should().Be(orders.Count - 1);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task GetPagedOrders_CompletedStatus_ReturnsExpectedResults(
            Organisation organisation,
            List<Order> orders,
            [Frozen] BuyingCatalogueDbContext context,
            OrderAdminService service,
            DateTime completedDate)
        {
            orders.First().Completed = null;
            for (int i = 1; i < orders.Count; i++)
            {
                orders[i].Completed = completedDate;
            }

            organisation.Orders = orders;

            context.Orders.AddRange(orders);
            context.Organisations.Add(organisation);

            context.SaveChanges();

            var result = await service.GetPagedOrders(new PageOptions("0", 2), status: OrderStatus.Completed);

            result.Items.Should().NotBeNull();
            result.Items.Count.Should().Be(orders.Count - 1);
        }
    }
}
