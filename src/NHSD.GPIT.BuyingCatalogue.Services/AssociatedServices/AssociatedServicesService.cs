﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AssociatedServices;

namespace NHSD.GPIT.BuyingCatalogue.Services.AssociatedServices
{
    public sealed class AssociatedServicesService : IAssociatedServicesService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public AssociatedServicesService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Task<List<CatalogueItem>> GetAllAssociatedServicesForSupplier(int? supplierId)
        {
            return dbContext.CatalogueItems
                .Include(s => s.CatalogueItemCapabilities)
                .ThenInclude(sc => sc.Capability)
                .Include(c => c.Supplier)
                .Include(c => c.AssociatedService)
                .Where(
                    c => c.SupplierId == supplierId.GetValueOrDefault()
                        && c.CatalogueItemType == CatalogueItemType.AssociatedService)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public Task<List<CatalogueItem>> GetPublishedAssociatedServicesForSupplier(int? supplierId)
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

        // checks to see if this associated services' name is unique for the supplier
        public Task<bool> AssociatedServiceExistsWithNameForSupplier(
            string additionalServiceName,
            int supplierId,
            CatalogueItemId currentCatalogueItemId = default) =>
            dbContext
                .AssociatedServices
                .AnyAsync(asoc =>
                    asoc.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService
                    && asoc.CatalogueItem.SupplierId == supplierId
                    && asoc.CatalogueItem.Name == additionalServiceName
                    && asoc.CatalogueItemId != currentCatalogueItemId);

        public async Task RelateAssociatedServicesToSolution(
            CatalogueItemId solutionId,
            IEnumerable<CatalogueItemId> associatedServices)
        {
            var solution = await dbContext.CatalogueItems
                .Include(i => i.SupplierServiceAssociations)
                .Where(i => i.Id == solutionId)
                .SingleAsync();

            solution.SupplierServiceAssociations.Clear();

            solution.SupplierServiceAssociations = associatedServices.Select(a => new SupplierServiceAssociation(solutionId, a)).ToList();

            await dbContext.SaveChangesAsync();
        }

        public async Task<List<CatalogueItem>> GetAllSolutionsForAssociatedService(CatalogueItemId currentSolutionId, CatalogueItemId associatedServiceId)
            => await dbContext
                .SupplierServiceAssociations
                .Where(ssa => ssa.AssociatedServiceId == associatedServiceId && ssa.CatalogueItemId != currentSolutionId)
                .Select(ssa => ssa.CatalogueItem).ToListAsync();

        public async Task<CatalogueItemId> AddAssociatedService(
            CatalogueItem solution,
            AssociatedServicesDetailsModel model)
        {
            if (solution is null)
                throw new ArgumentNullException(nameof(solution));

            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var associatedService = new CatalogueItem
            {
                Name = model.Name,
                AssociatedService = new AssociatedService
                {
                    Description = model.Description,
                    OrderGuidance = model.OrderGuidance,
                },
                CatalogueItemType = CatalogueItemType.AssociatedService,
                SupplierId = solution.SupplierId,
                PublishedStatus = PublicationStatus.Draft,
            };

            dbContext.Add(associatedService);
            await dbContext.SaveChangesAsync();

            return associatedService.Id;
        }

        public async Task EditDetails(
            CatalogueItemId associatedServiceId,
            AssociatedServicesDetailsModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

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
