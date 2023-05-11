using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions
{
    public interface IManageFiltersService
    {
        Task<int> SaveFilter(
            string name,
            string description,
            int organisationId,
            List<int> capabilityIds,
            List<string> epicIds,
            string frameworkId,
            List<ClientApplicationType> clientApplicationTypes,
            List<HostingType> hostingTypes);

        Task AddFilterCapabilities(int filterId, List<int> capabilityIds);

        Task AddFilterEpics(int filterId, List<string> epicIds);

        Task AddFilterClientApplicationTypes(int filterId, List<ClientApplicationType> clientApplicationTypes);

        Task AddFilterHostingTypes(int filterId, List<HostingType> hostingTypes);
        
        Task<bool> FilterExists(string filterName, int organisationId);

        Task<List<Filter>> GetFilters(int organisationId);
    }
}
