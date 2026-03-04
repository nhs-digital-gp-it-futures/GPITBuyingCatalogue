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

        public async Task<List<Epic>> GetEpicsByIds(IEnumerable<string> epicIds)
        {
            ArgumentNullException.ThrowIfNull(epicIds);

            return await dbContext.Epics.AsNoTracking()
                .Include(x => x.Capabilities)
                .Where(
                    x => x.IsActive
                        && epicIds.Contains(x.Id))
                .ToListAsync();
        }

        public async Task<List<Epic>> GetReferencedEpicsByCapabilityIds(IEnumerable<int> capabilityIds)
        {
            ArgumentNullException.ThrowIfNull(capabilityIds);

            return await dbContext.Epics.AsNoTracking()
                .Include(x => x.Capabilities)
                .Where(
                    x => x.IsActive
                        && x.Capabilities.Any(y => capabilityIds.Contains(y.Id))
                        && dbContext.CatalogueItemEpics.Any(y => y.EpicId == x.Id))
                .ToListAsync();
        }
    }
}
