using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Ordering
{
    internal static class CatalogueItems
    {
        internal static By AddedItemsTable => ByExtensions.DataTestId("added-order-items");

        internal static By ContinueButton => By.LinkText("Continue");

        internal static By AddCatalogueSolution => By.LinkText("Add a Catalogue Solution");

        internal static By AddAdditionalService => By.LinkText("Add an Additional Service");

        internal static By AddAssociatedService => By.LinkText("Add an Associated Service");
    }
}
