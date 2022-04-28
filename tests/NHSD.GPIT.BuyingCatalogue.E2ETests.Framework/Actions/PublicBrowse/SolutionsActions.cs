using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.PublicBrowse
{
    public sealed class SolutionsActions : ActionBase
    {
        public SolutionsActions(IWebDriver driver)
            : base(driver)
        {
        }

        public static bool SolutionListCardDisplaysAllRequiredSections(IWebElement solution)
        {
            try
            {
                solution.FindElement(Objects.PublicBrowse.SolutionsObjects.SolutionName);
                solution.FindElement(Objects.PublicBrowse.SolutionsObjects.SolutionSummary);
                solution.FindElement(Objects.PublicBrowse.SolutionsObjects.SolutionCapabilityList);
                solution.FindElement(Objects.PublicBrowse.SolutionsObjects.SolutionSupplierName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void ClickSolutionName(IWebElement selectedSolution)
        {
            selectedSolution.FindElement(ByExtensions.DataTestId("solution-card-view-link")).FindElement(By.TagName("a")).Click();
        }

        public bool ListOfSolutionsDisplayed()
        {
            try
            {
                Driver.FindElement(Objects.PublicBrowse.SolutionsObjects.SolutionList);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<IWebElement> GetSolutionsInList()
        {
            return Driver.FindElements(Objects.PublicBrowse.SolutionsObjects.SolutionsInList);
        }
    }
}
