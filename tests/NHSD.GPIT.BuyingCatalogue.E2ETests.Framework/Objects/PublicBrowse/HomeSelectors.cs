using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Home
{
    public static class HomeSelectors
    {
        public static By ManageOrdersLink => By.Id("manage-orders-link");

        public static By RequestAccountLink => By.Id("request-account-link");

        public static By NominateOrganisationLink => By.Id("nominate-organisation-link");

        public static By ContactUsLink => By.LinkText("Contact us");

        public static By TechInnovationLink => By.LinkText("Tech Innovation framework");

        public static By DFOCVCframeworkLink => By.LinkText("DFOCVC framework");

        public static By AdvancedTelephonyLink => By.LinkText("Advanced Telephony Better Purchasing framework");
    }
}
