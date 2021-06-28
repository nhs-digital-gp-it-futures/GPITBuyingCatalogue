using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Constants
{
    public static class CurrencyCodeSignsTests
    {
        [Theory]
        [InlineData("GBP", "£")]
        [InlineData("USD", "$")]
        [InlineData("EUR", "€")]
        public static void CurrencyCode_ReturnsCorrectSymbol(string currencyCode, string expectedSymbol)
        {
            var actualSymbol = CurrencyCodeSigns.Code[currencyCode];

            actualSymbol.Should().Be(expectedSymbol);
        }
    }
}
