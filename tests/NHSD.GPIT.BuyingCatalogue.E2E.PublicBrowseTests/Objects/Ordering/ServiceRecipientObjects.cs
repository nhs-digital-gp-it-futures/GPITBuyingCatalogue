using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Ordering
{
    public static class ServiceRecipientObjects
    {
        public static By SelectedRecipientErrorMessage => By.ClassName("nhsuk-error-message");

        public static By PreSelectedInset => By.Id("pre-selected");

        public static By SelectAllLink => By.LinkText("Select all");

        public static By SelectNoneLink => By.LinkText("Select none");
    }
}
