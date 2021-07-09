using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Admin
{
    internal sealed class AddSolution : ActionBase
    {
        public AddSolution(IWebDriver driver)
            : base(driver)
        {
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

        internal void ClickManageSuppliersOrgLink()
        {
            Driver.FindElement(Objects.Admin.AddSolutionObjects.ManageSuppliersOrgsLink).Click();
        }

        internal bool AddSuppliersOrgLinkDisplayed()
        {
            return ElementDisplayed(Objects.Admin.AddSolutionObjects.AddSuppliersOrgLink);
        }

        internal bool SuppliersOrgTableDisplayed()
        {
            return ElementDisplayed(Objects.Admin.AddSolutionObjects.SupplierOrgsTable);
        }

        internal bool SuppliersEditLinkDisplayed()
        {
            return ElementDisplayed(Objects.Admin.AddSolutionObjects.SupplierEditLink);
        }

        internal IEnumerable<string> GetSuppliersOrgsInfoFromTable()
        {
            return Driver.FindElement(Objects.Admin.AddSolutionObjects.SupplierOrgRow).FindElements(By.CssSelector("tbody tr")).Select(s => s.Text);
        }

        internal int GetNumberOfSuppliersInTable()
        {
            return Driver.FindElement(Objects.Admin.AddSolutionObjects.SupplierOrgRow)
                    .FindElements(By.CssSelector("tbody tr")).Count;
        }

        internal void ManageCatalogueSolution()
        {
            Driver.FindElement(Objects.Admin.AddSolutionObjects.CatalogueSolutionLink).Click();
        }

        internal void ClickFilterCatalogueSolutionsButton()
        {
            Driver.FindElement(Objects.Admin.AddSolutionObjects.CatalogueSolutionFilter).Click();
        }

        internal void ClickApplyFilterButton()
        {
            Driver.FindElement(Objects.Admin.AddSolutionObjects.SaveSolutionButton).Click();
        }

        internal int NumberOfFilterRadioButtonsDisplayed()
        {
            ClickFilterCatalogueSolutionsButton();
            return Driver.FindElements(Objects.Admin.AddSolutionObjects.FilterRadioButton).Count;
        }

        internal PublicationStatus SelectFilterRadioButton(int index = 0)
        {
            NumberOfFilterRadioButtonsDisplayed();
            var element = Driver.FindElements(Objects.Admin.AddSolutionObjects.FilterRadioButton)[index].FindElement(By.TagName("input"));
            element.Click();
            var id = element.GetAttribute("id");
            int value = int.Parse(id);
            return (PublicationStatus)value;
        }

        internal PublicationStatus FilterCatalogueSolutions(int index = 0)
        {
            var publicationStatus = SelectFilterRadioButton(index);
            ClickApplyFilterButton();
            return publicationStatus;
        }

        internal bool AddSolutionLinkDisplayed()
        {
            try
            {
                ElementDisplayed(Objects.Admin.AddSolutionObjects.AddSolutionLink);
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal int GetNumberOfItemsInTable()
        {
            return Driver.FindElement(Objects.Admin.AddSolutionObjects.CatalogueSolutionTable)
                    .FindElements(By.CssSelector("tbody tr")).Count;
        }

        internal bool SolutionNameFieldDisplayed()
        {
            return ElementDisplayed(Objects.Admin.AddSolutionObjects.SolutionName);
        }

        internal bool SupplierNameFieldDisplayed()
        {
            return ElementDisplayed(Objects.Admin.AddSolutionObjects.SupplierName);
        }

        internal bool CatalogueSolutionTableDisplayed()
        {
            try
            {
                Wait.Until(d => d.FindElements(Objects.Admin.AddSolutionObjects.CatalogueSolutionTable));
                return true;
            }
            catch
            {
                return false;
            }
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
