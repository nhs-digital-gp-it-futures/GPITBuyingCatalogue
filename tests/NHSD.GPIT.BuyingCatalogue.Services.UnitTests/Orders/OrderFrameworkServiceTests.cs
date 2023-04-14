using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Notify.Client;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Orders
{
    public static class OrderFrameworkServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderFrameworkService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpdateFundingSourceAndSetSelectedFrameworkForOrder_FrameworkLocalFundingOnly_UpdatesDatabase(
            Order order,
            OrderItem orderItem,
            [Frozen] BuyingCatalogueDbContext context,
            OrderFrameworkService service)
        {
            order.SelectedFramework.LocalFundingOnly = true;
            orderItem.OrderItemFunding = null;
            order.OrderItems = new List<OrderItem>() { orderItem };
            context.Orders.Add(order);

            await context.SaveChangesAsync();

            await service.UpdateFundingSourceAndSetSelectedFrameworkForOrder(order.CallOffId, order.OrderingParty.InternalIdentifier, order.SelectedFramework.Id);

            var result = await context.Orders.FirstAsync(x => x.Id == order.Id);
            var orderItemFunding = result.OrderItems.First().OrderItemFunding;
            orderItemFunding.Should().NotBeNull();
            orderItemFunding.OrderId.Should().Be(order.Id);
            orderItemFunding.CatalogueItemId.Should().Be(orderItem.CatalogueItemId);
            orderItemFunding.OrderItemFundingType.Should().Be(OrderItemFundingType.LocalFundingOnly);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpdateFundingSourceAndSetSelectedFrameworkForOrder_GpPractice_UpdatesDatabase(
            Order order,
            Organisation organisation,
            OrderItem orderItem,
            [Frozen] BuyingCatalogueDbContext context,
            OrderFrameworkService service)
        {
            organisation.OrganisationType = OrganisationType.GP;
            order.OrderingParty = organisation;
            orderItem.OrderItemFunding = null;
            order.OrderItems = new List<OrderItem>() { orderItem };
            context.Orders.Add(order);

            await context.SaveChangesAsync();

            await service.UpdateFundingSourceAndSetSelectedFrameworkForOrder(order.OrderingParty.InternalIdentifier, order.CallOffId);

            var result = await context.Orders.FirstAsync(x => x.Id == order.Id);
            var orderItemFunding = result.OrderItems.First().OrderItemFunding;
            orderItemFunding.Should().NotBeNull();
            orderItemFunding.OrderId.Should().Be(order.Id);
            orderItemFunding.CatalogueItemId.Should().Be(orderItem.CatalogueItemId);
            orderItemFunding.OrderItemFundingType.Should().Be(OrderItemFundingType.LocalFundingOnly);
        }
    }
}
