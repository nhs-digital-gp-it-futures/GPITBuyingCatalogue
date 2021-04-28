using NHSD.GPIT.BuyingCatalogue.E2ETests.Common.Actions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using OpenQA.Selenium;
using System;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Marketing
{
    internal sealed class DashboardActions : ActionBase
    {
        public DashboardActions(IWebDriver driver) : base(driver)
        {
        }

        internal bool SectionDisplayed(string section)
        {
            try
            {
                Driver.FindElements(DashboardObjects.SectionTitle)
                .Single(s => s.Text.Contains(section)).FindElement(By.TagName("a"));
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal void ClickSection(string section)
        {
            Driver.FindElements(DashboardObjects.SectionTitle)
                .Single(s => s.Text.Contains(section)).FindElement(By.TagName("a"))
                .Click();
        }
    }
}
