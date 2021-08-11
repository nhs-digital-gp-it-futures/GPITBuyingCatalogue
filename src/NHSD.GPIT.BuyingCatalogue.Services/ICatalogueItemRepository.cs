using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.Services
{
    public interface ICatalogueItemRepository : IDbRepository<CatalogueItem, BuyingCatalogueDbContext>
    {
        Task<CatalogueItemId> GetLatestCatalogueItemIdFor(int supplierId);

        Task<bool> SupplierHasSolutionName(int supplierId, string solutionName);
    }
}
