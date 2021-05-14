using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.PublicBrowse;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.PublicBrowse
{
    internal class CapabilitySelectorActions : ActionBase
    {
        public CapabilitySelectorActions(IWebDriver driver) : base(driver)
        {
        }

        internal IEnumerable<string> GetAllCheckboxLabels()
        {
            var checkboxes = Driver.FindElements(CapabilitySelectorObjects.CheckboxGroups);

            return checkboxes.Select(s => s.FindElement(By.TagName("label")).Text);
         }

        internal void ClickFirstCheckbox()
        {
            ClickCheckbox(0);
        }

        internal void ClickCheckbox(int index)
        {
            var checkboxes = Driver.FindElements(CapabilitySelectorObjects.CheckboxGroups);

            checkboxes[0].FindElement(By.TagName("input")).Click();
        }
    }
}