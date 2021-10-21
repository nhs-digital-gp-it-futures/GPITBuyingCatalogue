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
                Name = model.Name,
                AdditionalService = new()
                {
                    FullDescription = model.Description,
                    LastUpdated = DateTime.UtcNow,
                    LastUpdatedBy = model.UserId,
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
            additionalService.AdditionalService.LastUpdated = DateTime.UtcNow;
            additionalService.AdditionalService.LastUpdatedBy = model.UserId;

            await dbContext.SaveChangesAsync();
        }

        public Task<CatalogueItem> GetAdditionalService(CatalogueItemId catalogueItemId, CatalogueItemId additionalServiceId)
            => BaseQuery(catalogueItemId)
                .SingleAsync(i => i.Id == additionalServiceId);

        public Task<List<CatalogueItem>> GetAdditionalServicesBySolutionId(CatalogueItemId catalogueItemId)
            => BaseQuery(catalogueItemId)
                .ToListAsync();

        public Task<List<CatalogueItem>> GetAdditionalServicesBySolutionIds(IEnumerable<CatalogueItemId> solutionIds)
        {
            return dbContext.CatalogueItems
                .Include(i => i.CatalogueItemCapabilities).ThenInclude(sc => sc.Capability)
                .Include(i => i.Supplier)
                .Include(i => i.AdditionalService)
                .Where(i => solutionIds.Contains(i.AdditionalService.SolutionId)
                    && i.CatalogueItemType == CatalogueItemType.AdditionalService
                    && i.PublishedStatus == PublicationStatus.Published)
                .OrderBy(i => i.Name)
                .ToListAsync();
        }

        private IQueryable<CatalogueItem> BaseQuery(CatalogueItemId catalogueItemId) => dbContext.CatalogueItems
                .Include(i => i.AdditionalService)
                .Include(i => i.CatalogueItemCapabilities)
                .Include(i => i.Supplier)
                .Where(i => i.AdditionalService.SolutionId == catalogueItemId && i.CatalogueItemType == CatalogueItemType.AdditionalService)
                .OrderBy(i => i.Name);
    }
}
