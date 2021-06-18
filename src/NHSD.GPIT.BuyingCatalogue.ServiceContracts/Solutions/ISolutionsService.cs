using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public interface ISolutionsService
    {
        Task<List<CatalogueItem>> GetFuturesFoundationSolutions();

        Task<List<CatalogueItem>> GetFuturesSolutionsByCapabilities(string[] capabilities);

        Task<CatalogueItem> GetSolution(string solutionId);

        Task<List<CatalogueItem>> GetDFOCVCSolutions();

        Task<List<Capability>> GetFuturesCapabilities();

        Task SaveSolutionDescription(string solutionId, string summary, string description, string link);

        Task SaveSolutionFeatures(string solutionId, string[] features);

        Task SaveIntegrationLink(string solutionId, string integrationLink);

        Task SaveImplementationDetail(string solutionId, string detail);

        Task SaveRoadmap(string solutionId, string roadmap);

        Task<ClientApplication> GetClientApplication(string solutionId);

        Task SaveClientApplication(string solutionId, ClientApplication clientApplication);

        Task<Hosting> GetHosting(string solutionId);

        Task SaveHosting(string solutionId, Hosting hosting);

        Task<Supplier> GetSupplier(string supplierId);

        Task SaveSupplierDescriptionAndLink(string supplierId, string description, string link);

        Task SaveSupplierContacts(SupplierContactsModel model);

        Task<List<CatalogueItem>> GetSupplierSolutions(string supplierId);

        Task<List<CatalogueItem>> GetAllSolutions();
    }
}
