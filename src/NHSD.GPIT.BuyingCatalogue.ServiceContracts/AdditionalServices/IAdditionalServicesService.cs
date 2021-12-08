using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdditionalServices;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices
{
    public interface IAdditionalServicesService
    {
        Task<CatalogueItem> GetAdditionalService(CatalogueItemId catalogueItemId, CatalogueItemId additionalServiceId);

        Task<CatalogueItem> GetAdditionalServiceWithCapabilities(CatalogueItemId additionalServiceId);

        Task<List<CatalogueItem>> GetAdditionalServicesBySolutionId(CatalogueItemId catalogueItemId);

        Task<List<CatalogueItem>> GetAdditionalServicesBySolutionIds(IEnumerable<CatalogueItemId> solutionIds);

        Task<bool> AdditionalServiceExistsWithNameForSolution(
            string additionalServiceName,
            CatalogueItemId solutionId,
            CatalogueItemId currentCatalogueItemId = default);

        Task<CatalogueItemId> AddAdditionalService(CatalogueItem solution, AdditionalServicesDetailsModel model);

        Task EditAdditionalService(CatalogueItemId catalogueItemId, CatalogueItemId additionalServiceId, AdditionalServicesDetailsModel model);

        Task SavePublicationStatus(CatalogueItemId catalogueItemId, CatalogueItemId additionalServiceId, PublicationStatus publicationStatus);
    }
}
