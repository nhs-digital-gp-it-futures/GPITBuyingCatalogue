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
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
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
        [InMemoryDbInlineAutoData(true)]
        [InMemoryDbInlineAutoData(false)]
        public static async Task UpdateFundingSourceAndSetSelectedFrameworkForOrder_NotGP_LocalFrameworkOnlyUnchanged_UpdatesSelectedFramework(
            bool localFundingOnly,
            Order order,
            EntityFramework.Catalogue.Models.Framework selectedFramework,
            [Frozen] BuyingCatalogueDbContext context,
            OrderFrameworkService service)
        {
            selectedFramework.LocalFundingOnly = localFundingOnly;

            context.Frameworks.Add(selectedFramework);

            order.OrderingParty.OrganisationType = OrganisationType.IB;

            order.SelectedFramework.LocalFundingOnly = localFundingOnly;

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            await service.UpdateFundingSourceAndSetSelectedFrameworkForOrder(order.CallOffId, order.OrderingParty.InternalIdentifier, selectedFramework.Id);

            var result = await context.Orders.FirstAsync(x => x.Id == order.Id);
            result.SelectedFramework.Should().BeEquivalentTo(selectedFramework);
        }

        [Theory]
        [InMemoryDbInlineAutoData(true)]
        [InMemoryDbInlineAutoData(false)]
        public static async Task UpdateFundingSourceAndSetSelectedFrameworkForOrder_GPPractice_LocalFrameworkOnlyChanged_UpdatesSelectedFramework(
            bool localFundingOnly,
            Order order,
            EntityFramework.Catalogue.Models.Framework selectedFramework,
            [Frozen] BuyingCatalogueDbContext context,
            OrderFrameworkService service)
        {
            selectedFramework.LocalFundingOnly = localFundingOnly;

            context.Frameworks.Add(selectedFramework);

            order.OrderingParty.OrganisationType = OrganisationType.GP;

            order.SelectedFramework.LocalFundingOnly = !localFundingOnly;

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            await service.UpdateFundingSourceAndSetSelectedFrameworkForOrder(order.CallOffId, order.OrderingParty.InternalIdentifier, selectedFramework.Id);

            var result = await context.Orders.FirstAsync(x => x.Id == order.Id);
            result.SelectedFramework.Should().BeEquivalentTo(selectedFramework);
        }

        [Theory]
        [InMemoryDbInlineAutoData(true)]
        [InMemoryDbInlineAutoData(false)]
        public static async Task UpdateFundingSourceAndSetSelectedFrameworkForOrder_NotGP_LocalFrameworkOnlyChanged_OrderItemFundingNull(
            bool localFundingOnly,
            Order order,
            OrderItem orderItem,
            EntityFramework.Catalogue.Models.Framework selectedFramework,
            [Frozen] BuyingCatalogueDbContext context,
            OrderFrameworkService service)
        {
            selectedFramework.LocalFundingOnly = localFundingOnly;

            context.Frameworks.Add(selectedFramework);

            orderItem.OrderItemFunding.OrderItemFundingType = OrderItemFundingType.LocalFundingOnly;

            order.OrderItems.Clear();

            order.OrderItems.Add(orderItem);

            order.OrderingParty.OrganisationType = OrganisationType.IB;

            order.SelectedFramework.LocalFundingOnly = !localFundingOnly;

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            await service.UpdateFundingSourceAndSetSelectedFrameworkForOrder(order.CallOffId, order.OrderingParty.InternalIdentifier, selectedFramework.Id);

            var result = await context.Orders.FirstAsync(x => x.Id == order.Id);
            result.SelectedFramework.Should().BeEquivalentTo(selectedFramework);
            result.OrderItems.Count.Should().Be(order.OrderItems.Count);
            result.OrderItems.First().OrderItemFunding.Should().BeNull();
        }
    }
}
