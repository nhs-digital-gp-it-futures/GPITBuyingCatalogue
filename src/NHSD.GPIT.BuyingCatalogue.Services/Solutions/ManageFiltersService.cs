using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;
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
            List<InteropIm1Integrations> iM1IntegrationsTypes,
            List<InteropGpConnectIntegrations> gPConnectIntegrationsTypes,
            List<InteropIntegrationType> interoperabilityIntegrationTypes)
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

            await AddFilterCapabilities(filter.Id, capabilityAndEpicIds);
            await AddFilterCapabilityEpics(filter.Id, capabilityAndEpicIds);
            await AddFilterApplicationTypes(filter.Id, applicationTypes);
            await AddFilterHostingTypes(filter.Id, hostingTypes);
            await AddInteroperabilityIntegrationTypes(filter.Id, interoperabilityIntegrationTypes);
            await AddIM1IntegrationsTypes(filter.Id, iM1IntegrationsTypes);
            await AddGPConnectIntegrationsTypes(filter.Id, gPConnectIntegrationsTypes);

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
                        CapabilityAndEpicIds = new Dictionary<int, string[]>(x.Capabilities
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
                        IM1Integrations = x.FilterIM1IntegrationsTypes.Select(fc => (int)fc.IM1IntegrationsType),
                        GPConnectIntegrations = x.FilterGPConnectIntegrationsTypes.Select(fc => (int)fc.GPConnectIntegrationsType),
                        InteroperabilityOptions = x.FilterInteropIntegrationTypes.Select(fc => (int)fc.InteroperabilityIntegrationType),
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
                        InteropIntegrationTypes = x.FilterInteropIntegrationTypes.Select(y => y.InteroperabilityIntegrationType).ToList(),
                        InteropIm1IntegrationsTypes = x.FilterIM1IntegrationsTypes.Select(y => y.IM1IntegrationsType).ToList(),
                        InteropGpConnectIntegrationsTypes = x.FilterGPConnectIntegrationsTypes.Select(y => y.GPConnectIntegrationsType).ToList(),
                        Invalid = x.FilterCapabilityEpics.Any(e => e.CapabilityId == null)
                            || x.Capabilities.Any(c => c.Status == CapabilityStatus.Expired)
                            || x.FilterCapabilityEpics.Any(e => e.Epic.IsActive == false)
                            || !x.FilterCapabilityEpics.All(ce => ce.Capability.CapabilityEpics.Any(c => c.CapabilityId == ce.CapabilityId && c.EpicId == ce.EpicId)),
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

        internal async Task AddFilterCapabilities(int filterId, Dictionary<int, string[]> capabilitiesAndEpics)
        {
            if (capabilitiesAndEpics is null || capabilitiesAndEpics.Count == 0) return;

            var filter = await dbContext.Filters
                .Include(x => x.Capabilities)
                .FirstOrDefaultAsync(o => o.Id == filterId);

            if (filter is null)
            {
                return;
            }

            var capabilities = await dbContext.Capabilities.Where(x => capabilitiesAndEpics.Keys.Contains(x.Id)).ToListAsync();

            filter.Capabilities.AddRange(capabilities);

            await dbContext.SaveChangesAsync();
        }

        internal async Task AddFilterCapabilityEpics(int filterId, Dictionary<int, string[]> capabilitiesAndEpics)
        {
            if (capabilitiesAndEpics is null || capabilitiesAndEpics.Count == 0) return;

            var filter = await dbContext.Filters
                .Include(x => x.FilterCapabilityEpics)
                .FirstOrDefaultAsync(o => o.Id == filterId);

            if (filter is null)
            {
                return;
            }

            var filterCapabilityEpics = capabilitiesAndEpics
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

        internal async Task AddInteroperabilityIntegrationTypes(int filterId, List<InteropIntegrationType> interopIntegrationTypes)
        {
            if (interopIntegrationTypes is null || interopIntegrationTypes.Count == 0) return;

            var filter = await dbContext.Filters.FirstOrDefaultAsync(o => o.Id == filterId);

            if (filter is null)
            {
                return;
            }

            foreach (var type in interopIntegrationTypes)
            {
                filter.FilterInteropIntegrationTypes.Add(new FilterInteroperabilityIntegrationType()
                {
                    FilterId = filterId,
                    InteroperabilityIntegrationType = type,
                });
            }

            await dbContext.SaveChangesAsync();
        }

        internal async Task AddIM1IntegrationsTypes(int filterId, List<InteropIm1Integrations> interopIm1IntegrationsTypes)
        {
            if (interopIm1IntegrationsTypes is null || interopIm1IntegrationsTypes.Count == 0) return;

            var filter = await dbContext.Filters.FirstOrDefaultAsync(o => o.Id == filterId);

            if (filter is null)
            {
                return;
            }

            foreach (var type in interopIm1IntegrationsTypes)
            {
                filter.FilterIM1IntegrationsTypes.Add(new FilterIM1IntegrationsType()
                {
                    FilterId = filterId,
                    IM1IntegrationsType = type,
                });
            }

            await dbContext.SaveChangesAsync();
        }

        internal async Task AddGPConnectIntegrationsTypes(int filterId, List<InteropGpConnectIntegrations> interopGpConnectIntegrationsTypes)
        {
            if (interopGpConnectIntegrationsTypes is null || interopGpConnectIntegrationsTypes.Count == 0) return;

            var filter = await dbContext.Filters.FirstOrDefaultAsync(o => o.Id == filterId);

            if (filter is null)
            {
                return;
            }

            foreach (var type in interopGpConnectIntegrationsTypes)
            {
                filter.FilterGPConnectIntegrationsTypes.Add(new FilterGPConnectIntegrationsType()
                {
                    FilterId = filterId,
                    GPConnectIntegrationsType = type,
                });
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
