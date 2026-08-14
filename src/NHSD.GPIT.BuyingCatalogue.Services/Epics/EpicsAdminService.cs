using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Epics;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdminManageEpic;

namespace NHSD.GPIT.BuyingCatalogue.Services.Epics;

public sealed class EpicsAdminService(BuyingCatalogueDbContext dbContext) : IEpicsAdminService
{
    private readonly BuyingCatalogueDbContext dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<PagedList<AdminManageEpic>> GetPagedEpicsAsync(
        PageOptions options,
        string search = null)
    {
        ArgumentNullException.ThrowIfNull(options);

        var baseQuery = dbContext.Epics
            .Include(x => x.Capabilities)
            .IgnoreQueryFilters()
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            baseQuery = baseQuery.Where(x => x.Name.Contains(search)
                || x.Description.Contains(search));
        }

        options.TotalNumberOfItems = await baseQuery.CountAsync();

        if (options.PageNumber != 0)
            baseQuery = baseQuery.Skip((options.PageNumber - 1) * options.PageSize);

        var epicList = await baseQuery
            .Take(options.PageSize)
            .Select(x => new AdminManageEpic
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                IsActive = x.IsActive,
                Capabilities = x.Capabilities == null ? 0 : x.Capabilities.Count,
            })
            .ToListAsync();

        return new PagedList<AdminManageEpic>(
            epicList,
            options);
    }

    public Task<Epic> GetEpicAsync(string epicId) =>
        dbContext.Epics
            .AsNoTracking()
            .Include(x => x.Capabilities)
            .ThenInclude(x => x.Category)
            .Where(x => x.Id == epicId)
            .FirstOrDefaultAsync();

    public async Task UpdateEpicAsync(UpdateAdminEpic request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var epic = await dbContext.Epics
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync()
            ?? throw new InvalidOperationException($"Epic with ID {request.Id} not found.");

        epic.Name = request.Name;
        epic.Description = request.Description;
        epic.SourceUrl = request.SourceUrl?.ToString();
        epic.IsActive = request.IsActive;

        await dbContext.SaveChangesAsync();
    }
}
