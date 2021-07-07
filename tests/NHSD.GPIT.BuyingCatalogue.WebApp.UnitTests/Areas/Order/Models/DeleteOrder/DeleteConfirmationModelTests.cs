using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.DeleteOrder;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.DeleteOrder
{
    public static class DeleteConfirmationModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string odsCode, 
            EntityFramework.Ordering.Models.Order order
            )
        {
            var model = new DeleteConfirmationModel(odsCode, order);

            model.BackLink.Should().Be($"/order/organisation/{odsCode}");
            model.BackLinkText.Should().Be("Go back to all orders");
            model.Title.Should().Be($"Order {order.CallOffId} deleted");
            model.OdsCode.Should().Be(odsCode);
            model.Description.Should().Be(order.Description);
        }
    }
}
