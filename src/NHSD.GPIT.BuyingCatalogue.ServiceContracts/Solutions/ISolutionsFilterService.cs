using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.SolutionsFilterModels;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public interface ISolutionsFilterService
    {
       Task<(IQueryable<CatalogueItem> CatalogueItems, List<CapabilitiesAndCountModel> CapabilitiesAndCount)> GetFilteredAndNonFilteredQueryResults(
            string selectedCapabilityIds = null,
            string selectedEpicIds = null);

       Task<(IList<CatalogueItem> CatalogueItems, PageOptions Options, List<CapabilitiesAndCountModel> CapabilitiesAndCount)> GetAllSolutionsFiltered(
            PageOptions options = null,
            string selectedCapabilityIds = null,
            string selectedEpicIds = null,
            string search = null,
            string selectedFrameworkId = null,
            string clientApplicationTypeSelected = null);

       Task<List<SearchFilterModel>> GetSolutionsBySearchTerm(string searchTerm, int maxToBringBack = 15);
    }
}
