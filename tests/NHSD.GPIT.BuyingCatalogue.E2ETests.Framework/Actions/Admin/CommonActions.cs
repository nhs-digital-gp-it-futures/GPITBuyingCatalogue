using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Admin
{
    public class CommonActions : ActionBase
    {
        public CommonActions(IWebDriver driver)
            : base(driver)
        {
        }

        public void SavePage()
        {
            Driver.FindElement(Objects.Admin.CommonObjects.SaveButton).Click();
        }

        public void ClickGoBack()
        {
            Driver.FindElement(Objects.Admin.CommonObjects.GoBackLink).Click();
        }

        public void ClickAddHostingTypeLink()
        {
            Driver.FindElement(Objects.Admin.CommonObjects.ActionLink).Click();
        }
    }
}
