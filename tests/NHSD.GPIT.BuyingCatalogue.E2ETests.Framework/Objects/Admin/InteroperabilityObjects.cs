using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin
{
    public static class InteroperabilityObjects
    {
        public static By SelectedIntegrationType => By.Id(nameof(SelectedIntegrationType));

        public static By SelectedProviderOrConsumer => By.Id(nameof(SelectedProviderOrConsumer));

        public static By IntegratesWith => By.Id(nameof(IntegratesWith));
    }
}
