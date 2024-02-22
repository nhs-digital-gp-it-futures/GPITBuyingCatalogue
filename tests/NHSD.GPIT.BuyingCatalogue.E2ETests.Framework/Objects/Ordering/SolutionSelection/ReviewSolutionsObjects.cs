using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.SolutionSelection
{
    public static class ReviewSolutionsObjects
    {
        public static By AddedStickers => By.XPath("//*[contains(@class, 'nhsuk-tag') and text()='Added']");

        public static By SreenReaderExisting => By.XPath("//span[@aria-label='Existing']");

        public static By IndicativeCosts => By.Id("review-solutions-indicative-costs");

        public static By IndicativeCostsAmendment => By.Id("review-solutions-amended-indicative-costs");

        public static By CatalogueSolutionSectionTitle => By.Id("catalogue-solutions-title");

        public static By AdditionalServicesSectionTitle => By.Id("additional-services-title");

        public static By AssociatedServicesSectionTitle => By.Id("associated-services-title");

        public static By ContinueButton => By.LinkText("Continue");

        public static By ChangeCatalogueSolutionLink => By.LinkText("Change Catalogue Solution");

        public static By EditCatalogueItemServiceRecipientLink(string catalogueItemID) => By.Id($"ServiceRecipients_{catalogueItemID}");

        public static By EditCatalogueItemPriceLink(string catalogueItemID) => By.Id($"Price_{catalogueItemID}");

        public static By EditCatalogueItemQuantiyLink(string catalogueItemID) => By.Id($"Quantity_{catalogueItemID}");

        public static By ChangeAdditionalServiceLink => By.LinkText("Change Additional Services");

        public static By AddAdditionalServiceLink => By.XPath("//a[contains(@href, '/additional-services/add')]");

        public static By AddAssociatedServiceLink => By.LinkText("Add Associated Services");

        public static By ChangeAssociatedServiceLink => By.LinkText("Change Associated Services");
    }
}
