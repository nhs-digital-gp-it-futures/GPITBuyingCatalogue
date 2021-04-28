using NHSD.GPIT.BuyingCatalogue.E2ETests.Common.Actions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.RandomData;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Marketing
{
    internal sealed class AboutSupplierActions : ActionBase
    {
        public AboutSupplierActions(IWebDriver driver) : base(driver)
        {
        }

        internal string DescriptionAddText(int numChars)
        {
            Driver.FindElement(AboutSupplierObjects.Description).Clear();
            var description = Strings.RandomString(numChars);
            Driver.FindElement(AboutSupplierObjects.Description).SendKeys(description);
            return description;
        }

        internal string LinkAddText(int numChars)
        {
            Driver.FindElement(AboutSupplierObjects.Link).Clear();
            var description = Strings.RandomString(numChars);
            Driver.FindElement(AboutSupplierObjects.Link).SendKeys(description);
            return description;
        }
    }
}
