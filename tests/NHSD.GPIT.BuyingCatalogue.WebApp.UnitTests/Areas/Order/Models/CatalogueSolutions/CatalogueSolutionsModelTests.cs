using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.CatalogueSolutions
{
    public static class CatalogueSolutionsModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string odsCode,
            EntityFramework.Ordering.Models.Order order,
            List<OrderItem> orderItems)
        {
            var model = new CatalogueSolutionsModel(odsCode, order, orderItems);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}/order/{order.CallOffId}");
            model.Title.Should().Be($"Catalogue Solution for {order.CallOffId}");
            model.OdsCode.Should().Be(odsCode);
            model.OrderDescription.Should().Be(order.Description);
            model.CallOffId.Should().Be(order.CallOffId);
            model.OrderItems.Should().BeEquivalentTo(orderItems);
        }
    }
}
