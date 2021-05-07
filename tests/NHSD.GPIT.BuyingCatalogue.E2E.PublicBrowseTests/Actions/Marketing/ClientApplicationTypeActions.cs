using NHSD.GPIT.BuyingCatalogue.E2ETests.Common.Actions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.RandomData;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;

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
                    Driver.FindElement(ClientApplicationObjects.BrowserBasedCheckbox).Click();
                    break;
                case "Native mobile or tablet":
                    Driver.FindElement(ClientApplicationObjects.NativeMobileCheckbox).Click();
                    break;
                case "Native desktop":
                    Driver.FindElement(ClientApplicationObjects.NativeDesktopCheckbox).Click();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(clientApplicationType));
            }
        }

        internal string ClickBrowserCheckbox(int index = 0)
        {
            var checkboxItems = Driver.FindElements(ClientApplicationObjects.BrowserCheckboxItem);

            var selected = checkboxItems[index];

            selected.FindElement(By.TagName("input")).Click();
            return selected.FindElement(By.TagName("label")).Text;
        }

        internal void ClickRadioButtonWithText(string label)
        {
            var radioButtonItems = Driver.FindElements(ClientApplicationObjects.RadioButtonItems);

            radioButtonItems.Single(r => r.FindElement(By.TagName("label")).Text == label).FindElement(By.TagName("input")).Click();
        }

        internal string EnterAdditionalInformation(int numChars)
        {
            var generatedString = Strings.RandomString(numChars);

            Driver.FindElement(ClientApplicationObjects.AdditionalInfoTextArea).SendKeys(generatedString);

            return generatedString;
        }

        internal void SelectConnectionSpeedDropdown(int index = 0)
        {
            var select = Driver.FindElement(ClientApplicationObjects.ConnectionSpeedSelect);
            new SelectElement(select).SelectByIndex(index);
        }

        internal void SelectResolutionDropdown(int index = 0)
        {
            var select = Driver.FindElement(ClientApplicationObjects.ResolutionSelect);
            new SelectElement(select).SelectByIndex(index);
        }
    }
}