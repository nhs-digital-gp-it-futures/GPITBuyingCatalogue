using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Admin
{
    internal class Organisation : ActionBase
    {
        public Organisation(IWebDriver driver) : base(driver)
        {
        }

        internal IEnumerable<string> GetAddress()
        {
            return Driver.FindElements(Objects.Admin.OrganisationObjects.AddressLines).Select(s => s.Text);
        }

        internal string GetOdsCode()
        {
            return Driver.FindElement(Objects.Admin.OrganisationObjects.OdsCode).Text;
        }

        internal bool AddUserButtonDisplayed()
        {
            return ElementDisplayed(Objects.Admin.OrganisationObjects.AddUserButton);
        }

        internal bool UserTableDisplayed()
        {
            return ElementDisplayed(Objects.Admin.OrganisationObjects.UserTable);
        }

        internal bool AddRelatedOrganisationButtonDisplayed()
        {
            return ElementDisplayed(Objects.Admin.OrganisationObjects.AddRelatedOrgButton);
        }

        internal bool RelatedOrganisationTableDisplayed()
        {
            return ElementDisplayed(Objects.Admin.OrganisationObjects.RelatedOrgsTable);
        }

        internal void ClickAddUserButton()
        {
            Driver.FindElement(Objects.Admin.OrganisationObjects.AddUserButton).Click();
        }

        private bool ElementDisplayed(By by)
        {
            try
            {
                Driver.FindElement(by);
                return true;
            }
            catch 
            {
                return false;
            }
        }
    }
}
