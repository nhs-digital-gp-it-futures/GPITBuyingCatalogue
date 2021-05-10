using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Marketing
{
    internal sealed class Hosting
    {
        public Hosting(IWebDriver driver)
        {
            PublicCloudActions = new(driver);
            PrivateCloudActions = new(driver);
            HybridActions = new(driver);
            OnPremiseActions = new(driver);
        }

        internal PublicCloudActions PublicCloudActions { get; set; }

        internal PrivateCloudActions PrivateCloudActions { get; set; }

        internal HybridActions HybridActions { get; set; }

        internal OnPremiseActions OnPremiseActions { get; set; }
    }
}