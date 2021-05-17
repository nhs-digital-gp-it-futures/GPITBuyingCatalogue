using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Authorization;
using OpenQA.Selenium;
using System;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Authorization
{
    internal class CommonActions : ActionBase
    {
        public CommonActions(IWebDriver driver) : base(driver)
        {
        }

        internal bool LogoutLinkDisplayed()
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