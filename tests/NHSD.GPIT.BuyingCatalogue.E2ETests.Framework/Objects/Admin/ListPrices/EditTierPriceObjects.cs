using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ListPrices
{
    public static class EditTierPriceObjects
    {
        public static By PriceInput => By.Id("InputPrice");

        public static By PriceInputError => By.Id("InputPrice-error");
    }
}
