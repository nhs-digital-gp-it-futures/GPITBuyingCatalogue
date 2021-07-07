using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Admin
{
    internal sealed class AddSolution : ActionBase
    {
        public AddSolution(IWebDriver driver) : base(driver)
        {
        }

        internal bool SolutionNameFieldDisplayed()
        {
            return ElementDisplayed(Objects.Admin.AddSolutionObjects.SolutionName);
        }

        internal bool SupplierNameFieldDisplayed()
        {
            return ElementDisplayed(Objects.Admin.AddSolutionObjects.SupplierName);
        }

        public bool FrameworkNamesDisplayed()
        {
            try
            {
                Wait.Until(d => d.FindElements(Objects.Admin.AddSolutionObjects.SolutionFrameworks).Count == 2);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool FoundationSolutionDisplayed()
        {
            var checkbox = Driver.FindElements(Objects.Admin.AddSolutionObjects.SolutionFrameworks)[0];
            checkbox.Click();
            try
            {
                Wait.Until(s => ElementDisplayed(Objects.Admin.AddSolutionObjects.FoundationSolution));
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal bool SaveSolutionButtonDisplayed()
        {
            return ElementDisplayed(Objects.Admin.AddSolutionObjects.SaveSolutionButton);
        }

        internal bool ManageSuppliersLinkDisplayed()
        {
            return ElementDisplayed(Objects.Admin.AddSolutionObjects.ManageSuppliersOrgsLink);
        }

        public void ClickManageSuppliersOrgLink()
        {
            Driver.FindElement(Objects.Admin.AddSolutionObjects.ManageSuppliersOrgsLink).Click();
        }

        public bool AddSuppliersOrgLinkDisplayed()
        {
            Driver.FindElement(Objects.Admin.AddSolutionObjects.ManageSuppliersOrgsLink).Click();
            try
            {
                return ElementDisplayed(Objects.Admin.AddSolutionObjects.AddSuppliersOrgLink);
            }
            catch
            {
                return false;
            }
        }

        internal bool SuppliersOrgTableDisplayed()
        {
            ClickManageSuppliersOrgLink();
            return ElementDisplayed(Objects.Admin.AddSolutionObjects.SupplierOrgsTable);
        }

        internal bool SuppliersEditLinkDisplayed()
        {
            ClickManageSuppliersOrgLink();
            return ElementDisplayed(Objects.Admin.AddSolutionObjects.SupplierEditLink);
        }

        internal IEnumerable<string> GetSuppliersOrgsInfoFromTable()
        {
            SuppliersOrgTableDisplayed();
            return Driver.FindElement(Objects.Admin.AddSolutionObjects.SupplierOrgRow).FindElements(By.CssSelector("tbody tr")).Select(s => s.Text);
        }

        public int GetNumberOfSuppliersInTable()
        {
            ClickManageSuppliersOrgLink();

            return Driver.FindElement(Objects.Admin.AddSolutionObjects.SupplierOrgRow)
                   .FindElements(By.CssSelector("tbody tr")).Count();
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
