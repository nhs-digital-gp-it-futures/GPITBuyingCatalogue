using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities
{
    public interface ISupplierDefinedEpicsService
    {
        Task<List<Epic>> GetSupplierDefinedEpics();
    }
}
