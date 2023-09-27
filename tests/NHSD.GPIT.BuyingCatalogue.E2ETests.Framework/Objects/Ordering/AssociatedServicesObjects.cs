using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering
{
    public static class AssociatedServicesObjects
    {
        public static By AdditionalServicesRequired => By.Id("add-associated-services");

        public static By AdditionalServicesRequiredErrorMessage => By.Id("add-associated-services-error");

        public static By SelectedServicesErrorMessage => By.Id("select-services-error");

        public static By ServicesToSelect => By.Id("services-to-select");

        public static By NothingToSelect => By.Id("nothing-to-select");

        public static By AddAssociateServiceLink => By.LinkText("Add Associated Services");
    }
}
