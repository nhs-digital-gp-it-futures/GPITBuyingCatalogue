using NHSD.GPIT.BuyingCatalogue.E2ETests.Common.Actions;
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
            return Driver.FindElement(Objects.PublicBrowse.CommonObjects.PageTitle).Text;
        }
    }
}
