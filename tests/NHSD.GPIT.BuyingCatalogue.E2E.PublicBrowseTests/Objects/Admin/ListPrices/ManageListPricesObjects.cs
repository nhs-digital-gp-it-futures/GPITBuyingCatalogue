using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.ListPrices
{
    internal static class ManageListPricesObjects
    {
        internal static By AddPriceLink => By.LinkText("Add a list price");

        internal static By TieredPrices => ByExtensions.DataTestId("tiered-prices");

        internal static By FlatPrices => ByExtensions.DataTestId("flat-prices");

        internal static By TieredPrice(int cataloguePriceId) => ByExtensions.DataTestId($"tiered-price-{cataloguePriceId}");

        internal static By FlatPrice(int cataloguePriceId) => ByExtensions.DataTestId($"flat-price-{cataloguePriceId}");

        internal static By EditTieredPriceLink(int cataloguePriceId) => By.XPath($"//a[contains(@href, '/list-prices/tiered/{cataloguePriceId}/edit')]");

        internal static By EditFlatPriceLink(int cataloguePriceId) => By.XPath($"//a[contains(@href, '/list-prices/flat/{cataloguePriceId}/edit')]");
    }
}
