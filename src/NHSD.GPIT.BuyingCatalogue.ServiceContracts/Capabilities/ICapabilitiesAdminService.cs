using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageCapability;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;

public interface ICapabilitiesAdminService
{
    Task<PagedList<AdminManageCapability>> GetPagedCapabilitiesAsync(PageOptions options, string search);

    Task<Capability> GetCapabilityAsync(int capabilityId);

    Task UpdateCapabilityAsync(UpdateAdminCapability request);
}
