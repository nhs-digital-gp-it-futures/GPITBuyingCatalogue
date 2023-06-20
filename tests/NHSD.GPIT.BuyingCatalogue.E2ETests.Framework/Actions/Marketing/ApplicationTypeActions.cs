using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Marketing;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Marketing
{
    public class ApplicationTypeActions : ActionBase
    {
        public ApplicationTypeActions(IWebDriver driver)
            : base(driver)
        {
        }

        public void SelectApplicationTypeCheckbox(string applicationType)
        {
            switch (applicationType)
            {
                case "Browser-based":
                    Driver.FindElement(CommonSelectors.BrowserBasedCheckbox).Click();
                    break;

                case "Native mobile or tablet":
                    Driver.FindElement(CommonSelectors.NativeMobileCheckbox).Click();
                    break;

                case "Native desktop":
                    Driver.FindElement(CommonSelectors.NativeDesktopCheckbox).Click();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(applicationType));
            }
        }
    }
}
