using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.UIComponentTests.Actions
{
    internal class CheckboxesActions : ActionBase
    {
        public CheckboxesActions(IWebDriver driver)
            : base(driver)
        {
        }

        internal string GetElementChecked(By by) => Driver.FindElement(by).GetDomProperty("checked");
    }
}
