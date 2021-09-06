using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public interface ISolutionsFilterService
    {
        Task<PagedList<CatalogueItem>> GetAllSolutionsFiltered(
            PageOptions options = null,
            string frameworkId = null,
            string selectedCapabilites = null);

        Task<Dictionary<Framework, int>> GetAllFrameworksAndCountForFilter();

        Task<CategoryFilterModel> GetAllCategoriesAndCountForFilter(string frameworkId = null);
    }
}
