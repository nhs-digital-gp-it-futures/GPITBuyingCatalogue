using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin
{
    public static class HomeObjects
    {
        public static By ManageCatalogueSolutionsLink => By.LinkText("Manage Catalogue Solutions");

        public static By ManageFrameworksLink => By.LinkText("Manage frameworks");

        public static By ManageSupplierDefinedEpicsLink => By.LinkText("Manage supplier defined Epics");

        public static By ManageBuyerOrganisationsLink => By.LinkText("Manage buyer organisations");

        public static By ManageSupplierOrganisationsLink => By.LinkText("Manage supplier organisations");

        public static By ManageAllUsersLink => By.LinkText("Manage all users");

        public static By ManageAllOrdersLink => By.LinkText("Manage all orders");

        public static By ManageCapabilitiesAndEpics => By.LinkText("Manage Capabilities and Epics");
    }
}
