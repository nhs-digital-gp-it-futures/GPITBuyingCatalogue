using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class InteroperabilityModel : SolutionDisplayBaseModel
    {
        public InteroperabilityModel()
        {
        }

        public InteroperabilityModel(CatalogueItem catalogueItem, CatalogueItemContentStatus contentStatus)
            : base(catalogueItem, contentStatus)
        {
            ArgumentNullException.ThrowIfNull(catalogueItem);

            var integrations = catalogueItem.Solution.Integrations;

            IntegrationsUrl = catalogueItem.Solution.IntegrationsUrl;

            IM1Integrations = integrations.Where(i => i.IntegrationType.IntegrationId == SupportedIntegrations.Im1).ToArray();
            GpConnectIntegrations = integrations.Where(i => i.IntegrationType.IntegrationId == SupportedIntegrations.GpConnect).ToArray();
            NhsAppIntegrations = integrations.Where(i => i.IntegrationType.IntegrationId == SupportedIntegrations.NhsApp).ToArray();
        }

        public SolutionIntegration[] IM1Integrations { get; set; }

        public SolutionIntegration[] GpConnectIntegrations { get; set; }

        public SolutionIntegration[] NhsAppIntegrations { get; set; }

        public string IntegrationsUrl { get; set; }

        public override int Index => 8;
    }
}
