using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Admin
{
    internal sealed class AddSolution : ActionBase
    {
        public AddSolution(IWebDriver driver)
            : base(driver)
        {
        }

        internal bool FrameworkNamesDisplayed()
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

        // TODO : Fix so that this doesn't rely on Framework loading order (SUPER FRAGILE)
        internal bool FoundationSolutionDisplayed()
        {
            var checkbox = Driver.FindElements(Objects.Admin.AddSolutionObjects.SolutionFrameworks)[1];
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

        internal void CheckFrameworkByIndex(int index)
        {
            Driver.FindElements(Objects.Admin.AddSolutionObjects.SolutionFrameworks)[index].FindElement(By.TagName("input")).Click();
        }

        internal void SelectSupplier(string value)
        {
            new SelectElement(Driver.FindElement(Objects.Admin.AddSolutionObjects.SupplierName)).SelectByValue(value);
        }

        internal bool SaveSolutionButtonDisplayed()
        {
            return ElementDisplayed(Objects.Admin.AddSolutionObjects.SaveSolutionButton);
        }

        internal void ClickSaveButton()
        {
            Driver.FindElement(Objects.Admin.AddSolutionObjects.SaveSolutionButton).Click();
        }

        internal void EnterSolutionName(string name)
        {
            Driver.FindElement(Objects.Admin.AddSolutionObjects.SolutionName).SendKeys(name);
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
            var value = element.GetAttribute("value");
            return Enum.Parse<PublicationStatus>(value);
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
