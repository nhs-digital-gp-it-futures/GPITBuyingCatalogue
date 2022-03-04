using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin
{
    internal static class HomeObjects
    {
        internal static By ManageCatalogueSolutionsLink => By.LinkText("Manage Catalogue Solutions");

        internal static By ManageSupplierDefinedEpicsLink => By.LinkText("Manage supplier defined Epics");

        internal static By ManageBuyerOrganisationsLink => By.LinkText("Manage buyer organisations");

        internal static By ManageSupplierOrganisationsLink => By.LinkText("Manage supplier organisations");

        internal static By ManageAllUsersLink => By.LinkText("Manage all users");

        internal static By ManageAllOrdersLink => By.LinkText("Manage all orders");
    }
}
