using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Ordering.Prices
{
   public static class SelectPriceObjects
    {
        public static By SelectPriceRadio => By.ClassName("nhsuk-radios");

        public static By SelectPriceError => By.Id("select-price-error");

        public static By PriceTitle(int id) => ByExtensions.DataTestId($"price-title-{id}");
    }
}
