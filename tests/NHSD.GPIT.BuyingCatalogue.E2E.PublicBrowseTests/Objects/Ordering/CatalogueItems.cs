using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Ordering
{
    internal static class CatalogueItems
    {
        internal static By AddedItemsTable => ByExtensions.DataTestId("added-order-items");

        internal static By ContinueButton => By.LinkText("Continue");

        internal static By AddItemButton(string itemType) => By.LinkText($"Add {itemType}");
    }
}
