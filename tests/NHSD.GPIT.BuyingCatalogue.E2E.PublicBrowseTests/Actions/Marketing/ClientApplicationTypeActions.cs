using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using OpenQA.Selenium;
using System;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Marketing
{
    internal class ClientApplicationTypeActions : ActionBase
    {
        public ClientApplicationTypeActions(IWebDriver driver) : base(driver)
        {
        }

        internal void SelectClientApplicationCheckbox(string clientApplicationType)
        {
            switch (clientApplicationType)
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
                    throw new ArgumentOutOfRangeException(nameof(clientApplicationType));
            }
        }
    }
}