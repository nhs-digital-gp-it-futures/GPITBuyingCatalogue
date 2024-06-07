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
             Dictionary<int, string[]> capabilitiesAndEpics);

        Task<(IList<CatalogueItem> CatalogueItems, PageOptions Options, List<CapabilitiesAndCountModel> CapabilitiesAndCount)> GetAllSolutionsFiltered(
             PageOptions options = null,
             Dictionary<int, string[]> capabilitiesAndEpics = null,
             string search = null,
             string selectedFrameworkId = null,
             string selectedApplicationTypeIds = null,
             string selectedHostingTypeIds = null,
             string selectedIm1Integrations = null,
             string selectedGpConnectIntegrations = null,
             string selectedNhsAppIntegrations = null,
             string selectedInteroperabilityOptions = null);

        Task<IList<CatalogueItem>> GetAllSolutionsFilteredFromFilterIds(
            FilterIdsModel filterIds);

        Task<List<SearchFilterModel>> GetSolutionsBySearchTerm(string searchTerm, int maxToBringBack = 15);
    }
}
