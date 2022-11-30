﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Constants;
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

        public async Task<List<Epic>> GetReferencedEpicsByCapabilityIds(IEnumerable<int> capabilityIds)
        {
            if (capabilityIds == null)
            {
                throw new ArgumentNullException(nameof(capabilityIds));
            }

            return await dbContext.Epics.AsNoTracking()
                .Include(x => x.Capability)
                .Where(
                    x => x.IsActive
                        && capabilityIds.Contains(x.Capability.Id)
                        && dbContext.CatalogueItemEpics.Any(y => y.EpicId == x.Id))
                .ToListAsync();
        }

        public async Task<string> GetEpicsForSelectedCapabilities(IEnumerable<int> capabilityIds, IEnumerable<string> epicIds)
        {
            var epics = await dbContext.Epics
                .Where(e => epicIds.Contains(e.Id) && capabilityIds.Contains(e.CapabilityId))
                .ToListAsync();

            return string.Join(FilterConstants.Delimiter, epics.Select(e => e.Id));
        }
    }
}
