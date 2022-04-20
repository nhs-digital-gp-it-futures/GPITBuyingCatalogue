using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Marketing;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Marketing
{
    public class HostingTypeActions : ActionBase
    {
        public HostingTypeActions(IWebDriver driver)
            : base(driver)
        {
        }

        public bool ToggleHSCNCheckbox()
        {
            var checkbox = Driver.FindElement(CommonSelectors.Checkbox);
            checkbox.Click();
            return bool.Parse(checkbox.GetDomProperty("checked"));
        }
    }
}
