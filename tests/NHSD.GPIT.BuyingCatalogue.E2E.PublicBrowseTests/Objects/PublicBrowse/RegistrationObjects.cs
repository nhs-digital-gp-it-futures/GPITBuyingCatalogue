using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.PublicBrowse
{
    internal static class RegistrationObjects
    {
        internal static By RequestAnAccountLink => By.LinkText("Request an account");

        internal static By FullNameInput => By.Id("FullName");

        internal static By FullNameError => By.Id("FullName-error");

        internal static By TelephoneNumberInput => By.Id("TelephoneNumber");

        internal static By TelephoneNumberError => By.Id("TelephoneNumber-error");

        internal static By EmailAddressInput => By.Id("EmailAddress");

        internal static By EmailAddressError => By.Id("EmailAddress-error");

        internal static By OrganisationNameInput => By.Id("OrganisationName");

        internal static By OrganisationNameError => By.Id("OrganisationName-error");

        internal static By OdsCodeInput => By.Id("OdsCode");

        internal static By HasReadPrivacyPolicyError => By.Id("registration-details-error");
    }
}
