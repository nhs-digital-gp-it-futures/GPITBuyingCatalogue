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

        Task EditSupplierDefinedEpic(AddEditSupplierDefinedEpic epicModel);

        Task<bool> EpicExists(string epicId, int capabilityId, string name, string description, bool isActive);

        Task<Epic> GetEpic(string epicId);

        Task<List<CatalogueItem>> GetItemsReferencingEpic(string epicId);
    }
}
