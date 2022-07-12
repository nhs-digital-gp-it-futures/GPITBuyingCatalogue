using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Marketing;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.RandomData;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Marketing
{
    public sealed class FeaturesActions : ActionBase
    {
        public FeaturesActions(IWebDriver driver)
            : base(driver)
        {
        }

        public string EnterFeature(int index = 0)
        {
            var randomString = Strings.RandomString(100);

            Driver.FindElements(CommonSelectors.NhsInput)[index].SendKeys(randomString);

            return randomString;
        }
    }
}
