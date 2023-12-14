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
        public static void InProgressOrder_WithValidArguments_PropertiesCorrectlySet(
            string internalOrgId,
            AspNetUser aspNetUser,
            EntityFramework.Ordering.Models.Order order,
            OrderProgress progress)
        {
            order.LastUpdatedByUser = aspNetUser;

            var model = new OrderModel(internalOrgId, progress, order);

            model.Progress.Should().BeEquivalentTo(progress);
            model.Title.Should().Be($"Order {order.CallOffId}");
            model.OrganisationName.Should().Be(order.OrderingParty.Name);
            model.CallOffId.Should().Be(order.CallOffId);
            model.Description.Should().Be(order.Description);
            model.LastUpdatedByUserName.Should().Be(aspNetUser.FullName);
            model.LastUpdated.Should().Be(order.LastUpdated);
        }

        [Theory]
        [CommonAutoData]
        public static void NewOrder_WithValidArguments_PropertiesCorrectlySet(
            string internalOrgId,
            OrderTypeEnum orderType,
            OrderProgress progress,
            string organisationName)
        {
            var model = new OrderModel(internalOrgId, orderType, progress, organisationName);

            model.Progress.Should().BeEquivalentTo(progress);
            model.Title.Should().Be("New order");
            model.OrganisationName.Should().Be(organisationName);
            model.CallOffId.Should().BeEquivalentTo(default(CallOffId));
            model.Description.Should().Be(default);
        }
    }
}
