using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.PublicBrowse
{
    public class CapabilitySelectorActions : ActionBase
    {
        public CapabilitySelectorActions(IWebDriver driver)
            : base(driver)
        {
        }

        public IEnumerable<string> GetAllCheckboxLabels()
        {
            var checkboxes = Driver.FindElements(CapabilitySelectorObjects.CheckboxGroups);

            return checkboxes.Select(s => s.FindElement(By.TagName("label")).Text);
        }

        public void ClickFirstCheckbox()
        {
            ClickCheckbox(0);
        }

        public void ClickCheckbox(int index)
        {
            var checkboxes = Driver.FindElements(CapabilitySelectorObjects.CheckboxGroups);

            checkboxes[index].FindElement(By.TagName("input")).Click();
        }
    }
}
