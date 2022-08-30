using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models
{
    public static class DeleteOrderModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            EntityFramework.Ordering.Models.Order order)
        {
            var model = new DeleteOrderModel(order);

            model.CallOffId.Should().Be(order.CallOffId);
        }
    }
}
