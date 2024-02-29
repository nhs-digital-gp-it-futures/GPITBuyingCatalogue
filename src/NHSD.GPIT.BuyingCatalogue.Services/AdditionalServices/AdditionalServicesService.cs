using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdditionalServices;

namespace NHSD.GPIT.BuyingCatalogue.Services.AdditionalServices
{
    public sealed class AdditionalServicesService : IAdditionalServicesService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public AdditionalServicesService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<CatalogueItemId> AddAdditionalService(CatalogueItem solution, AdditionalServicesDetailsModel model)
        {
            if (solution is null)
                throw new ArgumentNullException(nameof(solution));

            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var additionalService = new CatalogueItem
            {
                Id = model.Id,
                Name = model.Name,
                AdditionalService = new()
                {
                    FullDescription = model.Description,
                    SolutionId = solution.Id,
                },
                CatalogueItemType = CatalogueItemType.AdditionalService,
                SupplierId = solution.SupplierId,
                PublishedStatus = PublicationStatus.Draft,
            };

            dbContext.Add(additionalService);
            await dbContext.SaveChangesAsync();

            return additionalService.Id;
        }

        public async Task EditAdditionalService(CatalogueItemId catalogueItemId, CatalogueItemId additionalServiceId, AdditionalServicesDetailsModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var additionalService = await GetAdditionalService(catalogueItemId, additionalServiceId);

            additionalService.Name = model.Name;
            additionalService.AdditionalService.FullDescription = model.Description;

            await dbContext.SaveChangesAsync();
        }

        public Task<CatalogueItem> GetAdditionalService(CatalogueItemId catalogueItemId, CatalogueItemId additionalServiceId)
            => BaseQuery(catalogueItemId)
                .FirstOrDefaultAsync(i => i.Id == additionalServiceId);

        public async Task<CatalogueItem> GetAdditionalServiceWithCapabilities(CatalogueItemId additionalServiceId)
            => await dbContext.CatalogueItems.AsNoTracking()
                .Include(ci => ci.AdditionalService)
                .Include(ci => ci.CatalogueItemCapabilities)
                    .ThenInclude(cic => cic.Capability)
                .Include(ci => ci.CatalogueItemEpics)
                    .ThenInclude(cie => cie.Epic)
                .FirstOrDefaultAsync(ci => ci.Id == additionalServiceId);

        public async Task<List<CatalogueItem>> GetAdditionalServicesBySolutionId(CatalogueItemId? catalogueItemId, bool publishedOnly = false)
        {
            if (!catalogueItemId.HasValue)
            {
                return Enumerable.Empty<CatalogueItem>().ToList();
            }

            return publishedOnly
                ? await BaseQuery(catalogueItemId.Value).Where(x => x.PublishedStatus == PublicationStatus.Published).ToListAsync()
                : await BaseQuery(catalogueItemId.Value).ToListAsync();
        }

        public Task<List<CatalogueItem>> GetAdditionalServicesBySolutionIds(IEnumerable<CatalogueItemId> solutionIds)
        {
            return dbContext.CatalogueItems
                .Include(i => i.CatalogueItemCapabilities).ThenInclude(sc => sc.Capability)
                .Include(i => i.CatalogueItemEpics)
                .Include(i => i.Supplier)
                .Include(i => i.AdditionalService)
                .Where(i => solutionIds.Contains(i.AdditionalService.SolutionId)
                    && i.CatalogueItemType == CatalogueItemType.AdditionalService
                    && i.PublishedStatus == PublicationStatus.Published)
                .OrderBy(i => i.Name)
                .ToListAsync();
        }

        // checks to see if this additional services' name is unique for this solution
        public Task<bool> AdditionalServiceExistsWithNameForSolution(
            string additionalServiceName,
            CatalogueItemId solutionId,
            CatalogueItemId currentCatalogueItemId = default) =>
            dbContext
                .AdditionalServices
                .AnyAsync(add =>
                    add.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService
                    && add.SolutionId == solutionId
                    && add.CatalogueItem.Name == additionalServiceName
                    && add.CatalogueItemId != currentCatalogueItemId);

        private IQueryable<CatalogueItem> BaseQuery(CatalogueItemId catalogueItemId)
        {
            return dbContext.CatalogueItems
                .Include(i => i.AdditionalService)
                .Include(i => i.CatalogueItemCapabilities)
                .Include(i => i.CatalogueItemEpics)
                .Include(i => i.Supplier)
                .Include(ci => ci.CataloguePrices).ThenInclude(cp => cp.PricingUnit)
                .Include(ci => ci.CataloguePrices).ThenInclude(cp => cp.CataloguePriceTiers)
                .AsSplitQuery()
                .Where(i => i.AdditionalService.SolutionId == catalogueItemId
                    && i.CatalogueItemType == CatalogueItemType.AdditionalService)
                .OrderBy(i => i.Name);
        }
    }
}
