using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils
{
    public class PageBase
    {
        public PageBase(IWebDriver driver, CommonActions commonActions)
        {
            CommonActions = commonActions;
            Driver = driver;
            TextGenerators = new TextGenerators(Driver);
        }

        internal CommonActions CommonActions { get; }

        internal TextGenerators TextGenerators { get; }

        internal IWebDriver Driver { get; private set; }
    }
}
