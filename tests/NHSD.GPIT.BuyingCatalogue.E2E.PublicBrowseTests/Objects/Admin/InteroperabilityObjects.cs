using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin
{
    internal static class InteroperabilityObjects
    {
        internal static By SelectedIntegrationType => By.Id(nameof(SelectedIntegrationType));

        internal static By SelectedProviderOrConsumer => By.Id(nameof(SelectedProviderOrConsumer));

        internal static By IntegratesWith => By.Id(nameof(IntegratesWith));
    }
}
