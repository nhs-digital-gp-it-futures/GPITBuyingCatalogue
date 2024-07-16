using System.Linq;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.DeleteOrder;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.DeleteOrder
{
    public static class DeleteOrderModelTests
    {
        [Theory]
        [MockAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            EntityFramework.Ordering.Models.Order order)
        {
            var model = new DeleteOrderModel(order);

            model.CallOffId.Should().Be(order.CallOffId);
            model.IsAmendment.Should().Be(order.IsAmendment);
        }

        [Theory]
        [MockAutoData]
        public static void Amendment_PropertiesCorrectlySet(
            EntityFramework.Ordering.Models.Order order)
        {
            order.Revision = 2;

            var model = new DeleteOrderModel(order);

            model.Advice.Should().Be(DeleteOrderModel.AmendmentAdvice);
            model.OrderType.Should().Be(DeleteOrderModel.Amendment);

            var expected = new[]
            {
                DeleteOrderModel.AmendmentNoOptionText,
                DeleteOrderModel.AmendmentYesOptionText,
            };

            model.AvailableOptions.Count.Should().Be(expected.Length);
            expected.ForEach(x => model.AvailableOptions.FirstOrDefault(o => o.Text == x).Should().NotBeNull());
        }

        [Theory]
        [MockAutoData]
        public static void Order_PropertiesCorrectlySet(
            EntityFramework.Ordering.Models.Order order)
        {
            order.Revision = 1;

            var model = new DeleteOrderModel(order);

            model.Advice.Should().Be(DeleteOrderModel.OrderAdvice);
            model.OrderType.Should().Be(DeleteOrderModel.Order);

            var expected = new[]
            {
                DeleteOrderModel.OrderNoOptionText,
                DeleteOrderModel.OrderYesOptionText,
            };

            model.AvailableOptions.Count.Should().Be(expected.Length);
            expected.ForEach(x => model.AvailableOptions.FirstOrDefault(o => o.Text == x).Should().NotBeNull());
        }
    }
}
