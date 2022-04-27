using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering
{
    public static class OrderDashboard
    {
        public static By TaskList => By.ClassName("bc-c-task-list");

        public static By OrderDescriptionLink => By.LinkText("Order description");

        public static By OrderDescriptionStatus => By.Id("Order_description-status");

        public static By LastUpdatedEndNote => ByExtensions.DataTestId("last-updated-endnote");
    }
}
