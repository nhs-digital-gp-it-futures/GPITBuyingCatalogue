using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.PublicBrowse;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.PublicBrowse
{
    internal sealed class CommonActions : ActionBase
    {
        public CommonActions(IWebDriver driver) : base(driver)
        {
        }

        internal string PageTitle()
        {
            return Driver.FindElement(CommonObjects.PageTitle).Text;
        }

        internal void ClickGoBackLink()
        {
            Driver.FindElement(CommonObjects.GoBackLink).Click();
        }

        internal void ClickContinueButton()
        {
            Driver.FindElement(CommonObjects.ContinueButton).Click();
        }
    }
}
