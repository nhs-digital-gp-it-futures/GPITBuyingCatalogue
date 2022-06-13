using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering
{
    public static class ConfirmServiceChangesObjects
    {
        public static By RemovedItems => By.Id("removed-items");

        public static By AddedItems => By.Id("added-items");

        public static By ConfirmChangesError => By.Id("confirm-service-changes-error");
    }
}
