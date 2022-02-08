using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
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
            AspNetUser aspNetUser,
            EntityFramework.Ordering.Models.Order order,
            OrderTaskList orderSections)
        {
            order.LastUpdatedByUser = aspNetUser;

            var model = new OrderModel(odsCode, order, orderSections);

            model.SectionStatuses.Should().BeEquivalentTo(orderSections);
            model.Title.Should().Be($"Order {order.CallOffId}");
            model.CallOffId.Should().Be(order.CallOffId);
            model.Description.Should().Be(order.Description);
            model.LastUpdatedByUserName.Should().Be(aspNetUser.FullName);
            model.LastUpdated.Should().Be(order.LastUpdated);
        }

        [Theory]
        [CommonAutoData]
        public static void WithNoOrder_PropertiesCorrectlySet(
            string odsCode,
            OrderTaskList orderSections)
        {
            var model = new OrderModel(odsCode, null, orderSections);

            model.SectionStatuses.Should().BeEquivalentTo(orderSections);
            model.Title.Should().Be("New order");
            model.CallOffId.Should().BeEquivalentTo(default(CallOffId));
            model.Description.Should().Be(default);
        }
    }
}
