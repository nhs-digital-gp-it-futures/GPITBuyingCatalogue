using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AssociatedServices;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices
{
    public interface IAssociatedServicesService
    {
        Task<List<CatalogueItem>> GetAllAssociatedServicesForSupplier(int? supplierId);

        Task<List<CatalogueItem>> GetPublishedAssociatedServicesForSupplier(int? supplierId);

        Task<List<CatalogueItem>> GetPublishedAssociatedServicesForSolution(CatalogueItemId? catalogueItemId, bool excludeMergersAndSplits);

        Task<CatalogueItem> GetAssociatedService(CatalogueItemId associatedServiceId);

        Task<CatalogueItem> GetAssociatedServiceWithCataloguePrices(CatalogueItemId associatedServiceId);

        Task<bool> AssociatedServiceExistsWithNameForSupplier(
            string additionalServiceName,
            int supplierId,
            CatalogueItemId currentCatalogueItemId = default);

        Task<List<SolutionMergerAndSplitTypesModel>> GetSolutionsWithMergerAndSplitTypesForButExcludingAssociatedService(CatalogueItemId associatedServiceId);

        Task RelateAssociatedServicesToSolution(CatalogueItemId solutionId, IEnumerable<CatalogueItemId> associatedServices);

        Task RemoveServiceFromSolution(CatalogueItemId solutionId, CatalogueItemId associatedServiceId);

        Task EditDetails(CatalogueItemId associatedServiceId, AssociatedServicesDetailsModel model);

        Task<CatalogueItemId> AddAssociatedService(CatalogueItem solution, AssociatedServicesDetailsModel model);

        Task<List<CatalogueItem>> GetAllSolutionsForAssociatedService(CatalogueItemId associatedServiceId);
    }
}
