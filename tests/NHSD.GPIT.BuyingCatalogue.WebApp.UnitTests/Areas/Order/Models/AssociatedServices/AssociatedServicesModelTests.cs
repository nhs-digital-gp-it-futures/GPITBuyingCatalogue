using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.AssociatedServices
{
    public static class AssociatedServicesModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string odsCode,
            EntityFramework.Ordering.Models.Order order,
            List<OrderItem> orderItems)
        {
            var model = new AssociatedServiceModel(odsCode, order, orderItems);

            model.Title.Should().Be($"Associated Services for {order.CallOffId}");
            model.OdsCode.Should().Be(odsCode);
            model.OrderDescription.Should().Be(order.Description);
            model.CallOffId.Should().Be(order.CallOffId);
            model.OrderItems.Should().BeEquivalentTo(orderItems);
        }
    }
}
