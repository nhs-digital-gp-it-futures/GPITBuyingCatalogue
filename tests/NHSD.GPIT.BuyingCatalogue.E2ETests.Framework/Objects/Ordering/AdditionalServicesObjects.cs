using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering
{
    public static class AdditionalServicesObjects
    {
        public static By ExistingServices => By.Id("existing-services");

        public static By ServicesToSelect => By.Id("services-to-select");

        public static By NothingToSelect => By.Id("nothing-to-select");
    }
}
