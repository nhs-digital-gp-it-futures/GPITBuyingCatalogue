using System.Linq;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common
{
    internal sealed class CommonActions : ActionBase
    {
        public CommonActions(IWebDriver driver)
            : base(driver)
        {
        }

        // Click Actions
        public void ClickGoBackLink()
        {
            Driver.FindElement(CommonSelectors.GoBackLink).Click();
        }

        public bool GoBackLinkDisplayed()
        {
            try
            {
                Wait.Until(d => d.FindElement(CommonSelectors.GoBackLink));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void ClickSave()
        {
            Driver.FindElement(CommonSelectors.SaveAndReturn).Click();
        }

        public bool SaveButtonDisplayed()
        {
            try
            {
                Wait.Until(d => d.FindElement(CommonSelectors.SaveAndReturn));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void ClickFirstCheckbox() => Driver.FindElements(By.CssSelector("input[type=checkbox]")).First().Click();

        public string ClickCheckbox(By targetField, int index = 0)
        {
            var checkboxItems = Driver.FindElements(targetField);

            var selected = checkboxItems[index];

            selected.FindElement(By.TagName("input")).Click();
            return selected.FindElement(By.TagName("label")).Text;
        }

        public void ClickSection(By targetField, string section)
        {
            Driver.FindElements(targetField)
                .Single(s => s.Text.Contains(section)).FindElement(By.TagName("a"))
                .Click();
        }

        public void SelectDropdownItem(By targetField, int index = 0)
        {
            var select = Driver.FindElement(targetField);
            new SelectElement(select).SelectByIndex(index);
        }

        public void ClickRadioButtonWithText(string label)
        {
            var radioButtonItems = Driver.FindElements(CommonSelectors.RadioButtonItems);

            radioButtonItems.Single(r => r.FindElement(By.TagName("label")).Text == label).FindElement(By.TagName("input")).Click();
        }

        // testing
        public bool ErrorSummaryDisplayed()
        {
            try
            {
                Driver.FindElement(CommonSelectors.NhsErrorSection);
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal string PageTitle()
        {
            return Driver.FindElement(CommonSelectors.Header1).Text;
        }
    }
}
