using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.RandomData;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Marketing
{
    internal sealed class SolutionDescriptionActions : ActionBase
    {
        public SolutionDescriptionActions(IWebDriver driver) : base(driver)
        {
        }

        public string SummaryAddText(int numChars)
        {
            Driver.FindElement(SolutionDescriptionObjects.Summary).Clear();
            var summary = Strings.RandomString(numChars);
            Driver.FindElement(SolutionDescriptionObjects.Summary).SendKeys(summary);
            return summary;
        }

        internal string DescriptionAddText(int numChars)
        {
            Driver.FindElement(SolutionDescriptionObjects.Description).Clear();
            var description = Strings.RandomString(numChars);
            Driver.FindElement(SolutionDescriptionObjects.Description).SendKeys(description);
            return description;
        }

        internal string LinkAddText(int numChars)
        {
            Driver.FindElement(SolutionDescriptionObjects.Link).Clear();
            var link = Strings.RandomString(numChars);
            Driver.FindElement(SolutionDescriptionObjects.Link).SendKeys(link);
            return link;
        }

        internal bool ErrorMessageDisplayed()
        {
            try
            {
                Driver.FindElement(SolutionDescriptionObjects.ErrorSection);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
