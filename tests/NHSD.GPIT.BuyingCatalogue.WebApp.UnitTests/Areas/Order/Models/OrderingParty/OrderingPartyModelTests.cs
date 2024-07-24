using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.OrderingParty;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.OrderingParty
{
    public static class OrderingPartyModelTests
    {
        [Theory]
        [MockAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order)
        {
            var model = new OrderingPartyModel(internalOrgId, order);

            model.InternalOrgId.Should().Be(internalOrgId);
            model.CallOffId.Should().Be(order.CallOffId.ToString());
            model.FirstName.Should().Be(order.OrderingPartyContact.FirstName);
            model.LastName.Should().Be(order.OrderingPartyContact.LastName);
            model.EmailAddress.Should().Be(order.OrderingPartyContact.Email);
            model.TelephoneNumber.Should().Be(order.OrderingPartyContact.Phone);
        }

        [Theory]
        [MockAutoData]
        public static void WithNullOrderingPartyContact_PropertiesCorrectlySet(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order)
        {
            order.OrderingPartyContact = null;
            var model = new OrderingPartyModel(internalOrgId, order);

            model.FirstName.Should().BeNull();
            model.LastName.Should().BeNull();
            model.EmailAddress.Should().BeNull();
            model.TelephoneNumber.Should().BeNull();
        }
    }
}
