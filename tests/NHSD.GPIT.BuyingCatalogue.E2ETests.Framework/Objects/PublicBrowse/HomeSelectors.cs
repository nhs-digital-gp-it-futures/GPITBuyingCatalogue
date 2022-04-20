using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Home
{
    public static class HomeSelectors
    {
        public static By ManageOrdersLink => By.Id("manage-orders-link");

        public static By RequestAccountLink => By.Id("request-account-link");

        public static By NominateOrganisationLink => By.Id("nominate-organisation-link");

        public static By ContactUsLink => By.LinkText("Contact us");
    }
}
