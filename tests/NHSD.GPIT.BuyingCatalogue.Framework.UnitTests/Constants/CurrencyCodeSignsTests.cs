using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Constants
{
    [TestFixture]
    internal sealed class CurrencyCodeSignsTests
    {
        [TestCase("GBP", "£")]
        [TestCase("USD", "$")]
        [TestCase("EUR", "€")]
        public void CurrencyCode_ReturnsCorrectSymbol(string currencyCode, string symbol)
        {
            Assert.AreEqual(symbol, CurrencyCodeSigns.Code[currencyCode]);
        }        
    }
}
