using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.TestModels;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Admin
{
    internal sealed class Organisation : ActionBase
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

        internal void ClickAddRelatedOrgButton()
        {
            Driver.FindElement(Objects.Admin.OrganisationObjects.AddRelatedOrgButton).Click();
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

        internal RelatedOrg GetRelatedOrganisation(Guid orgId)
        {
            var relatedOrg = new RelatedOrg()
            {
                OrganisationId = orgId,
                OrganisationName = Driver.FindElement(Objects.Admin.OrganisationObjects.RelatedOrgTableOrgName(orgId)).Text,
                OdsCode = Driver.FindElement(Objects.Admin.OrganisationObjects.RelatedOrgTableOdsCode(orgId)).Text
            };

            return relatedOrg;
        }

        internal void RemoveRelatedOrganisation(Guid relatedOrgId)
        {
            Driver.FindElement(Objects.Admin.OrganisationObjects.RelatedOrganisationRemove(relatedOrgId)).Click();

            Driver.FindElement(Objects.Admin.OrganisationObjects.RelatedOrganisationRemoveConfirm).Click();
        }

        internal void ViewUserDetails(string id)
        {
            Driver.FindElement(Objects.Admin.OrganisationObjects.UserName(id)).Click();
        }
    }
}
