using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.SolutionsFilterModels;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public interface ISolutionsFilterService
    {
        Task<(IList<CatalogueItem> CatalogueItems, PageOptions Options, List<CapabilitiesAndCountModel> CapabilitiesAndCount)> GetAllSolutionsFiltered(
            PageOptions options = null,
            string selectedCapabilityIds = null,
            string selectedEpicIds = null,
            string search = null,
            string selectedFrameworkId = null);

        Task<List<SearchFilterModel>> GetSolutionsBySearchTerm(string searchTerm, int maxToBringBack = 15);
    }
}
