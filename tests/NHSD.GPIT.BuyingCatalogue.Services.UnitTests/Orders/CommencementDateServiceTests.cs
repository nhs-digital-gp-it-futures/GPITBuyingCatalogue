using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Orders
{
    public static class CommencementDateServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(CommencementDateService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task SetCommencementDate_UpdatesDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Order order,
            DateTime commencementDate,
            int initialPeriod,
            int maximumTerm,
            CommencementDateService service)
        {
            context.Orders.Add(order);

            await context.SaveChangesAsync();
            await service.SetCommencementDate(order.CallOffId, order.OrderingParty.InternalIdentifier, commencementDate, initialPeriod, maximumTerm);

            var updatedOrder = await context.Orders.FirstAsync();

            updatedOrder.CommencementDate.Should().Be(commencementDate);
            updatedOrder.InitialPeriod.Should().Be(initialPeriod);
            updatedOrder.MaximumTerm.Should().Be(maximumTerm);
        }
    }
}
