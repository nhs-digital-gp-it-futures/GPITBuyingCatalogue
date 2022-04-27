using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse
{
    public static class ContactUsObjects
    {
        public static By HomeCrumb => ByExtensions.DataTestId("home-crumb");

        public static By ContactUsCrumb => ByExtensions.DataTestId("contact-us-crumb");

        public static By TriageSection => ByExtensions.DataTestId("triage-section");

        public static By ContactMethodInput => By.Id("contact-method");

        public static By ContactMethodInputError => By.Id("contact-method-error");

        public static By FullNameInput => By.Id("FullName");

        public static By FullNameInputError => By.Id("FullName-error");

        public static By MessageInput => By.Id("Message");

        public static By MessageInputError => By.Id("Message-error");

        public static By EmailAddressInput => By.Id("EmailAddress");

        public static By EmailAddressInputError => By.Id("EmailAddress-error");

        public static By PrivacyPolicyInput => By.Id("privacy-policy-accepted");

        public static By PrivacyPolicyInputError => By.Id("privacy-policy-accepted-error");
    }
}
