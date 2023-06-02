using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
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
            List<int> capabilityIds,
            List<string> epicIds,
            string frameworkId,
            List<ClientApplicationType> clientApplicationTypes,
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
                    Name = name, Description = description, Organisation = organisation, Framework = framework,
                };

            dbContext.Filters.Add(filter);

            await dbContext.SaveChangesAsync();

            await AddFilterCapabilities(filter.Id, capabilityIds);
            await AddFilterEpics(filter.Id, epicIds);
            await AddFilterClientApplicationTypes(filter.Id, clientApplicationTypes);
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

        public async Task<FilterDetailsModel> GetFilterDetails(int organisationId, int filterId)
            => await dbContext.Filters.Where(x => x.OrganisationId == organisationId && x.Id == filterId)
                .Select(
                    x => new FilterDetailsModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        FrameworkName = x.Framework.ShortName,
                        HostingTypes = x.FilterHostingTypes.Select(y => y.HostingType).ToList(),
                        ClientApplicationTypes =
                            x.FilterClientApplicationTypes.Select(y => y.ClientApplicationType).ToList(),
                        Capabilities = x.Capabilities
                            .Select(
                                y => new KeyValuePair<string, List<string>>(
                                    y.Name,
                                    x.Epics.Where(z => z.Capabilities.Any(c => c.Id == y.Id))
                                        .Select(z => z.Name)
                                        .ToList()))
                            .ToList(),
                    })
                .AsNoTracking()
                .FirstOrDefaultAsync();

        internal async Task AddFilterCapabilities(int filterId, List<int> capabilityIds)
        {
            if (capabilityIds is null || capabilityIds.Count == 0) return;

            var filter = await dbContext.Filters.Include(x => x.Capabilities).FirstOrDefaultAsync(o => o.Id == filterId);

            if (filter is null)
            {
                return;
            }

            var capabilities = await dbContext.Capabilities.Where(x => capabilityIds.Contains(x.Id)).ToListAsync();
            var distinctCapabilities = capabilities.Where(x => filter.Capabilities.All(y => x.Id != y.Id));

            filter.Capabilities.AddRange(distinctCapabilities);

            await dbContext.SaveChangesAsync();
        }

        internal async Task AddFilterEpics(int filterId, List<string> epicIds)
        {
            if (epicIds is null || epicIds.Count == 0) return;

            var filter = await dbContext.Filters.Include(x => x.Epics).FirstOrDefaultAsync(o => o.Id == filterId);

            if (filter is null)
            {
                return;
            }

            var epics = await dbContext.Epics.Where(x => epicIds.Contains(x.Id)).ToListAsync();
            var distinctEpics = epics.Where(x => filter.Epics.All(y => x.Id != y.Id));

            filter.Epics.AddRange(distinctEpics);

            await dbContext.SaveChangesAsync();
        }

        internal async Task AddFilterClientApplicationTypes(int filterId, List<ClientApplicationType> clientApplicationTypes)
        {
            if (clientApplicationTypes is null || clientApplicationTypes.Count == 0) return;

            var filter = await dbContext.Filters.FirstOrDefaultAsync(o => o.Id == filterId);

            if (filter is null)
            {
                return;
            }

            foreach (var type in clientApplicationTypes.Where(type => filter.FilterClientApplicationTypes.All(x => x.ClientApplicationType != type)))
            {
                filter.FilterClientApplicationTypes.Add(new FilterClientApplicationType()
                {
                    FilterId = filterId,
                    ClientApplicationType = type,
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
