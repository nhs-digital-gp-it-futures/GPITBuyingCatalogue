using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Ordering
{
    internal sealed class OrderDashboard : ActionBase
    {
        public OrderDashboard(IWebDriver driver)
            : base(driver)
        {
        }

        public bool TaskListDisplayed()
        {
            try
            {
                Wait.Until(d => d.FindElement(Objects.Ordering.OrderDashboard.TaskList));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool OrderDescriptionLinkDisplayed()
        {
            try
            {
                Wait.Until(d => d.FindElement(Objects.Ordering.OrderDashboard.OrderDescriptionLink));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void ClickOrderDescriptionLink()
        {
            Driver.FindElement(Objects.Ordering.OrderDashboard.OrderDescriptionLink).Click();
        }

        public bool OrderDescriptionStatusDisplayed()
        {
            try
            {
                Wait.Until(d => d.FindElement(Objects.Ordering.OrderDashboard.OrderDescriptionStatus));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IWebElement GetOrderDescriptionStatus() => Driver.FindElement(Objects.Ordering.OrderDashboard.OrderDescriptionStatus);
    }
}
