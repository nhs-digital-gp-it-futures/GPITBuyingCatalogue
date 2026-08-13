using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageCapability;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;

public interface ICapabilitiesAdminService
{
    Task<PagedList<AdminManageCapability>> GetPagedCapabilities(PageOptions options, string search);

    Task<Capability> GetCapability(int capabilityId);

    Task UpdateCapability(UpdateAdminCapability request);
}
