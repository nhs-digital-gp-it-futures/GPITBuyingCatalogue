using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Ordering.Prices
{
    public static class ConfirmPriceObjects
    {
        public static By AgreedPriceInput(int index) => By.Id($"Tiers_{index}__AgreedPrice");

        public static By AgreedPriceInputError(int index) => By.Id($"Tiers_{index}__AgreedPrice-error");
    }
}
