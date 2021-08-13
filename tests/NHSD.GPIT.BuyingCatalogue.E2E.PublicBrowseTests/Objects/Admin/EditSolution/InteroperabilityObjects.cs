using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Objects.Admin.EditSolution
{
    internal static class InteroperabilityObjects
    {
        internal static By IntegrationType => By.Id("SelectedIntegrationType");

        internal static By ProviderOrConsumer => By.Id("SelectedProviderOrConsumer");

        internal static By IntegratesWith => By.Id(nameof(IntegratesWith));
    }
}
