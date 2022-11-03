using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Utils.TestModels;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Admin
{
    public sealed class Organisation : ActionBase
    {
        public Organisation(IWebDriver driver)
            : base(driver)
        {
        }

        public bool AddUserButtonDisplayed()
        {
            return ElementDisplayed(Objects.Admin.OrganisationObjects.AddUserButton);
        }

        public bool UserTableDisplayed()
        {
            return ElementDisplayed(Objects.Admin.OrganisationObjects.UserTable);
        }

        public bool AddRelatedOrganisationButtonDisplayed()
        {
            return ElementDisplayed(Objects.Admin.OrganisationObjects.AddRelatedOrgButton);
        }

        public bool RelatedOrganisationTableDisplayed()
        {
            return ElementDisplayed(Objects.Admin.OrganisationObjects.RelatedOrgsTable);
        }

        public void ClickAddUserButton()
        {
            Driver.FindElement(Objects.Admin.OrganisationObjects.AddUserButton).Click();
        }

        public void ClickAddRelatedOrgButton()
        {
            Driver.FindElement(Objects.Admin.OrganisationObjects.AddRelatedOrgButton).Click();
        }

        public RelatedOrg GetRelatedOrganisation(int orgId)
        {
            var relatedOrg = new RelatedOrg
            {
                OrganisationId = orgId,
                OrganisationName = Driver.FindElement(Objects.Admin.OrganisationObjects.RelatedOrgTableOrgName(orgId)).Text,
                OdsCode = Driver.FindElement(Objects.Admin.OrganisationObjects.RelatedOrgTableOdsCode(orgId)).Text,
            };

            return relatedOrg;
        }

        public void RemoveRelatedOrganisation(int relatedOrgId)
        {
            Driver.FindElement(Objects.Admin.OrganisationObjects.RelatedOrganisationRemove(relatedOrgId)).Click();

            Driver.FindElement(Objects.Admin.OrganisationObjects.RelatedOrganisationRemoveConfirm).Click();
        }

        public void ViewUserDetails(int id)
        {
            Driver.FindElement(Objects.Admin.OrganisationObjects.UserName(id)).Click();
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
