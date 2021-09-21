using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
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

        public Task<List<CatalogueItem>> GetAssociatedServicesForSupplier(int? supplierId)
        {
            return dbContext.CatalogueItems
                .Include(s => s.CatalogueItemCapabilities)
                .ThenInclude(sc => sc.Capability)
                .Include(c => c.Supplier)
                .Include(c => c.AssociatedService)
                .Where(
                    c => c.SupplierId == supplierId.GetValueOrDefault()
                        && c.CatalogueItemType == CatalogueItemType.AssociatedService
                        && c.PublishedStatus == PublicationStatus.Published)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task RelateAssociatedServicesToSolution(
            CatalogueItemId solutionId,
            IEnumerable<CatalogueItemId> associatedServices)
        {
            var solution = await dbContext.CatalogueItems
                .Include(i => i.SupplierServiceAssociations)
                .Where(i => i.Id == solutionId)
                .SingleAsync();

            solution.SupplierServiceAssociations.Clear();

            solution.SupplierServiceAssociations = associatedServices.Select(a => new SupplierServiceAssociation
                {
                    CatalogueItemId = solutionId,
                    AssociatedServiceId = a,
                }).ToList();

            await dbContext.SaveChangesAsync();
        }
    }
}
