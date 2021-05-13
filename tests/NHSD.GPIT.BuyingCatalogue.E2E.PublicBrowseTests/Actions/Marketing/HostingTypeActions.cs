using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Marketing
{
    internal class HostingTypeActions : ActionBase
    {
        public HostingTypeActions(IWebDriver driver) : base(driver)
        {
        }

        internal bool ToggleHSCNCheckbox()
        {
            var checkbox = Driver.FindElement(CommonSelectors.Checkbox);
            checkbox.Click();
            return bool.Parse(checkbox.GetProperty("checked"));
        }
    }
}