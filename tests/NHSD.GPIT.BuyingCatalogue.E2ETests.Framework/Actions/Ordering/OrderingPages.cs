using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Ordering
{
    public sealed class OrderingPages
    {
        public OrderingPages(IWebDriver driver)
        {
            PageActions = new ActionCollection
            {
                CatalogueItems = new(driver),
                OrderDashboard = new(driver),
            };
        }

        public ActionCollection PageActions { get; }
    }
}
