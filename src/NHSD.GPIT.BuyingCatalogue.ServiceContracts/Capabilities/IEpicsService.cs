using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities
{
    public interface IEpicsService
    {
        Task<List<Epic>> GetActiveEpicsByCapabilityIds(IEnumerable<int> capabilityIds);
    }
}
