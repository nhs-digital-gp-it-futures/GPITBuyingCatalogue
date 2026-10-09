using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageCapabilities;

namespace NHSD.GPIT.BuyingCatalogue.Services.Capabilities
{
    public sealed class CapabilitiesAdminService(BuyingCatalogueDbContext dbContext) : ICapabilitiesAdminService
    {
        private readonly BuyingCatalogueDbContext dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        public async Task<PagedList<AdminManageCapability>> GetPagedCapabilitiesAsync(
         PageOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);

            var baseQuery = dbContext.Capabilities
                .Include(x => x.Category)
                .IgnoreQueryFilters()
                .AsNoTracking();

            baseQuery = baseQuery.OrderBy(x => x.Name);

            options.TotalNumberOfItems = await baseQuery.CountAsync();

            if (options.PageNumber != 0)
                baseQuery = baseQuery.Skip((options.PageNumber - 1) * options.PageSize);

            var capabilityList = await baseQuery
                .Take(options.PageSize)
                .Select(x => new AdminManageCapability
                {
                    Id = x.Id,
                    CapabilityRef = x.CapabilityRef,
                    Name = x.Name,
                    CapabilityCategoryName = x.Category.Name,
                    Status = x.Status,
                })
                .ToListAsync();

            return new PagedList<AdminManageCapability>(
                capabilityList,
                options);
        }
    }
}
