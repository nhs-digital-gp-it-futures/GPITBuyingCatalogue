using OpenQA.Selenium;
using System;

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
    }
}
