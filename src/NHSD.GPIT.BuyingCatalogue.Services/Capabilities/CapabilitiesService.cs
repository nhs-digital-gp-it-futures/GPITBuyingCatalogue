using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.Services.Capabilities
{
    public sealed class CapabilitiesService : ICapabilitiesService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public CapabilitiesService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Task<List<CapabilityCategory>> GetCapabilitiesByCategory()
            => dbContext.CapabilityCategories.Include(c => c.Capabilities).ThenInclude(c => c.Epics).ToListAsync();

        public async Task AddCapabilitiesToCatalogueItem(CatalogueItemId catalogueItemId, SaveCatalogueItemCapabilitiesModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var catalogueItem = await dbContext.CatalogueItems
                .Include(i => i.CatalogueItemCapabilities)
                .Include(i => i.CatalogueItemEpics)
                .SingleAsync(c => c.Id == catalogueItemId);

            AddCapabilities(catalogueItem, model);
            AddCapabilityEpics(catalogueItem, model);

            await dbContext.SaveChangesAsync();
        }

        private static void AddCapabilities(CatalogueItem catalogueItem, SaveCatalogueItemCapabilitiesModel model)
        {
            var staleCapabilities = catalogueItem.CatalogueItemCapabilities.Where(capability => !model.Capabilities.Any(newCapability => newCapability.Id == capability.CapabilityId));
            staleCapabilities.ToList().ForEach(c => catalogueItem.CatalogueItemCapabilities.Remove(c));

            foreach (var (id, _) in model.Capabilities)
            {
                if (catalogueItem.CatalogueItemCapabilities.Any(c => c.CapabilityId == id))
                    continue;

                var catalogueItemCapability = new CatalogueItemCapability
                {
                    LastUpdated = DateTime.UtcNow,
                    LastUpdatedBy = model.UserId,
                    CapabilityId = id,
                    StatusId = 1,
                };

                catalogueItem.CatalogueItemCapabilities.Add(catalogueItemCapability);
            }
        }

        private static void AddCapabilityEpics(CatalogueItem catalogueItem, SaveCatalogueItemCapabilitiesModel model)
        {
            var staleEpics = new List<CatalogueItemEpic>();
            foreach (var (id, epicIds) in model.Capabilities)
            {
                staleEpics.AddRange(catalogueItem.CatalogueItemEpics.Where(epic => epic.CapabilityId == id && !epicIds.Contains(epic.EpicId)));
                foreach (var epicId in epicIds)
                {
                    if (catalogueItem.CatalogueItemEpics.Any(c => c.EpicId == epicId))
                        continue;

                    var catalogueItemEpic = new CatalogueItemEpic
                    {
                        LastUpdated = DateTime.UtcNow,
                        LastUpdatedBy = model.UserId,
                        CapabilityId = id,
                        EpicId = epicId,
                        StatusId = 1,
                    };

                    catalogueItem.CatalogueItemEpics.Add(catalogueItemEpic);
                }
            }

            if (staleEpics.Any())
                staleEpics.ForEach(epic => catalogueItem.CatalogueItemEpics.Remove(epic));
        }
    }
}
