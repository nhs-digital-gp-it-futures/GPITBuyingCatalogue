using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Ordering
{
    internal static class OrderDashboard
    {
        public static By TaskList => By.ClassName("bc-c-task-list");

        public static By OrderDescriptionLink => By.LinkText("Provide a description of your order");

        public static By OrderDescriptionStatus => By.Id("Provide_a_description_of_your_order-status");
    }
}
