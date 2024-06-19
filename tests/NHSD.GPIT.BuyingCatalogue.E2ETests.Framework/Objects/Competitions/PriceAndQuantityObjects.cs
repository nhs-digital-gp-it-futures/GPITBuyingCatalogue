using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions
{
    public static class PriceAndQuantityObjects
    {
        public static By SolutionPriceEditLink => By.LinkText("Edit");

        public static By EditCompetitionSolutionLink(string catalogueSolutionId) => By.XPath($"//a[contains(@href, '/hub/{catalogueSolutionId}')]");

        public static By EditCatalogueItemPriceIdLink(int catalogueSolutionServicePriceId) => By.XPath($"//a[contains(@href, 'select-price/{catalogueSolutionServicePriceId}/confirm')]");

        public static By EditCatalogueItemQuantityLink(string catalogueSolutionId) => By.XPath($"//a[contains(@href, '/hub/{catalogueSolutionId}/select-quantity')]");

        public static By EditAdditionalServiceEditPriceLink(int catalogueServicePriceId) => By.XPath($"//a[contains(@href, 'select-price/{catalogueServicePriceId}')]");

        public static By EditAdditionalServiceWithTieredPriceEditLink(string additionalServiceId) => By.XPath($"//a[contains(@href, 'select-price?serviceId={additionalServiceId}')]");

        public static By EditAdditionalServiceQuantityLink(string additionalServiceId) => By.XPath($"//a[contains(@href, 'select-quantity?serviceId={additionalServiceId}')]");
    }
}
