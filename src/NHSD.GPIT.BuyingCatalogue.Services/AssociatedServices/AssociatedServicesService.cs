using System;
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

        public async Task<List<CatalogueItem>> GetPublishedAssociatedServicesForSolution(CatalogueItemId? catalogueItemId, PracticeReorganisationTypeEnum? practiceReorganisationType = null)
        {
            if (catalogueItemId is null)
            {
                return Enumerable.Empty<CatalogueItem>().ToList();
            }

            var query = dbContext.SupplierServiceAssociations
                .Include(x => x.AssociatedService)
                .ThenInclude(x => x.CatalogueItem)
                .Where(x => x.CatalogueItemId == catalogueItemId);

            if (practiceReorganisationType.HasValue)
            {
                if (practiceReorganisationType == PracticeReorganisationTypeEnum.None)
                {
                    query = query.Where(ssa => ssa.AssociatedService.PracticeReorganisationType == practiceReorganisationType);
                }
                else
                {
                    query = query.Where(ssa => ssa.AssociatedService.PracticeReorganisationType.HasFlag(practiceReorganisationType.Value));
                }
            }

            return await query.Select(x => x.AssociatedService.CatalogueItem)
                .Where(x => x.PublishedStatus == PublicationStatus.Published)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        public Task<CatalogueItem> GetAssociatedService(CatalogueItemId associatedServiceId)
        {
            return dbContext.CatalogueItems
                .Include(i => i.AssociatedService)
                .Include(i => i.Supplier)
                .Where(i => i.Id == associatedServiceId)
                .FirstOrDefaultAsync();
        }

        public async Task<CatalogueItem> GetAssociatedServiceWithCataloguePrices(CatalogueItemId associatedServiceId)
            => await dbContext.CatalogueItems
                .Include(i => i.AssociatedService)
                .Include(i => i.Supplier)
                .Include(i => i.CataloguePrices)
                .ThenInclude(cp => cp.CataloguePriceTiers)
                .Include(i => i.CataloguePrices)
                .ThenInclude(cp => cp.PricingUnit)
                .FirstOrDefaultAsync(i => i.Id == associatedServiceId);

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

        public async Task<List<SolutionMergerAndSplitTypesModel>> GetSolutionsWithMergerAndSplitTypesForButExcludingAssociatedService(CatalogueItemId associatedServiceId)
        {
            return await dbContext.CatalogueItems
                .Where(i => i.SupplierServiceAssociations.Any(ssa => ssa.AssociatedServiceId == associatedServiceId))
                .Select(s =>
                    new SolutionMergerAndSplitTypesModel(
                            s.Name,
                            s.SupplierServiceAssociations
                                .Where(ssa => ssa.AssociatedServiceId != associatedServiceId && ssa.AssociatedService.PracticeReorganisationType != PracticeReorganisationTypeEnum.None)
                                .Select(ssa => ssa.AssociatedService.PracticeReorganisationType)))
                .ToListAsync();
        }

        public async Task RelateAssociatedServicesToSolution(
            CatalogueItemId solutionId,
            IEnumerable<CatalogueItemId> associatedServices)
        {
            var solution = await dbContext.CatalogueItems
                .Include(i => i.SupplierServiceAssociations)
                .Where(i => i.Id == solutionId)
                .FirstAsync();

            solution.SupplierServiceAssociations.Clear();

            solution.SupplierServiceAssociations = associatedServices.Select(a => new SupplierServiceAssociation(solutionId, a)).ToList();

            await dbContext.SaveChangesAsync();
        }

        public async Task RemoveServiceFromSolution(CatalogueItemId solutionId, CatalogueItemId associatedServiceId)
        {
            var associatedServiceAssociation = await dbContext.SupplierServiceAssociations.AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.CatalogueItemId == solutionId && x.AssociatedServiceId == associatedServiceId);

            if (associatedServiceAssociation is null) return;

            dbContext.SupplierServiceAssociations.Remove(associatedServiceAssociation);
            await dbContext.SaveChangesAsync();
        }

        public async Task<List<CatalogueItem>> GetAllSolutionsForAssociatedService(CatalogueItemId associatedServiceId)
            => await dbContext
                .SupplierServiceAssociations
                .Where(ssa => ssa.AssociatedServiceId == associatedServiceId)
                .Select(ssa => ssa.CatalogueItem)
                .ToListAsync();

        public async Task<CatalogueItemId> AddAssociatedService(
            CatalogueItem solution,
            AssociatedServicesDetailsModel model)
        {
            ArgumentNullException.ThrowIfNull(solution);

            ArgumentNullException.ThrowIfNull(model);

            var associatedService = new CatalogueItem
            {
                Name = model.Name,
                AssociatedService = new AssociatedService
                {
                    Description = model.Description,
                    OrderGuidance = model.OrderGuidance,
                    PracticeReorganisationType = model.PracticeReorganisationType,
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
            ArgumentNullException.ThrowIfNull(model);

            var associatedService = await dbContext.CatalogueItems
                .Include(i => i.AssociatedService)
                .Where(i => i.Id == associatedServiceId)
                .FirstAsync();

            associatedService.Name = model.Name;
            associatedService.AssociatedService.Description = model.Description;
            associatedService.AssociatedService.OrderGuidance = model.OrderGuidance;
            associatedService.AssociatedService.PracticeReorganisationType = model.PracticeReorganisationType;

            await dbContext.SaveChangesAsync();
        }
    }
}
