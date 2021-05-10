using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.RandomData;
using OpenQA.Selenium;
using System;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Marketing
{
    internal class PublicCloudActions : ActionBase
    {
        public PublicCloudActions(IWebDriver driver) : base(driver)
        {
        }

        internal string EnterSummary(int charCount)
        {
            var summary = Strings.RandomString(charCount);

            Driver.FindElement(HostingTypesObjects.PublicCloud_Summary).SendKeys(summary);

            return summary;
        }

        internal string EnterLink(int charCount)
        {
            var link = Strings.RandomString(charCount);

            Driver.FindElement(HostingTypesObjects.PublicCloud_Link).SendKeys(link);

            return link;
        }

        internal bool ToggleHSCNCheckbox()
        {
            var checkbox = Driver.FindElement(HostingTypesObjects.HSCN_Checkbox);
            checkbox.Click();
            return bool.Parse(checkbox.GetProperty("checked"));
        }
    }
}