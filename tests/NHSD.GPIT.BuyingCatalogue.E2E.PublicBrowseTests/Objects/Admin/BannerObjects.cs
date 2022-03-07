using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin
{
    internal static class BannerObjects
    {
        internal static By BuyerOrganisationsLink => By.LinkText("Buyer organisations");

        internal static By SupplierOrganisationsLink => By.LinkText("Supplier organisations");

        internal static By CatalogueSolutionsLink => By.LinkText("Catalogue Solutions");

        internal static By LogOutLink => By.LinkText("Log out");
    }
}
