using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ListPrices
{
    public static class AddTieredPriceTierObjects
    {
        public static By PriceInput => By.Id("InputPrice");

        public static By LowerRangeInput => By.Id("LowerRange");

        public static By UpperRangeInput => By.Id("UpperRange");

        public static By RangeTypeInput => By.Id("is-infinite-range");

        public static By PriceInputError => By.Id("InputPrice-error");

        public static By LowerRangeInputError => By.Id("LowerRange-error");

        public static By UpperRangeInputError => By.Id("UpperRange-error");

        public static By RangeTypeInputError => By.Id("is-infinite-range-error");
    }
}
