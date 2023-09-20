using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public sealed class SupplierTemporalService : ISupplierTemporalService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public SupplierTemporalService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        [ExcludeFromCodeCoverage(Justification = "Method uses Temporal tables which the In-Memory provider doesn't support.")]
        public async Task<Supplier> GetSupplierByDate(int id, DateTime dateTime)
        {
            return await dbContext.Suppliers
                .TemporalAsOf(dateTime)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
