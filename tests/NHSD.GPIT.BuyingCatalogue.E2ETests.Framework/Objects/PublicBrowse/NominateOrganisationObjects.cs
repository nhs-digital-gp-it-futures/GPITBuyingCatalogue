using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.PublicBrowse
{
    public static class NominateOrganisationObjects
    {
        public static By ProcurementHubLink => By.LinkText("Get procurement support");

        public static By NominateAnOrganisationLink => By.LinkText("Nominate an organisation");

        public static By OrganisationNameInput => By.Id("OrganisationName");

        public static By OrganisationNameError => By.Id("OrganisationName-error");

        public static By OdsCodeInput => By.Id("OdsCode");

        public static By HasReadPrivacyPolicyError => By.Id("nominate-organisation-details-error");
    }
}
