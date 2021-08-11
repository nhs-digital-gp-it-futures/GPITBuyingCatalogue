using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public interface ISolutionsService
    {
        Task<List<CatalogueItem>> GetFuturesFoundationSolutions();

        Task<List<CatalogueItem>> GetFuturesSolutionsByCapabilities(string[] capabilities);

        Task<CatalogueItem> GetSolutionListPrices(CatalogueItemId solutionId);

        Task<CatalogueItem> GetSolution(CatalogueItemId solutionId);

        Task<CatalogueItem> GetSolutionByName(string solutionName);

        Task<CatalogueItem> GetSolutionCapability(CatalogueItemId catalogueItemId, Guid capabilityId);

        Task<CatalogueItem> GetSolutionWithAllAssociatedServices(CatalogueItemId solutionId);

        Task<CatalogueItem> GetSolutionOverview(CatalogueItemId solutionId);

        Task<CatalogueItem> GetSolutionWithAllAdditionalServices(CatalogueItemId solutionId);

        Task<List<CatalogueItem>> GetDFOCVCSolutions();

        Task<List<Capability>> GetFuturesCapabilities();

        Task<IList<Supplier>> GetAllSuppliers();

        Task SaveSolutionDescription(CatalogueItemId solutionId, string summary, string description, string link);

        Task SaveSolutionFeatures(CatalogueItemId solutionId, string[] features);

        Task SaveImplementationDetail(CatalogueItemId solutionId, string detail);

        Task SaveRoadMap(CatalogueItemId solutionId, string roadMap);

        Task<ClientApplication> GetClientApplication(CatalogueItemId solutionId);

        Task SaveClientApplication(CatalogueItemId solutionId, ClientApplication clientApplication);

        Task<Hosting> GetHosting(CatalogueItemId solutionId);

        Task SaveHosting(CatalogueItemId solutionId, Hosting hosting);

        Task<Supplier> GetSupplier(int supplierId);

        Task SaveSupplierDescriptionAndLink(int supplierId, string description, string link);

        Task SaveSupplierContacts(SupplierContactsModel model);

        Task<List<CatalogueItem>> GetSupplierSolutions(int? supplierId);

        Task<IList<CatalogueItem>> GetAllSolutions(PublicationStatus? publicationStatus = null);

        Task<CatalogueItem> GetSolutionAdditionalServiceCapabilities(CatalogueItemId id);

        Task<CatalogueItem> GetAdditionalServiceCapability(
            CatalogueItemId catalogueItemId,
            Guid capabilityId);

        Task<CatalogueItemId> AddCatalogueSolution(CreateSolutionModel model);

        Task<IList<Framework>> GetAllFrameworks();

        Task<bool> SupplierHasSolutionName(int supplierId, string solutionName);
    }
}
