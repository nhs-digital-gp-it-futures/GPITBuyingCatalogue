using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageEpic;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Epics;

public interface IEpicsAdminService
{
    Task<PagedList<AdminManageEpic>> GetPagedEpicsAsync(PageOptions options, string search);

    Task<Epic> GetEpicAsync(string epicId);

    Task UpdateEpicAsync(UpdateAdminEpic request);
}
