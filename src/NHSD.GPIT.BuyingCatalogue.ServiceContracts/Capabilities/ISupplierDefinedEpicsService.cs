using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.SupplierDefinedEpics;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities
{
    public interface ISupplierDefinedEpicsService
    {
        Task<List<Epic>> GetSupplierDefinedEpics();

        Task<List<Epic>> GetSupplierDefinedEpicsBySearchTerm(string searchTerm);

        Task<string> AddSupplierDefinedEpic(AddEditSupplierDefinedEpic epicModel);

        Task EditSupplierDefinedEpic(AddEditSupplierDefinedEpic epicModel);

        Task DeleteSupplierDefinedEpic(string epicId);

        Task<bool> EpicExists(string epicId, string name, string description, bool isActive);

        Task<bool> EpicWithNameExists(string epicId, string name);

        Task<Epic> GetEpic(string epicId);

        Task<List<CatalogueItem>> GetItemsReferencingEpic(string epicId);
    }
}
