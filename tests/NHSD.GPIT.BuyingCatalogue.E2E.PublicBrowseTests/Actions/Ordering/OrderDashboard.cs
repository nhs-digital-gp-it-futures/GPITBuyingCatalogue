﻿using NHSD.GPIT.BuyingCatalogue.E2ETests.Actions.Common;
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
                return Driver.FindElement(Objects.Ordering.OrderDashboard.TaskList).Displayed;
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

        public IWebElement GetOrderDescriptionStatus() => Driver.FindElement(Objects.Ordering.OrderDashboard.OrderDescriptionStatus);
    }
}
