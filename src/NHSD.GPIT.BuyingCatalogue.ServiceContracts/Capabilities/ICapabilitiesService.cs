using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities
{
    public interface ICapabilitiesService
    {
        Task<List<Capability>> GetCapabilities();

        Task<List<Capability>> GetReferencedCapabilities();

        Task<List<Capability>> GetCapabilitiesByIds(IEnumerable<int> capabilityIds);

        Task<List<CapabilityCategory>> GetCapabilitiesByCategory();

        Task AddCapabilitiesToCatalogueItem(CatalogueItemId catalogueItemId, SaveCatalogueItemCapabilitiesModel model);

        Task<Dictionary<string, IOrderedEnumerable<Epic>>> GetGroupedCapabilitiesAndEpics(Dictionary<int, string[]> ids);
    }
}
