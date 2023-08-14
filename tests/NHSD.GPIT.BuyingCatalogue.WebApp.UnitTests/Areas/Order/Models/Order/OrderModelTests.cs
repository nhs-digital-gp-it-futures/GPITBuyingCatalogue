using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Order
{
    public static class OrderModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string internalOrgId,
            AspNetUser aspNetUser,
            EntityFramework.Ordering.Models.Order order,
            OrderProgress progress)
        {
            order.LastUpdatedByUser = aspNetUser;

            var model = new OrderModel(internalOrgId, order, progress);

            model.Progress.Should().BeEquivalentTo(progress);
            model.Title.Should().Be($"Order {order.CallOffId}");
            model.CallOffId.Should().Be(order.CallOffId);
            model.Description.Should().Be(order.Description);
            model.LastUpdatedByUserName.Should().Be(aspNetUser.FullName);
            model.LastUpdated.Should().Be(order.LastUpdated);
        }

        [Theory]
        [CommonAutoData]
        public static void WithNoOrder_PropertiesCorrectlySet(
            string internalOrgId,
            OrderProgress progress)
        {
            var model = new OrderModel(internalOrgId, null, progress);

            model.Progress.Should().BeEquivalentTo(progress);
            model.Title.Should().Be("New order");
            model.CallOffId.Should().BeEquivalentTo(default(CallOffId));
            model.Description.Should().Be(default);
        }
    }
}
