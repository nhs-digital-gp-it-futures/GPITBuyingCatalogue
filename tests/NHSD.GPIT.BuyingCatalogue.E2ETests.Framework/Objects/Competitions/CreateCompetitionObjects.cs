using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions
{
    public static class CreateCompetitionObjects
    {
        public static By CompetitionName => By.Id("Name");

        public static By CompetitionDescription => By.Id("Description");
    }
}
