using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Actions
{
    internal sealed class CommonActions : ActionBase
    {
        public CommonActions(IWebDriver driver) : base(driver)
        {
        }

        internal void ClickElement(By by) => Driver.FindElement(by).Click();

        internal string GetElementChecked(By by) => Driver.FindElement(by).GetDomProperty("checked");

        internal string GetElementText(By by) => Driver.FindElement(by).Text;

        internal void SendTextToElement(By by, string text) => Driver.FindElement(by).SendKeys(text);

        internal string GetElementValue(By by) => Driver.FindElement(by).GetDomProperty("value");

        internal bool IsElementDisplayed(By by) => Driver.FindElement(by).Displayed;

        internal IEnumerable<IWebElement> GetElements(By by) => Driver.FindElements(by);

        internal void ClickCheckboxByLabel(string labelText)
        {
            var targetId =
                 Driver
                .FindElements(By.ClassName("nhsuk-checkboxes__label"))
                .FirstOrDefault(label => label.Text == labelText)
                .GetAttribute("for");

            Driver.FindElement(By.Id(targetId)).Click();
        }

        internal string GetSelectDropDownValue(By by)
        {
            var selectElement = new SelectElement(Driver.FindElement(by));
            return selectElement.SelectedOption.GetDomAttribute("value");
            
        }

        internal void SelectDropDownItemByText(By targetField, string text)
        {
            var selectElement = new SelectElement(Driver.FindElement(targetField));
            selectElement.SelectByText(text);
        }

    }
}
