using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Authorization;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Authorization
{
    public class CommonActions : ActionBase
    {
        public CommonActions(IWebDriver driver)
            : base(driver)
        {
        }

        public bool LogoutLinkDisplayed()
        {
            try
            {
                Driver.FindElement(AuthorizationObjects.LogoutLink);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
