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
    }
}
