using System;
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

        public async Task<CatalogueItemId> GetLatestCatalogueItemIdFor(string supplierId)
        {
            if (!int.TryParse(supplierId, out var supplierIdentifier))
                throw new ArgumentException($"'{supplierId}' is not a valid Supplier Id");

            var catalogueSolution = await DbSet
                .Where(i => i.CatalogueItemType == CatalogueItemType.Solution && i.SupplierId == supplierId)
                .OrderByDescending(i => i.CatalogueItemId)
                .FirstOrDefaultAsync();

            return catalogueSolution?.CatalogueItemId ?? new CatalogueItemId(supplierIdentifier, "000");
        }

        public Task<bool> SupplierHasSolutionName(string supplierId, string solutionName) =>
            DbSet.AnyAsync(i => i.SupplierId == supplierId && i.Name == solutionName);
    }
}
