using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common
{
    public static class BreadcrumbObjects
    {
        public static By HomeBreadcrumbLink => By.LinkText("Home");

        public static By BuyerDashboardBreadcrumbLink => By.LinkText("Buyer Dashboard");

        public static By OrganisationDetailsBreadcrumbLink => By.LinkText("Organisation details");
    }
}
