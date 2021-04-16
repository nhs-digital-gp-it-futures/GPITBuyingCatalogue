using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.E2E.PublicBrowseTests.Actions
{
    internal sealed class SolutionsActions : ActionBase
    {
        public SolutionsActions(IWebDriver driver) : base(driver)
        {
        }

        internal bool ListOfSolutionsDisplayed()
        {
            try
            {
                Driver.FindElement(Objects.SolutionsObjects.SolutionList);
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal IEnumerable<IWebElement> GetSolutionsInList()
        {
            return Driver.FindElements(Objects.SolutionsObjects.SolutionsInList);
        }

        internal static bool SolutionListCardDisplaysAllRequiredSections(IWebElement solution)
        {
            try
            {
                solution.FindElement(Objects.SolutionsObjects.SolutionName);
                solution.FindElement(Objects.SolutionsObjects.SolutionSummary);
                solution.FindElement(Objects.SolutionsObjects.SolutionCapabilityList);
                solution.FindElement(Objects.SolutionsObjects.SolutionSupplierName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal static void ClickSolution(IWebElement selectedSolution)
        {
            selectedSolution.Click();
        }
    }
}
