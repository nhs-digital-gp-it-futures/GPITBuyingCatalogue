using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.AssociatedServices
{
    public static class NoAssociatedServicesFoundModelTests
    {
        [Fact]
        public static void WithValidArguments_PropertiesCorrectlySet()
        {
            var model = new NoAssociatedServicesFoundModel();

            model.Title.Should().Be("No Associated Services found");
        }
    }
}
