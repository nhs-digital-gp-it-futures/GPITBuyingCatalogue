using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using OpenQA.Selenium;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Marketing
{
    internal sealed class CommonActions : ActionBase
    {
        public CommonActions(IWebDriver driver) : base(driver)
        {
        }

        internal void ClickGoBackLink()
        {
            Driver.FindElement(MarketingCommonObjects.GoBackLink).Click();
        }

        internal void ClickSave()
        {
            Driver.FindElement(MarketingCommonObjects.SaveAndReturn).Click();
        }

        internal void ClickFirstCheckbox() => Driver.FindElements(By.CssSelector("input[type=checkbox]")).First().Click();
    }
}