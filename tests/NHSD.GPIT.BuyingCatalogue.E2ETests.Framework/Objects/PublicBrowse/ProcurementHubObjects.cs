using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse
{
    public static class ProcurementHubObjects
    {
        public static By FullNameInput => By.Id("FullName");

        public static By FullNameError => By.Id("FullName-error");

        public static By EmailAddressInput => By.Id("EmailAddress");

        public static By EmailAddressError => By.Id("EmailAddress-error");

        public static By OrganisationNameInput => By.Id("OrganisationName");

        public static By OrganisationNameError => By.Id("OrganisationName-error");

        public static By OdsCodeInput => By.Id("OdsCode");

        public static By QueryInput => By.Id("Query");

        public static By QueryError => By.Id("Query-error");

        public static By HasReadPrivacyPolicyError => By.Id("procurement-hub-details-error");
    }
}
