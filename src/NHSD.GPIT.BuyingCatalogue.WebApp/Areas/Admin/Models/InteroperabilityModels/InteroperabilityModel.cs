using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.InteroperabilityModels
{
    public sealed class InteroperabilityModel : NavBaseModel
    {
        public InteroperabilityModel()
        {
        }

        public InteroperabilityModel(CatalogueItem catalogueItem)
        {
            SetSolution(catalogueItem);
        }

        public string SolutionName { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public SolutionIntegration[] IM1Integrations { get; set; }

        public SolutionIntegration[] GpConnectIntegrations { get; set; }

        public SolutionIntegration[] NhsAppIntegrations { get; set; }

        [StringLength(1000)]
        public string Link { get; set; }

        public void SetSolution(CatalogueItem catalogueItem)
        {
            ArgumentNullException.ThrowIfNull(catalogueItem);

            var integrations = catalogueItem.Solution.Integrations;

            IM1Integrations = integrations.Where(i => i.IntegrationType.IntegrationId == SupportedIntegrations.Im1).ToArray();

            GpConnectIntegrations = integrations.Where(i => i.IntegrationType.IntegrationId == SupportedIntegrations.GpConnect).ToArray();

            NhsAppIntegrations = integrations.Where(i => i.IntegrationType.IntegrationId == SupportedIntegrations.NhsApp).ToArray();

            Link = catalogueItem.Solution?.IntegrationsUrl;
            SolutionName = catalogueItem.Name;
            CatalogueItemId = catalogueItem.Id;
        }
    }
}
