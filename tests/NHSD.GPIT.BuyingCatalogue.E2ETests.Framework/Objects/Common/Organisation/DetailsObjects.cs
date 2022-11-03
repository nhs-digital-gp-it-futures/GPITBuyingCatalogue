using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common.Organisation
{
    public static class DetailsObjects
    {
        public static By UserAccountsLink => By.LinkText("User accounts");

        public static By RelatedOrganisationsLink => By.LinkText("Related organisations");

        public static By NominatedOrganisationsLink => By.LinkText("Nominated organisations");

        public static By OdsCode => ByExtensions.DataTestId("org-page-external-identifier");

        public static By AddressLines => ByExtensions.DataTestId("org-page-address");

    }
}
