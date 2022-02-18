using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.PublicBrowse
{
    internal static class ProcurementHubObjects
    {
        internal static By FullNameInput => By.Id("FullName");

        internal static By FullNameError => By.Id("FullName-error");

        internal static By EmailAddressInput => By.Id("EmailAddress");

        internal static By EmailAddressError => By.Id("EmailAddress-error");

        internal static By OrganisationNameInput => By.Id("OrganisationName");

        internal static By OrganisationNameError => By.Id("OrganisationName-error");

        internal static By OdsCodeInput => By.Id("OdsCode");

        internal static By QueryInput => By.Id("Query");

        internal static By QueryError => By.Id("Query-error");

        internal static By HasReadPrivacyPolicyError => By.Id("procurement-hub-details-error");
    }
}
