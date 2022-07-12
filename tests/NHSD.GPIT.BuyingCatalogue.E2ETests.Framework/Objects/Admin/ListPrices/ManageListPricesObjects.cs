using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ListPrices
{
    public static class ManageListPricesObjects
    {
        public static By AddPriceLink => By.LinkText("Add a list price");

        public static By TieredPrices => ByExtensions.DataTestId("tiered-prices");

        public static By FlatPrices => ByExtensions.DataTestId("flat-prices");

        public static By TieredPrice(int cataloguePriceId) => ByExtensions.DataTestId($"tiered-price-{cataloguePriceId}");

        public static By FlatPrice(int cataloguePriceId) => ByExtensions.DataTestId($"flat-price-{cataloguePriceId}");

        public static By EditTieredPriceLink(int cataloguePriceId) => By.XPath($"//a[contains(@href, '/list-prices/tiered/{cataloguePriceId}/edit')]");

        public static By EditFlatPriceLink(int cataloguePriceId) => By.XPath($"//a[contains(@href, '/list-prices/flat/{cataloguePriceId}/edit')]");
    }
}
