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
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Orders
{
    public static class DefaultDeliveryDateServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(DefaultDeliveryDateService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetDefaultDeliveryDate_UpdatesDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Order order,
            ICollection<DefaultDeliveryDate> defaultDeliveryDates,
            DateTime deliveryDate,
            DefaultDeliveryDateService service)
        {
            order.DefaultDeliveryDates = defaultDeliveryDates;
            context.Orders.Add(order);

            await context.SaveChangesAsync();

            await service.SetDefaultDeliveryDate(order.CallOffId, order.OrderingParty.InternalIdentifier, order.DefaultDeliveryDates.First().CatalogueItemId, deliveryDate);

            var actual = await context.Orders
                .Include(o => o.DefaultDeliveryDates)
                .SingleOrDefaultAsync();

            actual.DefaultDeliveryDates.Single(d => d.CatalogueItemId == order.DefaultDeliveryDates.First().CatalogueItemId)
                .DeliveryDate.Should().Be(deliveryDate);
        }
    }
}
