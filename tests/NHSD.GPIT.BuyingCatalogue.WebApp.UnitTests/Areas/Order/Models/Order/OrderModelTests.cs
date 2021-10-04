using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.TaskList;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Order
{
    public static class OrderModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string odsCode,
            EntityFramework.Ordering.Models.Order order,
            OrderTaskList orderSections)
        {
            var model = new OrderModel(odsCode, order, orderSections);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}");
            model.BackLinkText.Should().Be("Go back to all orders");
            model.SectionStatuses.Should().BeEquivalentTo(orderSections);
            model.Title.Should().Be($"Order {order.CallOffId}");
            model.CallOffId.Should().Be(order.CallOffId);
            model.TitleAdvice.Should().Be("Complete the following steps to create an order");
            model.Description.Should().Be(order.Description);
        }

        [Theory]
        [CommonAutoData]
        public static void WithNoOrder_PropertiesCorrectlySet(
            string odsCode,
            OrderTaskList orderSections)
        {
            var model = new OrderModel(odsCode, null, orderSections);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}");
            model.BackLinkText.Should().Be("Go back to all orders");
            model.SectionStatuses.Should().BeEquivalentTo(orderSections);
            model.Title.Should().Be("New order");
            model.CallOffId.Should().BeEquivalentTo(default(CallOffId));
            model.TitleAdvice.Should().Be("Step 1 must be completed before a summary page and ID number are created for this order.");
            model.Description.Should().Be(default);
        }
    }
}
