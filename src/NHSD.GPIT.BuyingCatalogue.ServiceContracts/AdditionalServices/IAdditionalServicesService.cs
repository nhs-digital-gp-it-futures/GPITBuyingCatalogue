using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditonalServices
{
    public interface IAdditionalServicesService
    {
        Task<List<CatalogueItem>> GetAdditionalServicesBySolutionIds(IEnumerable<CatalogueItemId> solutionIds);

        Task<AdditionalService> GetAdditionalService(CatalogueItemId catalogueItemId);
    }
}
