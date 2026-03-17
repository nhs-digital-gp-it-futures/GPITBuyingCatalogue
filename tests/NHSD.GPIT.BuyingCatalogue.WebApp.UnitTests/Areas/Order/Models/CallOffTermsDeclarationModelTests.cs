using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models;

public static class CallOffTermsDeclarationModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        EntityFramework.Ordering.Models.Order order)
    {
        var model = new CallOffTermsDeclarationModel(order);

        model.Title.Should().Be("Declaration");
        model.Caption.Should().Be($"Order {order.CallOffId}");
        model.CallOffId.Should().Be(order.CallOffId);
        model.DeclarationAccepted.Should().Be(order.AcceptedTermsAndConditions);
    }
}
