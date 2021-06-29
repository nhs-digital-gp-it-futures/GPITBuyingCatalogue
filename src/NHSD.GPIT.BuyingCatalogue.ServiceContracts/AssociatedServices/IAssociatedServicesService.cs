using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices
{
    public interface IAssociatedServicesService
    {
        Task<List<CatalogueItem>> GetAssociatedServicesForSupplier(string supplierId);
    }
}
