using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions
{
    public static class CompetitionToOrderObjects
    {
        public static By CreateOrderFromeCompetitionWinningSolution(string winningSolutionId) => By.XPath($"//a[contains(@href, '/results/ordering-information?solutionId=" + winningSolutionId + "')]");
    }
}
