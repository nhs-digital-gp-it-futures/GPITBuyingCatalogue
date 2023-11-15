using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
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

        public Integration[] IM1Integrations { get; set; }

        public string[] IM1IntegrationQualifiers { get; set; }

        public Integration[] GpConnectIntegrations { get; set; }

        public string[] GpConnectIntegrationQualifiers { get; set; }

        public Integration[] NhsAppIntegrations { get; set; }

        public string[] NhsAppIntegrationTypes { get; set; }

        [StringLength(1000)]
        public string Link { get; set; }

        public void SetSolution(CatalogueItem catalogueItem)
        {
            if (catalogueItem is null)
                throw new ArgumentNullException(nameof(catalogueItem));

            var integrations = catalogueItem.Solution?.GetIntegrations();

            IM1Integrations = integrations.Where(i => i.IntegrationType.EqualsIgnoreCase(Framework.Constants.Interoperability.IM1IntegrationType)).ToArray();
            IM1IntegrationQualifiers = IM1Integrations.Select(i => i.Qualifier).Distinct().ToArray();

            GpConnectIntegrations = integrations.Where(i => i.IntegrationType.EqualsIgnoreCase(Framework.Constants.Interoperability.GpConnectIntegrationType)).ToArray();
            GpConnectIntegrationQualifiers = GpConnectIntegrations.Select(i => i.Qualifier).Distinct().ToArray();

            NhsAppIntegrations = integrations.Where(i => i.IntegrationType.EqualsIgnoreCase(Framework.Constants.Interoperability.NhsAppIntegrationType)).ToArray();
            NhsAppIntegrationTypes = NhsAppIntegrations.Select(i => i.Qualifier).Distinct().ToArray();

            Link = catalogueItem.Solution?.IntegrationsUrl;
            SolutionName = catalogueItem.Name;
            CatalogueItemId = catalogueItem.Id;
        }
    }
}
