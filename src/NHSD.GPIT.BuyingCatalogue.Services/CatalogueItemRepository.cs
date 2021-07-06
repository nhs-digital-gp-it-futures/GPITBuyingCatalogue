using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.Services
{
    public class CatalogueItemRepository : DbRepository<CatalogueItem, GPITBuyingCatalogueDbContext>, ICatalogueItemRepository
    {
        public CatalogueItemRepository(GPITBuyingCatalogueDbContext dbContext)
            : base(dbContext)
        {
        }

        public async Task<CatalogueItemId> GetLatestCatalogueItemIdFor(string supplierId)
        {
            if (!int.TryParse(supplierId, out var supplierIdentifier))
                throw new ArgumentException($"'{supplierId}' is not a valid Supplier Id");

            var catalogueSolution = await DbSet
                .Where(i => i.CatalogueItemType == CatalogueItemType.Solution && i.SupplierId == supplierId)
                .OrderByDescending(i => i.Created)
                .FirstOrDefaultAsync();

            return catalogueSolution?.CatalogueItemId ?? new CatalogueItemId(supplierIdentifier, "000");
        }

        public Task<bool> SupplierHasSolutionName(string supplierId, string solutionName) =>
            DbSet.AnyAsync(i => i.SupplierId == supplierId && i.Name == solutionName);
    }
}
