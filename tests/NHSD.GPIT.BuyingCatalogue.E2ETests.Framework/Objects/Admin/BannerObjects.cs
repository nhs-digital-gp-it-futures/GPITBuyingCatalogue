using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin
{
    public static class BannerObjects
    {
        public static By BuyerOrganisationsLink => By.LinkText("Buyer organisations");

        public static By SupplierOrganisationsLink => By.LinkText("Supplier organisations");

        public static By CatalogueSolutionsLink => By.LinkText("Catalogue Solutions");

        public static By LogOutLink => By.LinkText("Log out");
    }
}
