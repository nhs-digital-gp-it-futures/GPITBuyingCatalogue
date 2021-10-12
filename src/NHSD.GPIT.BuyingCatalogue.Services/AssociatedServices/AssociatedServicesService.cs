using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AssociatedServices;

namespace NHSD.GPIT.BuyingCatalogue.Services.AssociatedServices
{
    public sealed class AssociatedServicesService : IAssociatedServicesService
    {
        private readonly BuyingCatalogueDbContext dbContext;
        private readonly ICatalogueItemRepository catalogueItemRepository;

        public AssociatedServicesService(
            BuyingCatalogueDbContext dbContext,
            ICatalogueItemRepository catalogueItemRepository)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.catalogueItemRepository = catalogueItemRepository ?? throw new ArgumentNullException(nameof(catalogueItemRepository));
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

        public Task<CatalogueItem> GetAssociatedService(CatalogueItemId associatedServiceId)
        {
            return dbContext.CatalogueItems
                .Include(i => i.AssociatedService)
                .Include(i => i.Supplier)
                .Include(i => i.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Where(i => i.Id == associatedServiceId)
                .FirstOrDefaultAsync();
        }

        public Task DeleteAssociatedService(CatalogueItemId associatedServiceId)
        {
            // MJRTODO
            return Task.CompletedTask;
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

        public async Task<CatalogueItemId> AddAssociatedService(
            CatalogueItem solution,
            AssociatedServicesDetailsModel model)
        {
            model.ValidateNotNull(nameof(AssociatedServicesDetailsModel));

            var latestAssociatedServiceCatalogueItemId = await catalogueItemRepository.GetLatestAssociatedServiceCatalogueItemIdFor(solution.SupplierId);
            var catalogueItemId = latestAssociatedServiceCatalogueItemId.NextAssociatedServiceId();

            var associatedService = new CatalogueItem
            {
                Id = catalogueItemId,
                Name = model.Name,
                AssociatedService = new AssociatedService
                {
                    Description = model.Description,
                    OrderGuidance = model.OrderGuidance,
                    LastUpdated = DateTime.UtcNow,
                    LastUpdatedBy = model.UserId,
                },
                CatalogueItemType = CatalogueItemType.AssociatedService,
                SupplierId = solution.SupplierId,
                PublishedStatus = PublicationStatus.Draft,
            };

            dbContext.Add(associatedService);
            await dbContext.SaveChangesAsync();

            return catalogueItemId;
        }

        public async Task EditDetails(
            CatalogueItemId associatedServiceId,
            AssociatedServicesDetailsModel model)
        {
            model.ValidateNotNull(nameof(AssociatedServicesDetailsModel));

            var associatedService = await dbContext.CatalogueItems
                .Include(i => i.AssociatedService)
                .Where(i => i.Id == associatedServiceId)
                .SingleAsync();

            associatedService.Name = model.Name;
            associatedService.AssociatedService.Description = model.Description;
            associatedService.AssociatedService.OrderGuidance = model.OrderGuidance;

            await dbContext.SaveChangesAsync();
        }
    }
}
