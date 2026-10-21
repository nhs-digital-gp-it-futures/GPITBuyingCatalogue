using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.CommunityPharmacy.Models;

public class CommunityPharmacyIntegrationModel : CommunityPharmacySolutionDisplayBaseModel, IIntegrationsModel
{
    public CommunityPharmacyIntegrationModel(CatalogueItem catalogueItem, CatalogueItemContentStatus contentStatus, bool isSubPage = false)
        : base(catalogueItem, contentStatus, isSubPage)
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


    public override int Index => 4;
}
