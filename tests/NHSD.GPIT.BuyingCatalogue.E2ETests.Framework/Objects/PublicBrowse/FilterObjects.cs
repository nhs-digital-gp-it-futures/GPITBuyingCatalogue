using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse
{
    public static class FilterObjects
    {
        public static By HomeBreadcrumbLink => By.LinkText("Home");

        public static By CatalogueSolutionsBreadcrumbLink => By.LinkText("Catalogue Solutions");

        public static By CapabilitiesBreadcrumbLink => By.LinkText("Capabilities");

        public static By EditCapabilitiesLink => By.LinkText("Edit Capabilities");
    }
}
