using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.Order
{
    public static class SummaryModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            string odsCode, 
            EntityFramework.Ordering.Models.Order order
        )
        {
            var model = new SummaryModel(odsCode, order);

            model.BackLink.Should().Be("./");
            model.BackLinkText.Should().Be("Go back");
            model.OdsCode.Should().Be(odsCode);
            model.Order.Should().BeEquivalentTo(order);
        }
    }
}
