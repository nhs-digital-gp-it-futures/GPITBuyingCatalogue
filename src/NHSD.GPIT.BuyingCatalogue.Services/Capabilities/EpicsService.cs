using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

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
            if (epicIds == null)
            {
                throw new ArgumentNullException(nameof(epicIds));
            }

            return await dbContext.Epics.AsNoTracking()
                .Include(x => x.Capabilities)
                .Where(
                    x => x.IsActive
                        && epicIds.Contains(x.Id))
                .ToListAsync();
        }

        public async Task<List<Epic>> GetReferencedEpicsByCapabilityIds(IEnumerable<int> capabilityIds)
        {
            if (capabilityIds == null)
            {
                throw new ArgumentNullException(nameof(capabilityIds));
            }

            return await dbContext.Epics.AsNoTracking()
                .Include(x => x.Capabilities)
                .Where(
                    x => x.IsActive
                        && x.Capabilities.Any(y => capabilityIds.Contains(y.Id))
                        && dbContext.CatalogueItemEpics.Any(y => y.EpicId == x.Id))
                .ToListAsync();
        }

        public async Task<PagedList<Epic>> GetEpicsAsync(PageOptions pageOptions)
        {
            var query = dbContext.Epics.AsNoTracking();
            var totalNumberOfItems = await query.CountAsync();
            if (pageOptions != null)
            {
                if (pageOptions.PageNumber != 0)
                    query = query.Skip((pageOptions.PageNumber - 1) * pageOptions.PageSize);

                query = query.Take(pageOptions.PageSize);
            }
            else
            {
                pageOptions = new PageOptions("1", totalNumberOfItems);
            }

            pageOptions.TotalNumberOfItems = totalNumberOfItems;

            var epics = await query.OrderBy(epic => epic.Id).ToListAsync();

            return new PagedList<Epic>(epics, pageOptions);
        }
    }
}
