using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Admin
{
    public static class InteroperabilityObjects
    {
        public static By SelectedIntegrationType => By.Id(nameof(SelectedIntegrationType));

        public static By SelectedProviderOrConsumer => By.Id(nameof(SelectedProviderOrConsumer));

        public static By IntegratesWith => By.Id(nameof(IntegratesWith));

        public static By NHSAppIntegrationsLink => By.LinkText("Add NHS App integrations");

        public static By IM1IntegrationsLink => By.LinkText("Add an IM1 integration");

        public static By GPConnectIntegrationsLink => By.LinkText("Add a GP Connect integration");
    }
}
