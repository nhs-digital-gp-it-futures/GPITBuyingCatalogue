using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin.ListPrices
{
    public static class TieredPriceTiersObjects
    {
        public static By TiersTable => ByExtensions.DataTestId("tiered-price-table");

        public static By PublicationStatusInput => By.Id("selected-publication-status");

        public static By PublicationStatusInputError => By.Id("selected-publication-status-error");

        public static By AddTierLink => By.LinkText("Add a pricing tier");
    }
}
