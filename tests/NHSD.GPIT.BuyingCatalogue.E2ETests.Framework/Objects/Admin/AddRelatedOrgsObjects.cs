using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin
{
    public static class AddRelatedOrgsObjects
    {
        public static By SubmitButton => By.Id("Submit");

        public static By OrganisationRadioButtons => By.Name("SelectedOrganisation");
    }
}
