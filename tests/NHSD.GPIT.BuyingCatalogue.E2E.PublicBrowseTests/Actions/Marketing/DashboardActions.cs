using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.Extensions;
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

        internal bool SectionMarkedComplete(string sectionName)
        {
            var section = Driver.FindElements(DashboardObjects.Sections)
                .Where(s => s.ContainsElement(DashboardObjects.SectionTitle))
                .Single(s => s.FindElement(DashboardObjects.SectionTitle).Text == sectionName);
            return section.FindElement(DashboardObjects.SectionStatus).Text.Equals("Complete", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
