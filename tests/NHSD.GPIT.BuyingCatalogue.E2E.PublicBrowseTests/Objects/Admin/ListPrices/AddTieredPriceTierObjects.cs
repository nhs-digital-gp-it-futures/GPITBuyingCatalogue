using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.ListPrices
{
    internal static class AddTieredPriceTierObjects
    {
        internal static By PriceInput => By.Id("Price");

        internal static By LowerRangeInput => By.Id("LowerRange");

        internal static By UpperRangeInput => By.Id("UpperRange");

        internal static By RangeTypeInput => By.Id("is-infinite-range");

        internal static By PriceInputError => By.Id("Price-error");

        internal static By LowerRangeInputError => By.Id("LowerRange-error");

        internal static By UpperRangeInputError => By.Id("UpperRange-error");

        internal static By RangeTypeInputError => By.Id("is-infinite-range-error");
    }
}
