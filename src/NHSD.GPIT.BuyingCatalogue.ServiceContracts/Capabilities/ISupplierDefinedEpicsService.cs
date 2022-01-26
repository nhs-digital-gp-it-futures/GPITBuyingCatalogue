using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.SupplierDefinedEpics;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities
{
    public interface ISupplierDefinedEpicsService
    {
        Task<List<Epic>> GetSupplierDefinedEpics();

        Task AddSupplierDefinedEpic(AddEditSupplierDefinedEpic epicModel);

        Task<bool> EpicExists(string id, int capabilityId, string name, string description, bool isActive);
    }
}
