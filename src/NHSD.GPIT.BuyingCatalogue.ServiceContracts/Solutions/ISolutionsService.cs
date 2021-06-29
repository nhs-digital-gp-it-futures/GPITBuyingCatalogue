using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
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

        Task<CatalogueItem> GetSolutionCapability(CatalogueItemId catalogueItemId, Guid capabilityId);

        Task<List<CatalogueItem>> GetDFOCVCSolutions();

        Task<List<Capability>> GetFuturesCapabilities();

        Task SaveSolutionDescription(CatalogueItemId solutionId, string summary, string description, string link);

        Task SaveSolutionFeatures(CatalogueItemId solutionId, string[] features);

        Task SaveIntegrationLink(CatalogueItemId solutionId, string integrationLink);

        Task SaveImplementationDetail(CatalogueItemId solutionId, string detail);

        Task SaveRoadMap(CatalogueItemId solutionId, string roadMap);

        Task<ClientApplication> GetClientApplication(CatalogueItemId solutionId);

        Task SaveClientApplication(CatalogueItemId solutionId, ClientApplication clientApplication);

        Task<Hosting> GetHosting(CatalogueItemId solutionId);

        Task SaveHosting(CatalogueItemId solutionId, Hosting hosting);

        Task<Supplier> GetSupplier(string supplierId);

        Task SaveSupplierDescriptionAndLink(string supplierId, string description, string link);

        Task SaveSupplierContacts(SupplierContactsModel model);

        Task<List<CatalogueItem>> GetSupplierSolutions(string supplierId);

        Task<IList<CatalogueItem>> GetAllSolutions();
    }
}
