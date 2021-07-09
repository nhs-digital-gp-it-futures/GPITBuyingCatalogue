using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Admin
{
    internal sealed class AddRelatedOrganisation : ActionBase
    {
        public AddRelatedOrganisation(IWebDriver driver)
            : base(driver)
        {
        }

        internal Guid SelectOrganisation(string excludeOrg = null)
        {
            var orgs = (IEnumerable<IWebElement>)Driver.FindElements(Objects.Admin.AddRelatedOrgsObjects.OrganisationRadioButtons);

            if (!string.IsNullOrEmpty(excludeOrg))
            {
                orgs = orgs.Where(s => s.GetAttribute("value") != excludeOrg);
            }

            var selectedOrg = orgs.First();

            var orgId = Guid.Parse(selectedOrg.GetAttribute("value"));

            selectedOrg.Click();

            return orgId;
        }

        internal void ClickAddRelatedOrgButton()
        {
            Driver.FindElement(Objects.Admin.AddRelatedOrgsObjects.SubmitButton).Click();
        }
    }
}
