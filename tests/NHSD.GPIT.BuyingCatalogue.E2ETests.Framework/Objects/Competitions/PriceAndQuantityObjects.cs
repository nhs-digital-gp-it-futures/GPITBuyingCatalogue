using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions
{
    public static class PriceAndQuantityObjects
    {
        public static By SolutionPriceEditLink => By.LinkText("Edit");

        public static By EditCompetitionSolutionLink(string solutionId) => By.XPath($"//a[contains(@href, '/hub/"+solutionId+"')]");

        public static By EditCatalogueItemPriceIdLink(int cataloguesolutionservicepriceid) => By.XPath($"//a[contains(@href, 'select-price/{cataloguesolutionservicepriceid}/confirm')]");
    }
}
