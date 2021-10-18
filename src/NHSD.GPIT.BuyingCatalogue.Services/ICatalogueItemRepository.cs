using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.Services
{
    public interface ICatalogueItemRepository : IDbRepository<CatalogueItem, BuyingCatalogueDbContext>
    {
        Task<bool> SupplierHasSolutionName(int supplierId, string solutionName);
    }
}
