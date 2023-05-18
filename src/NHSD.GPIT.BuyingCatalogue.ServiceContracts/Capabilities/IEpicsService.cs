using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities
{
    public interface IEpicsService
    {
        Task<List<Epic>> GetEpicsByIds(IEnumerable<string> epicIds);

        Task<List<Epic>> GetReferencedEpicsByCapabilityIds(IEnumerable<int> capabilityIds);

        Task<string> GetEpicsForSelectedCapabilities(IEnumerable<int> capabilityIds, IEnumerable<string> epicIds);
    }
}
