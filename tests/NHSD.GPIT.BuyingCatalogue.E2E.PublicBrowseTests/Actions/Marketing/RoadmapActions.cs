using NHSD.GPIT.BuyingCatalogue.E2ETests.Common.Actions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.RandomData;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Marketing
{
    internal sealed class RoadmapActions : ActionBase
    {
        public RoadmapActions(IWebDriver driver) : base(driver)
        {
        }

        internal string EnterSummary(int charCount)
        {
            var summary = Strings.RandomString(charCount);

            Driver.FindElement(RoadmapObjects.Summary).SendKeys(summary);

            return summary;
        }
    }
}
