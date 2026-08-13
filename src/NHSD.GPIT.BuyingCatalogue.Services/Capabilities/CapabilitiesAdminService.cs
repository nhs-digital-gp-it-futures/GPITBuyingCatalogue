using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageCapability;

namespace NHSD.GPIT.BuyingCatalogue.Services.Capabilities;

public sealed class CapabilitiesAdminService(BuyingCatalogueDbContext dbContext) : ICapabilitiesAdminService
{
    private readonly BuyingCatalogueDbContext dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<PagedList<AdminManageCapability>> GetPagedCapabilities(
        PageOptions options,
        string search = null)
    {
        ArgumentNullException.ThrowIfNull(options);

        var baseQuery = dbContext.Capabilities
            .Include(x => x.Category)
            .IgnoreQueryFilters()
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            baseQuery = baseQuery.Where(x => x.CapabilityRef.Contains(search)
                || x.Name.Contains(search)
                || x.Category.Name.Contains(search));
        }

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

    public Task<Capability> GetCapability(int capabilityId) =>
        dbContext.Capabilities
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Epics)
            .Include(x => x.CatalogueItemCapabilities)
            .ThenInclude(x => x.CatalogueItem)
            .ThenInclude(x => x.Supplier)
            .Where(x => x.Id == capabilityId)
            .FirstOrDefaultAsync();

    public async Task UpdateCapability(UpdateAdminCapability request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var capability = await dbContext.Capabilities
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync()
            ?? throw new InvalidOperationException($"Capability with ID {request.Id} not found.");

        capability.Name = request.Name;
        capability.Description = request.Description;
        capability.CapabilityRef = request.CapabilityRef;
        capability.SourceUrl = request.SourceUrl?.ToString();
        capability.Status = request.Status;

        await dbContext.SaveChangesAsync();
    }
}
