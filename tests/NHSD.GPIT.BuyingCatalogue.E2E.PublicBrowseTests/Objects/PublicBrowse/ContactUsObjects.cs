using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.PublicBrowse
{
    internal static class ContactUsObjects
    {
        internal static By HomeCrumb => ByExtensions.DataTestId("home-crumb");

        internal static By ContactUsCrumb => ByExtensions.DataTestId("contact-us-crumb");

        internal static By TriageSection => ByExtensions.DataTestId("triage-section");

        internal static By ContactMethodInput => By.Id("contact-method");

        internal static By ContactMethodInputError => By.Id("contact-method-error");

        internal static By FullNameInput => By.Id("FullName");

        internal static By FullNameInputError => By.Id("FullName-error");

        internal static By MessageInput => By.Id("Message");

        internal static By MessageInputError => By.Id("Message-error");

        internal static By EmailAddressInput => By.Id("EmailAddress");

        internal static By EmailAddressInputError => By.Id("EmailAddress-error");

        internal static By PrivacyPolicyInput => By.Id("privacy-policy-accepted");

        internal static By PrivacyPolicyInputError => By.Id("privacy-policy-accepted-error");
    }
}
