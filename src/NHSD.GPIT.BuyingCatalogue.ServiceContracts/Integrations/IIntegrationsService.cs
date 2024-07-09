using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Integrations;

public interface IIntegrationsService
{
    Task<IEnumerable<Integration>> GetIntegrations();

    Task<IEnumerable<Integration>> GetIntegrationsWithTypes();

    Task<Integration> GetIntegrationWithTypes(SupportedIntegrations integrationId);

    Task<Dictionary<string, IOrderedEnumerable<string>>> GetIntegrationAndTypeNames(
        Dictionary<SupportedIntegrations, int[]> integrationAndTypeIds);

    Task<IEnumerable<IntegrationType>> GetIntegrationTypesByIntegration(SupportedIntegrations integration);

    Task<bool> IntegrationTypeExists(SupportedIntegrations integrationId, string integrationTypeName);
}
