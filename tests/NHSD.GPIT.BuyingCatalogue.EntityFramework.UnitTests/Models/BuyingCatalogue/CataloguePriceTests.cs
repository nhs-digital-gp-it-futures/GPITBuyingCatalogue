using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.BuyingCatalogue
{
    public static class CataloguePriceTests
    {
        [Fact]
        public static void GetCurrencySymbol_CataloguePrice_WithInvalidCurrencyCode_ReturnsNull()
        {
            const string invalidCurrencyCode = "ABC";

            var cataloguePrice = new CataloguePrice
            {
                CurrencyCode = invalidCurrencyCode,
            };

            var currencySymbol = cataloguePrice.GetCurrencySymbol();

            currencySymbol.Should().BeNull();
        }

        [InlineData("GBP", "£")]
        [InlineData("USD", "$")]
        [Theory]
        public static void GetCurrencySymbol_CataloguePrice_WithCurrencyCode_ReturnsExpectedCurrencySymbol(
            string currencyCode,
            string expectedCurrencySymbol)
        {
            var cataloguePrice = new CataloguePrice
            {
                CurrencyCode = currencyCode,
            };

            var actualCurrencySymbol = cataloguePrice.GetCurrencySymbol();

            actualCurrencySymbol.Should().BeEquivalentTo(expectedCurrencySymbol);
        }
    }
}
