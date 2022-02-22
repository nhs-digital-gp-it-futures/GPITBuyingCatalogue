using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.PublicBrowse
{
    internal static class NominateOrganisationObjects
    {
        internal static By ProcurementHubLink => By.Id("procurement-hub-link");

        internal static By NominateAnOrganisationLink => By.LinkText("Nominate an organisation");

        internal static By OrganisationNameInput => By.Id("OrganisationName");

        internal static By OrganisationNameError => By.Id("OrganisationName-error");

        internal static By OdsCodeInput => By.Id("OdsCode");

        internal static By HasReadPrivacyPolicyError => By.Id("nominate-organisation-details-error");
    }
}
