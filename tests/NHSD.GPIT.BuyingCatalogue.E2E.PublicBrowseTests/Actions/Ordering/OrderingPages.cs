using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Ordering
{
    internal sealed class OrderingPages
    {
        public OrderingPages(IWebDriver driver)
        {
            PageActions = new ActionCollection
            {
                OrderDashboard = new(driver),
            };
        }

        internal ActionCollection PageActions { get; }
    }
}
