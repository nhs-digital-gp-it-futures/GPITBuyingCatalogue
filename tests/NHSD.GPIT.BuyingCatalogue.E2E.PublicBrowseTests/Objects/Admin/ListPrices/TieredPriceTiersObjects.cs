using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.ListPrices
{
    internal static class TieredPriceTiersObjects
    {
        internal static By TiersTable => ByExtensions.DataTestId("tiered-price-table");

        internal static By PublicationStatusInput => By.Id("selected-publication-status");

        internal static By PublicationStatusInputError => By.Id("selected-publication-status-error");

        internal static By AddTierLink => By.LinkText("Add a pricing tier");
    }
}
