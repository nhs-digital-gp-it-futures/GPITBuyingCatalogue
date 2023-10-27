using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions
{
    public static class PriceAndQuantityObjects
    {
        public static By SolutionPriceEditLink => By.LinkText("Edit");

        public static By EditCompetitionSolutionLink(string solutionId) => By.XPath($"//a[contains(@href, '/hub/"+solutionId+"')]");

        public static By EditCatalogueItemPriceIdLink(int cataloguesolutionservicepriceid) => By.XPath($"//a[contains(@href, 'select-price/{cataloguesolutionservicepriceid}/confirm')]");

        public static By EditCatalogueItemQuantityLink(string solutionId) => By.XPath($"//a[contains(@href, '/hub/"+solutionId+"/select-quantity')]");

        public static By EditAdditionalServiceEditPriceLink(int catalogueservicepriceid) => By.XPath($"//a[contains(@href, 'select-price/{catalogueservicepriceid}')]");

        public static By EditAdditionalServiceQuantityLink(string additionalserviceid) => By.XPath($"//a[contains(@href, 'select-quantity?serviceId="+ additionalserviceid + "')]");
    }
}
