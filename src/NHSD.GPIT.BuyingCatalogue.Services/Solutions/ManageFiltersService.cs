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
            List<HostingType> hostingTypes,
            Dictionary<SupportedIntegrations, int[]> integrations)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrEmpty(description))
                throw new ArgumentNullException(nameof(description));

            var useFramework = !string.IsNullOrEmpty(frameworkId) && await dbContext.Frameworks.AnyAsync(x => x.Id == frameworkId);

            var filter =
                new Filter
                {
                    Name = name,
                    Description = description,
                    OrganisationId = organisationId,
                    FrameworkId = useFramework ? frameworkId : null,
                    FilterHostingTypes = hostingTypes.Select(x => new FilterHostingType { HostingType = x }).ToList(),
                    FilterApplicationTypes = applicationTypes
                        .Select(x => new FilterApplicationType { ApplicationTypeID = x })
                        .ToList(),
                };

            dbContext.Filters.Add(filter);

            await AddIntegrations(filter, integrations);
            await AddFilterCapabilities(filter, capabilityAndEpicIds);
            AddFilterCapabilityEpics(filter, capabilityAndEpicIds);

            await dbContext.SaveChangesAsync();

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
            return await dbContext.Filters.Where(x => x.OrganisationId == organisationId && x.Id == filterId)
                .Select(
                    x => new FilterIdsModel()
                    {
                        CapabilityAndEpicIds = new Dictionary<int, string[]>(
                            x.Capabilities
                                .Select(
                                    y => new KeyValuePair<int, string[]>(
                                        y.Id,
                                        x.FilterCapabilityEpics.Where(z => z.CapabilityId == y.Id)
                                            .Select(z => z.Epic.Id)
                                            .ToArray()))
                                .ToList()),
                        FrameworkId = x.FrameworkId,
                        ApplicationTypeIds = x.FilterApplicationTypes.Select(fc => (int)fc.ApplicationTypeID),
                        HostingTypeIds = x.FilterHostingTypes.Select(fc => (int)fc.HostingType),
                        IntegrationsIds = new Dictionary<SupportedIntegrations, int[]>(
                            x.Integrations.Select(
                                y =>
                                    new KeyValuePair<SupportedIntegrations, int[]>(
                                        y.IntegrationId,
                                        y.IntegrationTypes.Select(
                                                z => z.IntegrationTypeId)
                                            .ToArray()))),
                    })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<FilterDetailsModel> GetFilterDetails(int organisationId, int filterId)
        {
            return await dbContext.Filters.Where(x => x.OrganisationId == organisationId && x.Id == filterId)
                .Select(
                    x => new FilterDetailsModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        FrameworkName = x.Framework.ShortName,
                        HostingTypes = x.FilterHostingTypes.Select(y => y.HostingType).ToList(),
                        ApplicationTypes =
                            x.FilterApplicationTypes.Select(y => y.ApplicationTypeID).ToList(),
                        Integrations =
                            x.Integrations.Select(
                                y => new KeyValuePair<string, string[]>(y.Integration.Name, y.IntegrationTypes.Select(z => z.IntegrationType.Name).ToArray())).ToList(),
                        Invalid = x.FilterCapabilityEpics.Any(e => e.CapabilityId == null)
                            || x.Capabilities.Any(c => c.Status == CapabilityStatus.Expired)
                            || x.FilterCapabilityEpics.Any(e => e.Epic.IsActive == false)
                            || !x.FilterCapabilityEpics.All(
                                ce => ce.Capability.CapabilityEpics.Any(
                                    c => c.CapabilityId == ce.CapabilityId && c.EpicId == ce.EpicId)),
                        Capabilities = x.Capabilities
                            .Select(
                                y => new KeyValuePair<string, List<string>>(
                                    y.NameWithStatusSuffix,
                                    x.FilterCapabilityEpics.Where(z => z.CapabilityId == y.Id)
                                        .Select(z => z.Epic.NameWithStatusSuffix)
                                        .ToList()))
                            .ToList(),
                    })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        internal async Task AddFilterCapabilities(Filter filter, Dictionary<int, string[]> capabilitiesAndEpics)
        {
            if (capabilitiesAndEpics is { Count: 0 }) return;

            var capabilities = await dbContext.Capabilities.Where(x => capabilitiesAndEpics.Keys.Contains(x.Id)).ToListAsync();

            filter.Capabilities.AddRange(capabilities);

            await dbContext.SaveChangesAsync();
        }

        internal async Task AddIntegrations(Filter filter, Dictionary<SupportedIntegrations, int[]> integrations)
        {
            if (integrations is { Count: 0 }) return;

            var integrationTypes = await dbContext.IntegrationTypes.ToListAsync();

            filter.Integrations = integrations.Select(
                    x => new FilterIntegration(x.Key)
                    {
                        IntegrationTypes = x.Value
                            .Where(y => integrationTypes.Any(z => z.Id == y && z.IntegrationId == x.Key))
                            .Select(y => new FilterIntegrationType(y))
                            .ToList(),
                    })
                .ToList();
        }

        internal void AddFilterCapabilityEpics(Filter filter, Dictionary<int, string[]> capabilitiesAndEpics)
        {
            if (capabilitiesAndEpics is { Count: 0 }) return;

            var filterCapabilityEpics = capabilitiesAndEpics
                .Where(kv => kv.Value is { Length: > 0 })
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

            filter.FilterCapabilityEpics.AddRange(filterCapabilityEpics);
        }
    }
}
