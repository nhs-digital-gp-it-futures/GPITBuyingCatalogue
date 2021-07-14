using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Ordering
{
    internal sealed class OrderingPages
    {

        public OrderingPages(IWebDriver driver)
        {
            PageActions = new ActionCollection
            {

            };
        }

        internal ActionCollection PageActions { get; }
    }
}
