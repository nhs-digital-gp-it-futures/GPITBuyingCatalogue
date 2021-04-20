using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public interface ISolutionsService
    {
        Task<List<CatalogueItem>> GetFuturesFoundationSolutions();

        Task<List<CatalogueItem>> GetFuturesSolutionsByCapabilities(string[] capabilities);

        Task<CatalogueItem> GetSolution(string id);

        Task<List<CatalogueItem>> GetDFOCVCSolutions();
        
        Task<List<Capability>> GetFuturesCapabilities();

        Task SaveSolutionDescription(string id, string summary, string description, string link);

        Task SaveSolutionFeatures(string id, string featuresJson);

        Task SaveIntegrationLink(string id, string integrationLink);

        Task SaveImplementationDetail(string id, string detail);

        Task SaveRoadmap(string id, string roadmap);

        Task<ClientApplication> GetClientApplication(string solutionId);

        Task SaveClientApplication(string solutionId, ClientApplication clientApplication);

        Task<Hosting> GetHosting(string solutionId);

        Task SaveHosting(string solutionId, Hosting hosting);

        Task<Supplier> GetSupplier(string supplierId);

        Task SaveSupplierDescriptionAndLink(string supplierId, string description, string link);
    }
}
