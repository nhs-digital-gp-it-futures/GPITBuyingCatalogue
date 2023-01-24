using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse
{
    public static class ListPriceDetailsObjects
    {
        public static By ListPriceTable => ByExtensions.DataTestId("flat-list-price-table");
        public static By TieredPriceTable => ByExtensions.DataTestId("tiered-list-price-table");

    }
}
