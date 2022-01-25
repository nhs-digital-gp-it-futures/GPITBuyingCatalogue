using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;

namespace NHSD.GPIT.BuyingCatalogue.Services.Capabilities
{
    public sealed class SupplierDefinedEpicsService : ISupplierDefinedEpicsService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public SupplierDefinedEpicsService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Task<List<Epic>> GetSupplierDefinedEpics()
            => dbContext
                .Epics
                .AsNoTracking()
                .Include(e => e.Capability)
                .Where(e => e.SupplierDefined)
                .ToListAsync();
    }
}
