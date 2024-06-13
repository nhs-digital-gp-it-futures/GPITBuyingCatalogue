using System;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Orders;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Orders
{
    public static class OrderingPartyServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrderingPartyService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static void SetOrderingPartyContact_NullContact_ThrowsException(
            CallOffId callOffId,
            OrderingPartyService service)
        {
            FluentActions
                .Awaiting(() => service.SetOrderingPartyContact(callOffId, null))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task SetOrderingPartyContact_ValidContact_SetsContact(
            Order order,
            Contact contact,
            [Frozen] BuyingCatalogueDbContext context,
            OrderingPartyService service)
        {
            order.OrderingPartyContactId = null;
            order.OrderingPartyContact = null;
            contact.Id = 0;
            contact.LastUpdatedBy = null;
            contact.LastUpdatedByUser = null;

            context.Orders.Add(order);

            await context.SaveChangesAsync();

            await service.SetOrderingPartyContact(order.CallOffId, contact);

            var result = await context.Order(order.CallOffId);

            result.OrderingPartyContact.Should().BeEquivalentTo(contact);
        }
    }
}
