using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Utils.Extensions;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Marketing
{
    public sealed class DashboardActions : ActionBase
    {
        public DashboardActions(IWebDriver driver)
            : base(driver)
        {
        }

        public bool SectionDisplayed(string section)
        {
            try
            {
                Driver.FindElements(DashboardObjects.SectionTitle)
                .First(s => s.Text.Contains(section)).FindElement(By.TagName("a"));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void ClickSection(string section)
        {
            Driver.FindElements(DashboardObjects.SectionTitle)
                .First(s => s.Text.Contains(section)).FindElement(By.TagName("a"))
                .Click();
        }

        public bool SectionMarkedComplete(string sectionName)
        {
            var section = Driver.FindElements(DashboardObjects.Sections)
                .Where(s => s.ContainsElement(DashboardObjects.SectionTitle))
                .First(s => s.FindElement(DashboardObjects.SectionTitle).Text == sectionName);
            return section.FindElement(DashboardObjects.SectionStatus).Text.Equals("Complete", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
