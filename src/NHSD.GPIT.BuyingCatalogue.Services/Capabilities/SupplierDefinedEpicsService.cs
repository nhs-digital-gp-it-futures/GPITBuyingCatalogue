using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.SupplierDefinedEpics;

namespace NHSD.GPIT.BuyingCatalogue.Services.Capabilities
{
    public sealed class SupplierDefinedEpicsService : ISupplierDefinedEpicsService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public SupplierDefinedEpicsService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task AddSupplierDefinedEpic(AddEditSupplierDefinedEpic epicModel)
        {
            if (epicModel is null)
                throw new ArgumentNullException(nameof(epicModel));

            var capability = await dbContext.Capabilities.FirstOrDefaultAsync(x => x.Id == epicModel.CapabilityId);

            var epic = new Epic
            {
                Name = epicModel.Name,
                Description = epicModel.Description,
                IsActive = epicModel.IsActive,
                SupplierDefined = true,
            };

            epic.CapabilityEpics.Add(new CapabilityEpic()
            {
                CapabilityId = capability.Id,
                Epic = epic,
                CompliancyLevel = CompliancyLevel.May,
            });

            dbContext.Epics.Add(epic);
            await dbContext.SaveChangesAsync();
        }

        public async Task EditSupplierDefinedEpic(AddEditSupplierDefinedEpic epicModel)
        {
            if (epicModel is null)
                throw new ArgumentNullException(nameof(epicModel));

            var epic = await dbContext
                .Epics
                .Include(x => x.Capabilities)
                .AsSplitQuery()
                .FirstOrDefaultAsync(e => e.Id == epicModel.Id && e.SupplierDefined);

            if (epic is null)
                throw new KeyNotFoundException($"{epicModel.Id} is not a valid Epic Id");

            epic.Name = epicModel.Name;
            epic.Description = epicModel.Description;
            epic.IsActive = epicModel.IsActive;

            var capability = await dbContext.Capabilities.FirstOrDefaultAsync(x => x.Id == epicModel.CapabilityId);
            epic.Capabilities.Clear();
            epic.Capabilities = new List<Capability> { capability };

            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteSupplierDefinedEpic(string epicId)
        {
            var epic = await dbContext.Epics.FirstOrDefaultAsync(e => e.Id == epicId);
            if (epic is null)
                return;

            dbContext.Epics.Remove(epic);
            await dbContext.SaveChangesAsync();
        }

        public Task<bool> EpicExists(
            string epicId,
            string name,
            string description,
            bool isActive) =>
            dbContext
               .Epics
               .AnyAsync(e =>
                   e.Id != epicId
                   && e.Name == name
                   && e.Description == description
                   && e.IsActive == isActive
                   && e.SupplierDefined == true);

        public Task<List<Epic>> GetSupplierDefinedEpics()
            => dbContext
                .Epics
                .AsNoTracking()
                .Include(e => e.Capabilities)
                .Where(e => e.SupplierDefined)
                .ToListAsync();

        public Task<List<Epic>> GetSupplierDefinedEpicsBySearchTerm(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new ArgumentNullException(nameof(searchTerm));

            return dbContext.Epics
                .AsNoTracking()
                .Include(e => e.Capabilities)
                .Where(e => e.SupplierDefined
                    && EF.Functions.Like(e.Name, $"%{searchTerm}%"))
                .ToListAsync();
        }

        public Task<Epic> GetEpic(string epicId)
        {
            if (string.IsNullOrWhiteSpace(epicId))
                throw new ArgumentException("Id is null or empty", nameof(epicId));

            return dbContext
                .Epics
                .Include(x => x.Capabilities)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(e =>
                    e.Id == epicId
                    && e.SupplierDefined == true);
        }

        public Task<List<CatalogueItem>> GetItemsReferencingEpic(string epicId)
        {
            if (string.IsNullOrWhiteSpace(epicId))
                throw new ArgumentException("Id is null or empty", nameof(epicId));

            return dbContext
                  .CatalogueItemEpics
                  .Include(e => e.CatalogueItem)
                  .ThenInclude(e => e.AdditionalService)
                  .AsNoTracking()
                  .Where(e =>
                      e.EpicId == epicId
                      && e.Epic.SupplierDefined == true)
                  .Select(e => e.CatalogueItem)
                  .ToListAsync();
        }
    }
}
