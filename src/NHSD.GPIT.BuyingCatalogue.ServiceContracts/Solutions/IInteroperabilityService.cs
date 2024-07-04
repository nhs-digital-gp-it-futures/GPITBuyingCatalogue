using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public interface IInteroperabilityService
    {
        Task SaveIntegrationLink(CatalogueItemId solutionId, string integrationLink);

        Task AddIntegration(CatalogueItemId catalogueItemId, SolutionIntegration integration);

        Task EditIntegration(CatalogueItemId solutionId, int integrationId, SolutionIntegration integration);

        Task DeleteIntegration(CatalogueItemId solutionId, int integrationId);

        Task SetNhsAppIntegrations(CatalogueItemId solutionId, IEnumerable<int> integrations);
    }
}
