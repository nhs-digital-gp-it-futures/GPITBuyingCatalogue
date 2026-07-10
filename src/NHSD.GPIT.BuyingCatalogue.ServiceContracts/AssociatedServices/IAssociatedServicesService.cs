using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AssociatedServices;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices
{
    public interface IAssociatedServicesService
    {
        Task<List<AssociatedService>> GetAllAssociatedServicesForSupplier(int supplierId);

        Task<List<AssociatedService>> GetPublishedAssociatedServicesForSupplier(int supplierId);

        Task<List<CatalogueItem>> GetPublishedAssociatedServicesForCatalogueItem(CatalogueItemId? catalogueItemId, PracticeReorganisationTypeEnum? practiceReorganisationType = null);

        Task<CatalogueItem> GetAssociatedService(CatalogueItemId associatedServiceId);

        Task<CatalogueItem> GetAssociatedServiceWithCataloguePrices(CatalogueItemId associatedServiceId);

        Task<bool> AssociatedServiceExistsWithNameForSupplier(
            string additionalServiceName,
            int supplierId,
            CatalogueItemId currentCatalogueItemId = default);

        Task<List<SolutionMergerAndSplitTypesModel>> GetSolutionsWithMergerAndSplitTypesForButExcludingAssociatedService(CatalogueItemId associatedServiceId);

        Task RelateAssociatedServicesToCatalogueItem(CatalogueItemId solutionId, IEnumerable<CatalogueItemId> associatedServices);

        Task RemoveServiceFromSolution(CatalogueItemId solutionId, CatalogueItemId associatedServiceId);

        Task EditDetails(CatalogueItemId associatedServiceId, AssociatedServicesDetailsModel model);

        Task<CatalogueItemId> AddAssociatedService(int supplierId, AssociatedServicesDetailsModel model);

        Task<List<CatalogueItem>> GetAssociatedServiceReferences(CatalogueItemId associatedServiceId);

        Task<IDictionary<CatalogueItemId, int>> GetCountOfAssociatedServicesForCatalogueItems(HashSet<CatalogueItemId> catalogueItems);
    }
}
