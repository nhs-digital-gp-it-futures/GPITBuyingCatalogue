using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.SolutionSelection
{
    public static class ReviewSolutionsObjects
    {
        public static By CatalogueSolutionSectionTitle => By.Id("catalogue-solutions-title");

        public static By AdditionalServicesSectionTitle => By.Id("additional-services-title");

        public static By AssociatedServicesSectionTitle => By.Id("associated-services-title");

        public static By ContinueButton => By.LinkText("Continue");

        public static By EditSolutionAndServicesLink => By.LinkText("Edit solution and services");

        public static By EditAssociatedServicesLink => By.LinkText("Edit Associated Services");

        public static By ChangeCatalogueSolutionLink => By.LinkText("Change Catalogue Solution");

        public static By EditCatalogueItemServiceRecipientLink(string catalogueItemID) => By.Id($"ServiceRecipients_{catalogueItemID}");

        public static By EditCatalogueItemPriceLink(string catalogueItemID) => By.Id($"Price_{catalogueItemID}");

        public static By EditCatalogueItemQuantiyLink(string catalogueItemID) => By.Id($"Quantity_{catalogueItemID}");

        public static By ChangeAdditionalServiceLink => By.LinkText("Change Additional Services");

        public static By AddAdditionalServiceLink => By.LinkText("Add Additional Services");

        public static By AddAssociatedServiceLink => By.LinkText("Add Associated Services");

        public static By ChangeAssociatedServiceLink => By.LinkText("Change Associated Services");
    }
}
