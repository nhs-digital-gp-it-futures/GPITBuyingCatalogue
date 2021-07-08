using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;

namespace NHSD.GPIT.BuyingCatalogue.Services.AssociatedServices
{
    public sealed class AssociatedServicesService : IAssociatedServicesService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public AssociatedServicesService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Task<List<CatalogueItem>> GetAssociatedServicesForSupplier(string supplierId)
        {
            return dbContext.CatalogueItems
                .Include(c => c.Solution)
                .ThenInclude(s => s.SolutionCapabilities)
                .ThenInclude(sc => sc.Capability)
                .Include(c => c.Supplier)
                .Include(c => c.AssociatedService)
                .Where(
                    c => c.SupplierId == supplierId
                        && c.CatalogueItemType == CatalogueItemType.AssociatedService
                        && c.PublishedStatus == PublicationStatus.Published)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
    }
}
