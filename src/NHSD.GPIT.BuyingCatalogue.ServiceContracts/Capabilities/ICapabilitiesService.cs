using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities
{
    public interface ICapabilitiesService
    {
        Task<List<CapabilityCategory>> GetCapabilitiesByCategory();

        Task AddCapabilitiesToCatalogueItem(CatalogueItemId catalogueItemId, SaveCatalogueItemCapabilitiesModel model);
    }
}
