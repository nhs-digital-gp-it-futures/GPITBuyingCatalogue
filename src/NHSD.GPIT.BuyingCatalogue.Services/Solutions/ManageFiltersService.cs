using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
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

        public async Task<int> AddFilter(
            string name,
            string description,
            int organisationId,
            Dictionary<int, string[]> capabilityAndEpicIds,
            string frameworkId,
            List<ApplicationType> applicationTypes,
            List<HostingType> hostingTypes)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrEmpty(description))
                throw new ArgumentNullException(nameof(description));

            var organisation = await dbContext.Organisations.FirstOrDefaultAsync(o => o.Id == organisationId);

            if (organisation == null)
                throw new ArgumentException("Invalid organisation", nameof(organisationId));

            var framework = !string.IsNullOrEmpty(frameworkId)
                ? await dbContext.Frameworks.FirstAsync(o => o.Id == frameworkId)
                : null;

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

            await AddFilterCapabilityEpics(filter.Id, capabilityAndEpicIds);
            await AddFilterApplicationTypes(filter.Id, applicationTypes);
            await AddFilterHostingTypes(filter.Id, hostingTypes);

            return filter.Id;
        }

        public async Task<bool> FilterExists(string filterName, int organisationId)
        {
            if (string.IsNullOrWhiteSpace(filterName))
                throw new ArgumentNullException(nameof(filterName));

            return await dbContext.Filters.AnyAsync(f => f.Name == filterName && f.OrganisationId == organisationId);
        }

        public async Task<List<Filter>> GetFilters(int organisationId)
        {
            return await dbContext.Filters.Where(o => o.OrganisationId == organisationId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task DeleteFilter(int filterId)
        {
            var filter = await dbContext.Filters.FirstOrDefaultAsync(o => o.Id == filterId);

            filter.IsDeleted = true;

            await dbContext.SaveChangesAsync();
        }

        public async Task<FilterIdsModel> GetFilterIds(int organisationId, int filterId)
        {
            var filter = await dbContext.Filters.Where(x => x.OrganisationId == organisationId && x.Id == filterId)
                .Include(f => f.FilterCapabilityEpics)
                .Include(f => f.FilterApplicationTypes)
                .Include(f => f.FilterHostingTypes)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync();

            return filter != null
                ? new FilterIdsModel()
                {
                    CapabilityAndEpicIds = filter.FilterCapabilityEpics
                            .GroupBy(c => c.CapabilityId)
                            .ToDictionary(i => i.Key, i => i.Where(c => c.EpicId != null).Select(c => c.EpicId).ToArray()),
                    FrameworkId = filter.FrameworkId,
                    ApplicationTypeIds = filter.FilterApplicationTypes.Select(fc => (int)fc.ApplicationTypeID),
                    HostingTypeIds = filter.FilterHostingTypes.Select(fc => (int)fc.HostingType),
                }
                : null;
        }

        public async Task<FilterDetailsModel> GetFilterDetails(int organisationId, int filterId)
        {
            var filter = await dbContext.Filters.Where(x => x.OrganisationId == organisationId && x.Id == filterId)
                .Include(f => f.FilterCapabilityEpics)
                    .ThenInclude(f => f.Epic)
                .Include(f => f.FilterCapabilityEpics)
                    .ThenInclude(f => f.Capability)
                .Include(f => f.Framework)
                .Include(f => f.FilterApplicationTypes)
                .Include(f => f.FilterHostingTypes)
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync();

            return filter != null
                ? new FilterDetailsModel()
                {
                    Id = filter.Id,
                    Name = filter.Name,
                    Description = filter.Description,
                    FrameworkName = filter.Framework?.ShortName,
                    HostingTypes = filter.FilterHostingTypes.Select(y => y.HostingType).ToList(),
                    ApplicationTypes =
                            filter.FilterApplicationTypes.Select(y => y.ApplicationTypeID).ToList(),
                    Capabilities = filter.FilterCapabilityEpics
                            .GroupBy(c => c.Capability.Name)
                            .ToDictionary(i => i.Key, i => i.Where(c => c.Epic != null).Select(c => c.Epic.Name).ToList())
                            .ToList(),
                }
                : null;
        }

        internal async Task AddFilterCapabilityEpics(int filterId, Dictionary<int, string[]> epicIds)
        {
            if (epicIds is null || epicIds.Count == 0) return;

            var filter = await dbContext.Filters
                .Include(x => x.FilterCapabilityEpics)
                .FirstOrDefaultAsync(o => o.Id == filterId);

            if (filter is null)
            {
                return;
            }

            var filterCapabilities = epicIds
                .Where(kv => kv.Value == null || kv.Value.Length == 0)
                .Select(kv => new FilterCapabilityEpic()
                {
                    CapabilityId = kv.Key,
                })
                .ToList();

            var filterCapabilityEpics = epicIds
                .Where(kv => kv.Value != null && kv.Value.Length > 0)
                .SelectMany(kv => kv.Value.Select(e => new
                {
                    CapabilityId = kv.Key,
                    Epic = dbContext.Epics.FirstOrDefault(x => x.Id == e),
                }))
                .Select(x => new FilterCapabilityEpic()
                {
                    CapabilityId = x.CapabilityId,
                    EpicId = x.Epic.Id,
                })
                .ToList();

            filter.FilterCapabilityEpics.AddRange(filterCapabilities);
            filter.FilterCapabilityEpics.AddRange(filterCapabilityEpics);

            await dbContext.SaveChangesAsync();
        }

        internal async Task AddFilterApplicationTypes(int filterId, List<ApplicationType> applicationTypes)
        {
            if (applicationTypes is null || applicationTypes.Count == 0) return;

            var filter = await dbContext.Filters.FirstOrDefaultAsync(o => o.Id == filterId);

            if (filter is null)
            {
                return;
            }

            foreach (var type in applicationTypes.Where(type => filter.FilterApplicationTypes.All(x => x.ApplicationTypeID != type)))
            {
                filter.FilterApplicationTypes.Add(new FilterApplicationType()
                {
                    FilterId = filterId,
                    ApplicationTypeID = type,
                });
            }

            await dbContext.SaveChangesAsync();
        }

        internal async Task AddFilterHostingTypes(int filterId, List<HostingType> hostingTypes)
        {
            if (hostingTypes is null || hostingTypes.Count == 0) return;

            var filter = await dbContext.Filters.FirstOrDefaultAsync(o => o.Id == filterId);

            if (filter is null)
            {
                return;
            }

            foreach (var type in hostingTypes)
            {
                filter.FilterHostingTypes.Add(new FilterHostingType()
                {
                    FilterId = filterId,
                    HostingType = type,
                });
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
