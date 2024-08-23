using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering
{
    public static class CalloffPartyInformation
    {
        public static By CallOffOrderingPartyContactDetailsLink => By.LinkText("Call-off Ordering Party contact details");

        public static By OrganisationName => ByExtensions.DataTestId("organisation-name");

        public static By OrganisationOdsCode => ByExtensions.DataTestId("organisation-ods-code");

        public static By OrganisationAddress => ByExtensions.DataTestId("organisation-address");

        public static By FirstNameInput => By.Id("FirstName");

        public static By LastNameInput => By.Id("LastName");

        public static By EmailAddressInput => By.Id("EmailAddress");

        public static By PhoneNumberInput => By.Id("TelephoneNumber");
    }
}
