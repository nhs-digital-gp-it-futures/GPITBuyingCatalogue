﻿using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Ordering
{
    internal static class OrderDashboard
    {
        public static By TaskList => By.ClassName("bc-c-task-list");

        public static By OrderDescriptionLink => By.LinkText("Order description");

        public static By OrderDescriptionStatus => By.Id("Order_description-status");
    }
}
