using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Ordering
{
    public sealed class OrderDashboard : ActionBase
    {
        public OrderDashboard(IWebDriver driver)
            : base(driver)
        {
        }

        public IWebElement GetOrderDescriptionStatus() => Driver.FindElement(Objects.Ordering.OrderDashboard.OrderDescriptionStatus);
    }
}
