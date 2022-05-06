using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Admin
{
    public sealed class AddSolution : ActionBase
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

        public void CheckFrameworkByIndex(int index)
        {
            Driver.FindElements(Objects.Admin.AddSolutionObjects.SolutionFrameworks)[index].FindElement(By.TagName("input")).Click();
        }

        public void SelectSupplier(string value)
        {
            new SelectElement(Driver.FindElement(Objects.Admin.AddSolutionObjects.SupplierName)).SelectByValue(value);
        }

        public bool SaveSolutionButtonDisplayed()
        {
            return ElementDisplayed(Objects.Admin.AddSolutionObjects.SaveSolutionButton);
        }

        public void ClickSaveButton()
        {
            Driver.FindElement(Objects.Admin.AddSolutionObjects.SaveSolutionButton).Click();
        }

        public void EnterSolutionName(string name)
        {
            Driver.FindElement(Objects.Admin.AddSolutionObjects.SolutionName).SendKeys(name);
        }

        public bool ManageSuppliersLinkDisplayed()
        {
            return ElementDisplayed(Objects.Admin.AddSolutionObjects.ManageSuppliersOrgsLink);
        }

        public void ClickManageSuppliersOrgLink()
        {
            Driver.FindElement(Objects.Admin.AddSolutionObjects.ManageSuppliersOrgsLink).Click();
        }

        public bool AddSuppliersOrgLinkDisplayed()
        {
            return ElementDisplayed(Objects.Admin.AddSolutionObjects.AddSuppliersOrgLink);
        }

        public bool SuppliersOrgTableDisplayed()
        {
            return ElementDisplayed(Objects.Admin.AddSolutionObjects.SupplierOrgsTable);
        }

        public bool SuppliersEditLinkDisplayed()
        {
            return ElementDisplayed(Objects.Admin.AddSolutionObjects.SupplierEditLink);
        }

        public IEnumerable<string> GetSuppliersOrgsInfoFromTable()
        {
            return Driver.FindElement(Objects.Admin.AddSolutionObjects.SupplierOrgRow).FindElements(By.CssSelector("tbody tr")).Select(s => s.Text);
        }

        public int GetNumberOfSuppliersInTable()
        {
            return Driver.FindElement(Objects.Admin.AddSolutionObjects.SupplierOrgRow)
                    .FindElements(By.CssSelector("tbody tr")).Count;
        }

        public void ManageCatalogueSolution()
        {
            Driver.FindElement(Objects.Admin.AddSolutionObjects.CatalogueSolutionLink).Click();
        }

        public void ClickFilterCatalogueSolutionsButton()
        {
            Driver.FindElement(Objects.Admin.AddSolutionObjects.CatalogueSolutionFilter).Click();
        }

        public void ClickApplyFilterButton()
        {
            Driver.FindElement(Objects.Admin.AddSolutionObjects.SaveSolutionButton).Click();
        }

        public int NumberOfFilterRadioButtonsDisplayed()
        {
            ClickFilterCatalogueSolutionsButton();
            return Driver.FindElements(Objects.Admin.AddSolutionObjects.FilterRadioButton).Count;
        }

        public PublicationStatus SelectFilterRadioButton(int index = 0)
        {
            NumberOfFilterRadioButtonsDisplayed();
            var element = Driver.FindElements(Objects.Admin.AddSolutionObjects.FilterRadioButton)[index].FindElement(By.TagName("input"));
            element.Click();
            var value = element.GetAttribute("value");
            return Enum.Parse<PublicationStatus>(value);
        }

        public PublicationStatus FilterCatalogueSolutions(int index = 0)
        {
            var publicationStatus = SelectFilterRadioButton(index);
            ClickApplyFilterButton();
            return publicationStatus;
        }

        public bool AddSolutionLinkDisplayed()
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

        public int GetNumberOfItemsInTable()
        {
            return Driver.FindElement(Objects.Admin.AddSolutionObjects.CatalogueSolutionTable)
                    .FindElements(By.CssSelector("tbody tr")).Count;
        }

        public bool SolutionNameFieldDisplayed()
        {
            return ElementDisplayed(Objects.Admin.AddSolutionObjects.SolutionName);
        }

        public bool SupplierNameFieldDisplayed()
        {
            return ElementDisplayed(Objects.Admin.AddSolutionObjects.SupplierName);
        }

        public bool CatalogueSolutionTableDisplayed()
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
