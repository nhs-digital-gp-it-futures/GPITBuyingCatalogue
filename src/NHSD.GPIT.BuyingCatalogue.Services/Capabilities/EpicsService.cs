using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;

namespace NHSD.GPIT.BuyingCatalogue.Services.Capabilities
{
    public class EpicsService : IEpicsService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public EpicsService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<List<Epic>> GetActiveEpicsByCapabilityIds(IEnumerable<int> capabilityIds)
        {
            if (capabilityIds == null)
            {
                throw new ArgumentNullException(nameof(capabilityIds));
            }

            return await dbContext.Epics.AsNoTracking()
                .Include(x => x.Capability)
                .Where(x => x.IsActive
                    && capabilityIds.Contains(x.Capability.Id))
                .ToListAsync();
        }
    }
}
