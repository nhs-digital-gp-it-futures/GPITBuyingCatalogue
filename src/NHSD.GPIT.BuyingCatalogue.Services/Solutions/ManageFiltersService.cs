using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Services.Solutions
{
    public sealed class ManageFiltersService : IManageFiltersService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public ManageFiltersService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        
        public async Task<int> SaveFilter(
            string name,
            string description,
            int organisationId,
            List<int> capabilityIds,
            List<string> epicIds,
            string frameworkId,
            List<ClientApplicationType> clientApplicationTypes,
            List<HostingType> hostingTypes)
        {
            var organisation = await dbContext.Organisations.FirstAsync(o => o.Id == organisationId);

            var framework = !string.IsNullOrEmpty(frameworkId) ? await dbContext.Frameworks.FirstAsync(o => o.Id == frameworkId) : null;

            var filter = 
                new Filter() 
                {
                    Name = name,
                    Description = description,
                    Organisation = organisation,
                    Framework = framework,
                };

            dbContext.Filters.Add(filter);

            await dbContext.SaveChangesAsync();

            await AddFilterCapabilities(filter.Id, capabilityIds);
            await AddFilterEpics(filter.Id, epicIds);
            await AddFilterClientApplicationTypes(filter.Id, clientApplicationTypes);
            await AddFilterHostingTypes(filter.Id, hostingTypes);

            return filter.Id;
        }

        public async Task AddFilterCapabilities(int filterId, List<int> capabilityIds)
        {
            if(capabilityIds is null || capabilityIds.Count == 0) return;

            var filter = await dbContext.Filters.FirstAsync(o => o.Id == filterId);

            if (filter is null)
            {
                return;
            }

            foreach (var id in capabilityIds)
            {
                if(filter.FilterCapabilities.Any(x => x.CapabilityId == id))
                    continue;

                var capability = dbContext.Capabilities.First(x => x.Id == id);

                dbContext.FilterCapabilities.Add(new FilterCapability
                {
                    FilterId = filterId,
                    CapabilityId = id,
                    Capability = capability,
                });
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task AddFilterEpics(int filterId, List<string> epicIds)
        {
            if (epicIds is null || epicIds.Count == 0) return;

            var filter = await dbContext.Filters.FirstAsync(o => o.Id == filterId);

            if (filter is null)
            {
                return;
            }

            foreach (var id in epicIds)
            {
                if (filter.FilterEpics.Any(x => x.EpicId == id))
                    continue;

                var epic = dbContext.Epics.First(x => x.Id == id);

                dbContext.FilterEpics.Add(new FilterEpic()
                {
                    FilterId = filterId,
                    EpicId = id,
                    Epic = epic,
                });
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task AddFilterClientApplicationTypes(int filterId, List<ClientApplicationType> clientApplicationTypes)
        {
            if (clientApplicationTypes is null || clientApplicationTypes.Count == 0) return;

            var filter = await dbContext.Filters.FirstAsync(o => o.Id == filterId);

            if (filter is null)
            {
                return;
            }

            foreach (var type in clientApplicationTypes)
            {
                if (filter.FilterClientApplicationTypes.Any(x => x.ClientApplicationType == type))
                    continue;

                dbContext.FilterClientApplicationTypes.Add(new FilterClientApplicationType()
                {
                    FilterId = filterId,
                    ClientApplicationType = type,
                });
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task AddFilterHostingTypes(int filterId, List<HostingType> hostingTypes)
        {
            if (hostingTypes is null || hostingTypes.Count == 0) return;

            var filter = await dbContext.Filters.FirstAsync(o => o.Id == filterId);

            if (filter is null)
            {
                return;
            }

            foreach (var type in hostingTypes)
            {
                if (filter.FilterHostingTypes.Any(x => x.HostingType == type))
                    continue;

                dbContext.FilterHostingTypes.Add(new FilterHostingType()
                {
                    FilterId = filterId,
                    HostingType = type,
                });
            }

            await dbContext.SaveChangesAsync();
        }
        public async Task<bool> FilterExists(string filterName, int organisationId)
        {
            if(string.IsNullOrEmpty(filterName)) 
                throw new ArgumentNullException(nameof(filterName));

            return await dbContext.Filters.AnyAsync(f => f.Name == filterName && f.OrganisationId == organisationId);
        }

        public async Task<List<Filter>> GetFilters(int organisationId)
        {
            return await dbContext.Filters.Where(o => o.OrganisationId == organisationId)
                .Include(x => x.Framework)
                .Include(x => x.FilterCapabilities)
                    .ThenInclude(y => y.Capability)
                .Include(x => x.FilterEpics)
                    .ThenInclude(y => y.Epic)
                .Include(x => x.FilterHostingTypes)
                .Include(x => x.FilterClientApplicationTypes)
                .ToListAsync();
        }
    }
}
