using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.ListPrices
{
    internal static class EditTierPriceObjects
    {
        internal static By PriceInput => By.Id("Price");

        internal static By PriceInputError => By.Id("Price-error");
    }
}
