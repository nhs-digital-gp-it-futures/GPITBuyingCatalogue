using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.AdditionalServices
{
    public static class SelectFlatDeclarativeQuantityModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(
            CallOffId callOffId,
            string solutionName,
            int? quantity)
        {
            var model = new SelectFlatDeclarativeQuantityModel(callOffId, solutionName, quantity);

            model.Title.Should().Be($"Quantity of {solutionName} for {callOffId}");
            model.Quantity.Should().Be(quantity.ToString());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("ABC")]
        public static void GetQuantity_QuantityMustBeANumber(string quantity)
        {
            var model = new SelectFlatDeclarativeQuantityModel { Quantity = quantity };

            (_, string error) = model.GetQuantity();

            error.Should().Be("Quantity must be a number");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public static void GetQuantity_QuantityMustBeGreaterThanZero(int quantity)
        {
            var model = new SelectFlatDeclarativeQuantityModel { Quantity = quantity.ToString() };

            (_, string error) = model.GetQuantity();

            error.Should().Be("Quantity must be greater than zero");
        }

        [Fact]
        public static void GetQuantity_ValidQuantityReturnedWithoutError()
        {
            var model = new SelectFlatDeclarativeQuantityModel { Quantity = "123" };

            (int? quantity, string error) = model.GetQuantity();

            Assert.Null(error);
            Assert.NotNull(quantity);
            quantity.Value.Should().Be(123);
        }
    }
}
