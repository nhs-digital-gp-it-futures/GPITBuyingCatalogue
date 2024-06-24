using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Integrations;

public interface IIntegrationsService
{
    Task<IEnumerable<Integration>> GetIntegrationsWithTypes();

    Task<IEnumerable<IntegrationType>> GetIntegrationTypesByIntegration(SupportedIntegrations integration);
}
