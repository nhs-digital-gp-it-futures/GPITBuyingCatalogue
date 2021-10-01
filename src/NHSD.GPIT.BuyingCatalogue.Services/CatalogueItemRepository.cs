using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.Services
{
    public sealed class CatalogueItemRepository : DbRepository<CatalogueItem, BuyingCatalogueDbContext>, ICatalogueItemRepository
    {
        public CatalogueItemRepository(BuyingCatalogueDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<CatalogueItemId> GetLatestCatalogueItemIdFor(int supplierId)
        {
            var catalogueSolution = await GetLatestCatalogueItem(supplierId, CatalogueItemType.Solution);

            return catalogueSolution?.Id ?? new CatalogueItemId(supplierId, "000");
        }

        public async Task<CatalogueItemId> GetLatestAssociatedServiceCatalogueItemIdFor(int supplierId)
        {
            var associatedService = await GetLatestCatalogueItem(supplierId, CatalogueItemType.AssociatedService);

            return associatedService?.Id ?? new CatalogueItemId(supplierId, "S-000");
        }

        public Task<bool> SupplierHasSolutionName(int supplierId, string solutionName) =>
            DbSet.AnyAsync(i => i.SupplierId == supplierId && i.Name == solutionName);

        private async Task<CatalogueItem> GetLatestCatalogueItem(int supplierId, CatalogueItemType catalogueItemType) =>
            await DbSet
                .Where(i => i.CatalogueItemType == catalogueItemType && i.SupplierId == supplierId)
                .OrderByDescending(i => i.Id)
                .FirstOrDefaultAsync();
    }
}
