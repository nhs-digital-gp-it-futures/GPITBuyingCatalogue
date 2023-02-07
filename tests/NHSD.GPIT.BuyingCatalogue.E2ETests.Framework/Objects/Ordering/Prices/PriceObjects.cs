using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Prices
{
    public static class PriceObjects
    {
        public static By AgreedPriceInput(int index) => By.Id($"Tiers_{index}__AgreedPrice");

        public static By AgreedPriceInputError(int index) => By.Id($"Tiers_{index}__AgreedPrice-error");

        public static By PriceTitle(int id) => ByExtensions.DataTestId($"price-title-{id}");

        public static By SelectPriceRadio => By.ClassName("nhsuk-radios");

        public static By SelectPriceError => By.Id("select-price-error");

        public static By ViewPriceDetails => By.Id("view-price-details");

        public static By ViewPriceTiers => By.Id("view-price-tiers");
    }
}
