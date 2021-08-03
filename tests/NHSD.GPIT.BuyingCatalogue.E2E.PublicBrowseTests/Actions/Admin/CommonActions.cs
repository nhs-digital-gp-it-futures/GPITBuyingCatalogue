using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Admin
{
    internal class CommonActions : ActionBase
    {
        public CommonActions(IWebDriver driver)
            : base(driver)
        {
        }

        internal void SavePage()
        {
            Driver.FindElement(Objects.Admin.CommonObjects.SaveButton).Click();
        }

        internal void ClickGoBack()
        {
            Driver.FindElement(Objects.Admin.CommonObjects.GoBackLink).Click();
        }

        internal void ClickAddHostingTypeLink()
        {
            Driver.FindElement(Objects.Admin.CommonObjects.ActionLink).Click();
        }
    }
}
