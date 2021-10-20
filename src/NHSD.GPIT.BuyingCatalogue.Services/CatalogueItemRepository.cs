using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.Services
{
    public sealed class CatalogueItemRepository : DbRepository<CatalogueItem, BuyingCatalogueDbContext>, ICatalogueItemRepository
    {
        public CatalogueItemRepository(BuyingCatalogueDbContext dbContext)
            : base(dbContext)
        {
        }

        public Task<bool> SupplierHasSolutionName(int supplierId, string solutionName) =>
            DbSet.AnyAsync(i => i.SupplierId == supplierId && i.Name == solutionName);
    }
}
