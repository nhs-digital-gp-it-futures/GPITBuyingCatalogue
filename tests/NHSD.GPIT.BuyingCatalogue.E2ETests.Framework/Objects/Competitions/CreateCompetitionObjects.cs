using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions
{
    public static class CreateCompetitionObjects
    {
        public static By CompetitionName => By.Id("Name");

        public static By CompetitionDescription => By.Id("Description");

        public static By CalculatePriceEditLink => By.LinkText("Edit");

        public static By PriceInput => By.Id("Price");

        public static By NonPriceInput => By.Id("NonPrice");
    }
}
