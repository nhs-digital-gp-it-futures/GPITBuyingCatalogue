using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
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

        public void ManageCatalougeSolution()
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
            ManageCatalougeSolution();
            ClickFilterCatalogueSolutionsButton();
            return Driver.FindElements(Objects.Admin.AddSolutionObjects.FilterRadioButton).Count;
        }

        public PublicationStatus SelectFilterRadioButton(int index = 0)
        {
            Wait.Until(d => NumberOfFilterRadioButtonsDisplayed() > 0);
            var element = Driver.FindElements(Objects.Admin.AddSolutionObjects.FilterRadioButton)[index].FindElement(By.TagName("input"));
            element.Click();
            var id = element.GetAttribute("id");
            int value = int.Parse(id);
            return (PublicationStatus)value;
        }

        public PublicationStatus FilterCatalogueSolutions(int index = 0)
        {
            var publicationStatus = SelectFilterRadioButton(index);
            ClickApplyFilterButton();
            return publicationStatus;
        }

        public bool AddSolutionLinkDisplayed()
        {
            ManageCatalougeSolution();
            try
            {
                Wait.Until(s => ElementDisplayed(Objects.Admin.AddSolutionObjects.AddSolutionLink));
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal bool CatalogueSolutionTableDisplayed()
        {
            ManageCatalougeSolution();
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

        public int GetNumberOfItemsInTable()
        {
            int numberOfItems = -1;
            try
            {
                numberOfItems = Driver.FindElement(Objects.Admin.AddSolutionObjects.CatalogueSolutionTable)
                   .FindElements(By.CssSelector("tbody tr")).Count();
            }
            catch (NoSuchElementException)
            {
                numberOfItems = 0;
            }
            catch
            {
                throw new WebDriverException("Something massively wrong");
            }
            return numberOfItems;
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
