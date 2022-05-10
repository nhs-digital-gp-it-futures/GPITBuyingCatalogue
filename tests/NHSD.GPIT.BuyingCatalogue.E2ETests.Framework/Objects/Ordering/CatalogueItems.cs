using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering
{
    public static class CatalogueItems
    {
        public static By AddedItemsTable => ByExtensions.DataTestId("added-order-items");

        public static By ContinueButton => By.LinkText("Continue");

        public static By AddCatalogueSolution => By.LinkText("Add a Catalogue Solution");

        public static By AddAdditionalService => By.LinkText("Add an Additional Service");

        public static By AddAssociatedService => By.LinkText("Add an Associated Service");
    }
}
