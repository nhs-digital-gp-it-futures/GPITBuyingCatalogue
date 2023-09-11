using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
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
            Dictionary<int, string[]> capabilityAndEpicIds,
            string frameworkId,
            List<ApplicationType> applicationTypes,
            List<HostingType> hostingTypes,
            List<InteropIm1IntegrationType> iM1IntegrationsTypes,
            List<InteropGpConnectIntegrationType> gPConnectIntegrationsTypes,
            List<InteropIntegrationType> interoperabilityIntegrationTypes);

        Task<bool> FilterExists(string filterName, int organisationId);

        Task<List<Filter>> GetFilters(int organisationId);

        Task<FilterDetailsModel> GetFilterDetails(int organisationId, int filterId);

        Task<FilterIdsModel> GetFilterIds(int organisationId, int filterId);

        Task DeleteFilter(int filterId);
    }
}
