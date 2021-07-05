using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.AdditionalServices
{
    public static class AdditionalServicesModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string odsCode,
            EntityFramework.Ordering.Models.Order order,
            List<OrderItem> orderItems)
        {
            var model = new AdditionalServiceModel(odsCode, order, orderItems);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{order.CallOffId}");
            model.BackLinkText.Should().Be("Go back");
            model.Title.Should().Be($"Additional Services for {order.CallOffId}");
            model.OdsCode.Should().Be(odsCode);
            model.OrderDescription.Should().Be(order.Description);
            model.CallOffId.Should().Be(order.CallOffId);
            model.OrderItems.Should().BeEquivalentTo(orderItems);
        }
    }
}
