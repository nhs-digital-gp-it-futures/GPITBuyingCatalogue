using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices
{
    public interface IAssociatedServicesService
    {
        Task<List<CatalogueItem>> GetAssociatedServicesForSupplier(int? supplierId);

        Task RelateAssociatedServicesToSolution(CatalogueItemId solutionId, IEnumerable<CatalogueItemId> associatedServices);
    }
}
