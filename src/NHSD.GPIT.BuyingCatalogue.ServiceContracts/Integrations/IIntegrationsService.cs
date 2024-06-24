using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Integrations;

public interface IIntegrationsService
{
    Task<IEnumerable<Integration>> GetIntegrationsWithTypes();

    Task<Dictionary<string, IOrderedEnumerable<string>>> GetIntegrationAndTypeNames(
        Dictionary<SupportedIntegrations, int[]> integrationAndTypeIds);

    Task<IEnumerable<IntegrationType>> GetIntegrationTypesByIntegration(SupportedIntegrations integration);
}
