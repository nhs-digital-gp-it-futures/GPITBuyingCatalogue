using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Common;
using OpenQA.Selenium;
using SeleniumExtras.PageObjects;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin
{
    public class NominatedOrganisationObjects
    {
        public static By CancelLink => By.LinkText("Cancel");

        public static By ContinueLink => By.LinkText("Continue");

        public static By NominatedOrganisationsTable => ByExtensions.DataTestId("nominated-organisations-table");

        public static By NominatedOrganisationsErrorMessage => ByExtensions.DataTestId("nominated-organisations-error-message");

        public static By NominatedOrganisationName => ByExtensions.DataTestId("nominated-organisation-name");

        public static By NominatedOrganisationsOdsCode => ByExtensions.DataTestId("nominated-organisation-ods-code");

        public static By RemoveNominatedOrganisationLink => ByExtensions.DataTestId("remove-nominated-organisation-link");

        public static By SelectedOrganisation => By.Id("SelectedOrganisationId");

        public static By SelectedOrganisationError => By.Id("SelectedOrganisationId-error");

        public static By SearchListBox => By.Id("SelectedOrganisationId__listbox");

        public static By SearchResultsErrorMessage => new ByChained(SearchListBox, By.ClassName("autocomplete__option--no-results"));

        public static By SearchResult(uint index) => By.Id($"SelectedOrganisationId__option--{index}");
    }
}
