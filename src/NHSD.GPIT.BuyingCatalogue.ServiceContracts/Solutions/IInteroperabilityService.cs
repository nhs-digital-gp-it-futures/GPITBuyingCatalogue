using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public interface IInteroperabilityService
    {
        Task SaveIntegrationLink(CatalogueItemId solutionId, string integrationLink);

        Task AddIntegration(CatalogueItemId catalogueItemId, Integration integration);

        Task<Integration> GetIntegrationById(CatalogueItemId solutionId, Guid integrationId);

        Task EditIntegration(CatalogueItemId solutionId, Guid integrationId, Integration integration);

        Task DeleteIntegration(CatalogueItemId solutionId, Guid integrationId);

        Task SetNhsAppIntegrations(CatalogueItemId solutionId, IEnumerable<string> integrations);
    }
}
