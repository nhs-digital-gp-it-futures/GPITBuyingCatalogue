using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditonalServices
{
    public interface IAdditionalServicesService
    {
        Task<List<CatalogueItem>> GetAdditionalServicesBySolutionIds(IEnumerable<string> solutionIds);

        Task<AdditionalService> GetAdditionalService(string catalogueItemId);
    }
}
