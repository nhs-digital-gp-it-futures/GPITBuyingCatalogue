using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin
{
    internal static class BreadcrumbObjects
    {
        public static By HomeBreadcrumbLink => By.LinkText("Home");

        public static By ManageBuyerOrganisationsBreadcrumbLink => By.LinkText("Manage buyer organisations");

        public static By ManageUsersBreadcrumbLink => By.LinkText("Manage users");

        public static By OrganisationDetailsBreadcrumbLink => By.LinkText("Organisation details");
    }
}
