using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Services.Solutions
{
    public sealed class ManageFiltersService : IManageFiltersService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public ManageFiltersService(BuyingCatalogueDbContext dbContext, ICapabilitiesService capabilityService)
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
            var framework = await dbContext.Frameworks.FirstAsync(o => o.Id == frameworkId);

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

        public async Task<bool> FilterExists(string filterName, int organisationId)
        {
            return await dbContext.Filters.AnyAsync(f => f.Name == filterName && f.OrganisationId == organisationId);
        }

        private async Task AddFilterCapabilities(int filterId, List<int> capabilityIds)
        {
            foreach (var id in capabilityIds)
            {
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

        private async Task AddFilterEpics(int filterId, List<string> epicIds)
        {
            foreach (var id in epicIds)
            {
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

        private async Task AddFilterClientApplicationTypes(int filterId, List<ClientApplicationType> clientApplicationTypes)
        {
            foreach (var type in clientApplicationTypes)
            {
                dbContext.FilterClientApplicationTypes.Add(new FilterClientApplicationType()
                {
                    FilterId = filterId,
                    ClientApplicationType = type,
                });
            }

            await dbContext.SaveChangesAsync();
        }

        private async Task AddFilterHostingTypes(int filterId, List<HostingType> hostingTypes)
        {
            foreach (var type in hostingTypes)
            {
                dbContext.FilterHostingTypes.Add(new FilterHostingType()
                {
                    FilterId = filterId,
                    HostingType = type,
                });
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
