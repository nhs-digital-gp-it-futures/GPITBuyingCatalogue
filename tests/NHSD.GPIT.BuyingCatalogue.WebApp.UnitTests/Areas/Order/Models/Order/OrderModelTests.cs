using FluentAssertions;
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

            // TODO: Title, CallOffId, TitleAdvice, Description
        }
    }
}
