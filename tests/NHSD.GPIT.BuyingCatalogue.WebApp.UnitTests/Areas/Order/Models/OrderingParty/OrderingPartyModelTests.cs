using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
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
            model.Contact.FirstName.Should().Be(order.OrderingPartyContact.FirstName);
            model.Contact.LastName.Should().Be(order.OrderingPartyContact.LastName);
            model.Contact.EmailAddress.Should().Be(order.OrderingPartyContact.Email);
            model.Contact.TelephoneNumber.Should().Be(order.OrderingPartyContact.Phone);
        }
    }
}
