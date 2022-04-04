using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.OrderDescription;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.OrderDescription
{
    public static class OrderDescriptionModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string internalOrgId,
            EntityFramework.Ordering.Models.Order order)
        {
            var model = new OrderDescriptionModel(internalOrgId, order);

            model.Title.Should().Be("Order description");
            model.Description.Should().Be(order.Description);
        }
    }
}
