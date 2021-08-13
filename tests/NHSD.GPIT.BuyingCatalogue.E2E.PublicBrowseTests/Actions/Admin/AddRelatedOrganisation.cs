using System.Collections.Generic;
using System.Globalization;
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

        internal int SelectOrganisation(int? excludeOrg)
        {
            var orgs = (IEnumerable<IWebElement>)Driver.FindElements(Objects.Admin.AddRelatedOrgsObjects.OrganisationRadioButtons);

            if (excludeOrg is not null)
            {
                orgs = orgs.Where(s => s.GetAttribute("value") != excludeOrg.Value.ToString());
            }

            var selectedOrg = orgs.First();

            var orgId = int.Parse(selectedOrg.GetAttribute("value"), CultureInfo.InvariantCulture);

            selectedOrg.Click();

            return orgId;
        }

        internal void ClickAddRelatedOrgButton()
        {
            Driver.FindElement(Objects.Admin.AddRelatedOrgsObjects.SubmitButton).Click();
        }
    }
}
