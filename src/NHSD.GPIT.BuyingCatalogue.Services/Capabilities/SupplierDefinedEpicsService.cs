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

            var epic = new Epic
            {
                CapabilityId = epicModel.CapabilityId,
                Name = epicModel.Name,
                Description = epicModel.Description,
                IsActive = epicModel.IsActive,
                SupplierDefined = true,
                CompliancyLevel = CompliancyLevel.May,
            };

            dbContext.Epics.Add(epic);
            await dbContext.SaveChangesAsync();
        }

        public Task<bool> EpicExists(
            string id,
            int capabilityId,
            string name,
            string description,
            bool isActive) =>
            dbContext
               .Epics
               .AnyAsync(e =>
                   e.Id != id
                   && e.CapabilityId == capabilityId
                   && e.Name == name
                   && e.Description == description
                   && e.IsActive == isActive
                   && e.SupplierDefined == true);

        public Task<List<Epic>> GetSupplierDefinedEpics()
            => dbContext
                .Epics
                .AsNoTracking()
                .Include(e => e.Capability)
                .Where(e => e.SupplierDefined)
                .ToListAsync();
    }
}
