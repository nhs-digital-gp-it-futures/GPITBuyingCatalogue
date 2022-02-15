using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public interface ISolutionsFilterService
    {
        Task<PagedList<CatalogueItem>> GetAllSolutionsFiltered(
            PageOptions options = null,
            string frameworkId = null,
            string selectedCapabilities = null,
            string search = null);

        Task<List<KeyValuePair<Framework, int>>> GetAllFrameworksAndCountForFilter();

        Task<CategoryFilterModel> GetAllCategoriesAndCountForFilter(string frameworkId = null);

        Task<List<SearchFilterModel>> GetSolutionsBySearchTerm(string searchTerm, int maxToBringBack = 15);
    }
}
