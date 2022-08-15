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

        public Task<List<Capability>> GetCapabilities() => dbContext.Capabilities.AsNoTracking()
            .Include(x => x.Category)
            .OrderBy(x => x.Category.Name)
            .ThenBy(x => x.Name)
            .ToListAsync();

        public Task<List<CapabilityCategory>> GetCapabilitiesByCategory()
            => dbContext
                .CapabilityCategories
                .Include(c => c.Capabilities)
                .ThenInclude(c => c.Epics.Where(e => e.IsActive && e.CompliancyLevel == CompliancyLevel.May))
                .ToListAsync();

        public async Task AddCapabilitiesToCatalogueItem(CatalogueItemId catalogueItemId, SaveCatalogueItemCapabilitiesModel model)
        {
            if (model is null)
                throw new ArgumentNullException(nameof(model));

            var catalogueItemCapabilities = await dbContext.CatalogueItemCapabilities.Where(c => c.CatalogueItemId == catalogueItemId).ToListAsync();
            var catalogueItemEpics = await dbContext.CatalogueItemEpics.Where(e => e.CatalogueItemId == catalogueItemId).ToListAsync();

            AddCapabilities(catalogueItemId, catalogueItemCapabilities, model);
            await AddCapabilityEpics(catalogueItemId, catalogueItemEpics, model);

            await dbContext.SaveChangesAsync();
        }

        private void AddCapabilities(CatalogueItemId catalogueItemId, List<CatalogueItemCapability> existingCapabilities, SaveCatalogueItemCapabilitiesModel model)
        {
            RemoveStaleCapabilities(existingCapabilities, model);

            foreach (var (id, _) in model.Capabilities)
            {
                if (existingCapabilities.Any(c => c.CapabilityId == id))
                    continue;

                var catalogueItemCapability = new CatalogueItemCapability
                {
                    CatalogueItemId = catalogueItemId,
                    CapabilityId = id,
                    StatusId = 1,
                };

                dbContext.CatalogueItemCapabilities.Add(catalogueItemCapability);
            }
        }

        private async Task AddCapabilityEpics(CatalogueItemId catalogueItemId, List<CatalogueItemEpic> existingEpics, SaveCatalogueItemCapabilitiesModel model)
        {
            var capabilitiesWithMustEpics = (await GetCapabilitiesWithMustEpics(model.Capabilities.Select(c => c.Key).ToArray()))
                .ToDictionary(c => c.Id, c => c.Epics.Select(e => e.Id).ToArray());

            var capabilitiesAndEpics = model
                .Capabilities
                .Concat(capabilitiesWithMustEpics)
                .GroupBy(kvp => kvp.Key, kvp => kvp.Value)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.SelectMany(k => k).ToArray());

            RemoveStaleEpics(existingEpics, capabilitiesAndEpics);

            foreach (var (id, epicIds) in capabilitiesAndEpics)
            {
                foreach (var epicId in epicIds)
                {
                    if (existingEpics.Any(c => c.EpicId == epicId))
                        continue;

                    var catalogueItemEpic = new CatalogueItemEpic
                    {
                        CatalogueItemId = catalogueItemId,
                        CapabilityId = id,
                        EpicId = epicId,
                        StatusId = 1,
                    };

                    dbContext.CatalogueItemEpics.Add(catalogueItemEpic);
                }
            }
        }

        private void RemoveStaleCapabilities(List<CatalogueItemCapability> existingCapabilities, SaveCatalogueItemCapabilitiesModel model)
        {
            var staleCapabilities = existingCapabilities.Where(capability => !model.Capabilities.Any(newCapability => newCapability.Key == capability.CapabilityId));
            dbContext.CatalogueItemCapabilities.RemoveRange(staleCapabilities);
        }

        private void RemoveStaleEpics(List<CatalogueItemEpic> existingEpics, Dictionary<int, string[]> selectedCapabilitiesAndEpics)
        {
            var staleEpics = existingEpics
                .Where(epic => !selectedCapabilitiesAndEpics.SelectMany(c => c.Value).Contains(epic.EpicId)).ToList();

            if (staleEpics.Any())
                dbContext.CatalogueItemEpics.RemoveRange(staleEpics);
        }

        private Task<List<Capability>> GetCapabilitiesWithMustEpics(params int[] capabilityIds)
            => dbContext.Capabilities.Include(c => c.Epics.Where(e => e.IsActive && e.CompliancyLevel == CompliancyLevel.Must)).Where(c => capabilityIds.Contains(c.Id)).ToListAsync();
    }
}
