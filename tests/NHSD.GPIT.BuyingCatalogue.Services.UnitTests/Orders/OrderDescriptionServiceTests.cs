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
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Orders
{
    public static class OrderDescriptionServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderDescriptionService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetOrderDescription_UpdatesDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Order order,
            string description,
            OrderDescriptionService service)
        {
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            await service.SetOrderDescription(order.CallOffId, order.OrderingParty.InternalIdentifier, description);

            var updatedOrder = await context.Orders.SingleAsync();
            updatedOrder.Description.Should().Be(description);
        }
    }
}
