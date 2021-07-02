using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.Services
{
    public interface ICatalogueItemRepository : IDbRepository<CatalogueItem, GPITBuyingCatalogueDbContext>
    {
        Task<CatalogueItemId> GetLatestCatalogueItemIdFor(string supplierId);

        Task<bool> SupplierHasSolutionName(string supplierId, string solutionName);
    }
}
