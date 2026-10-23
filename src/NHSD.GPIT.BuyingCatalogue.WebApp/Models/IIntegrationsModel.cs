using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models;

public interface IIntegrationsModel
{
    public SolutionIntegration[] IM1Integrations { get; set; }

    public SolutionIntegration[] GpConnectIntegrations { get; set; }

    public SolutionIntegration[] NhsAppIntegrations { get; set; }

    public string IntegrationsUrl { get; set; }
}
