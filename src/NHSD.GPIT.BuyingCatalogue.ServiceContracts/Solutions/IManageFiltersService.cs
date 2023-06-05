using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public interface IManageFiltersService
    {
        Task<int> AddFilter(
            string name,
            string description,
            int organisationId,
            List<int> capabilityIds,
            List<string> epicIds,
            string frameworkId,
            List<ClientApplicationType> clientApplicationTypes,
            List<HostingType> hostingTypes);

        Task<bool> FilterExists(string filterName, int organisationId);

        Task<List<Filter>> GetFilters(int organisationId);

        Task<FilterDetailsModel> GetFilterDetails(int organisationId, int filterId);

        Task<FilterIdsModel> GetFilterIds(int organisationId, int filterId);

        Task DeleteFilter(int filterId);
    }
}
