using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin
{
    internal class AddRelatedOrgsObjects
    {
        internal static By SubmitButton => By.Id("Submit");

        internal static By OrganisationRadioButtons => By.Id("SelectedOrganisation");
    }
}
