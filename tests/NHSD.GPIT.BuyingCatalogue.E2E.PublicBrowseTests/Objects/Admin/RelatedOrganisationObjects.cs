using NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Common;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin
{
    internal class RelatedOrganisationObjects
    {
        public static By CancelLink => By.LinkText("Cancel");

        public static By ContinueLink => By.LinkText("Continue");

        public static By RelatedOrganisationsTable => ByExtensions.DataTestId("related-organisations-table");

        public static By RelatedOrganisationsErrorMessage => ByExtensions.DataTestId("related-organisations-error-message");

        public static By RelatedOrganisationName => ByExtensions.DataTestId("related-organisation-name");

        public static By RelatedOrganisationsOdsCode => ByExtensions.DataTestId("related-organisation-ods-code");

        public static By RemoveRelatedOrganisationLink => ByExtensions.DataTestId("remove-related-organisation-link");
    }
}
