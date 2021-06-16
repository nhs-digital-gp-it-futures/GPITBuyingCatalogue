using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Admin
{
    internal sealed class Dashboard : ActionBase
    {
        public Dashboard(IWebDriver driver) : base(driver)
        {
        }

        internal bool AddOrgButtonDisplayed()
        {
            try
            {
                Driver.FindElement(Objects.Admin.DashboardObjects.AddOrgButton);
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal IEnumerable<string> GetOrgsOnPage()
        {
            return Driver.FindElements(Objects.Admin.DashboardObjects.OrganisationNameLinks).Select(s => s.Text);
        }
    }
}
