using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.AdditionalServices
{
    public static class NoAdditionalServicesFoundModelTests
    {
        [Fact]
        public static void WithValidArguments_PropertiesCorrectlySet()
        {
            var model = new NoAdditionalServicesFoundModel();

            model.Title.Should().Be("No Additional Services found");
        }
    }
}
