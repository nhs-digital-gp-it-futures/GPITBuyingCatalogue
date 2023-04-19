using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuyingCatalogueFunction.Extensions;
using BuyingCatalogueFunction.Models.CapabilitiesUpdate;
using BuyingCatalogueFunction.Models.CapabilitiesUpdate.CsvModels;
using BuyingCatalogueFunction.Services.CapabilitiesUpdate.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace BuyingCatalogueFunction.Services.CapabilitiesUpdate;

public class CapabilitiesUpdateService : ICapabilitiesUpdateService
{
    private readonly BuyingCatalogueDbContext _dbContext;
    private readonly ILogger<CapabilitiesUpdateService> _logger;

    public CapabilitiesUpdateService(
        BuyingCatalogueDbContext dbContext,
        ILogger<CapabilitiesUpdateService> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task UpdateAsync(CapabilitiesImportModel capabilitiesAndEpics)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            await UpdateStandardsAsync(capabilitiesAndEpics.Standards);
            await UpdateCapabilitiesAsync(capabilitiesAndEpics.Capabilities);
            await UpdateCapabilityRelationshipsAsync(capabilitiesAndEpics.Relationships);
            await UpdateEpicsAsync(capabilitiesAndEpics.Epics);

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occurred while updating {@Exception}", ex);

            await transaction.RollbackAsync();
        }
    }

    internal async Task UpdateStandardsAsync(IEnumerable<CsvStandard> csvStandards)
    {
        foreach (var standard in csvStandards)
        {
            var dbStandard = await _dbContext.Standards.FirstOrDefaultAsync(x => x.Id == standard.Id);
            if (dbStandard == null)
                continue;

            dbStandard.Name = standard.Name;
            dbStandard.Description = standard.Description;
            dbStandard.Version = standard.Version;
            dbStandard.Url = standard.Url;
            dbStandard.StandardType = standard.GetStandardType();
        }

        await _dbContext.SaveChangesAsync();
    }

    internal async Task UpdateCapabilitiesAsync(IEnumerable<CsvCapability> csvCapabilities)
    {
        foreach (var csvCapability in csvCapabilities)
        {
            var capabilityId = CapabilityExtensions.ParseCapabilityId(csvCapability.Id);
            var capability = await _dbContext.Capabilities.Include(x => x.FrameworkCapabilities)
                .FirstOrDefaultAsync(x => x.Id == capabilityId);
            var capabilityCategory =
                await _dbContext.CapabilityCategories.FirstOrDefaultAsync(x => x.Name == csvCapability.Category);

            if (capability == null)
                continue;

            capability.Name = csvCapability.Name;
            capability.Description = csvCapability.Description;
            capability.Version = csvCapability.Version;
            capability.SourceUrl = csvCapability.Url;
            capability.CategoryId = capabilityCategory.Id;

            await UpdateFrameworksForCapabilityAsync(csvCapability, capability);
        }

        await _dbContext.SaveChangesAsync();
    }

    internal async Task UpdateCapabilityRelationshipsAsync(IEnumerable<CsvRelationship> csvRelationship)
    {
        var relationships = csvRelationship
            .Where(x => x.FromId.StartsWith("C"))
            .Select(x => new StandardCapability()
            {
                CapabilityId = CapabilityExtensions.ParseCapabilityId(x.FromId), StandardId = x.ToId
            }).ToList();

        var dbRelationships = await _dbContext.StandardCapabilities.ToListAsync();
        var dbCapabilities = await _dbContext.Capabilities.ToListAsync();
        var dbStandards = await _dbContext.Standards.ToListAsync();

        var toAdd = relationships.Where(x =>
            !dbRelationships.Any(y => x.CapabilityId == y.CapabilityId && x.StandardId == y.StandardId) &&
            dbCapabilities.Any(y => x.CapabilityId == y.Id) &&
            dbStandards.Any(y => x.StandardId == y.Id)).ToList();

        var toRemove = dbRelationships.Where(x =>
                !relationships.Any(y => x.StandardId == y.StandardId && x.CapabilityId == y.CapabilityId))
            .ToList();

        _dbContext.StandardCapabilities.RemoveRange(toRemove);
        _dbContext.StandardCapabilities.AddRange(toAdd);

        await _dbContext.SaveChangesAsync();
    }

    internal async Task UpdateFrameworksForCapabilityAsync(CsvCapability csvCapability, Capability capability)
    {
        var frameworks = csvCapability.Framework.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList();
        if (!frameworks.Any()) return;

        var frameworkIds = new List<string>();
        foreach (var csvFramework in frameworks)
        {
            var framework = await _dbContext.Frameworks.FirstOrDefaultAsync(x => x.ShortName == csvFramework);
            if (framework == null)
                continue;

            frameworkIds.Add(framework.Id);
        }

        if (!frameworkIds.Any()) return;

        var frameworksToRemove =
            capability.FrameworkCapabilities.Where(x => !frameworkIds.Contains(x.FrameworkId)).ToList();
        var frameworksToAdd = frameworkIds.Where(x => capability.FrameworkCapabilities.All(y => x != y.FrameworkId))
            .Select(x => new FrameworkCapability(x, capability.Id)).ToList();

        _dbContext.FrameworkCapabilities.RemoveRange(frameworksToRemove.ToList());
        _dbContext.FrameworkCapabilities.AddRange(frameworksToAdd);

        await _dbContext.SaveChangesAsync();
    }

    internal async Task UpdateEpicsAsync(IEnumerable<CsvEpic> csvEpics)
    {
        foreach (var epic in csvEpics)
        {
            var dbEpic = await _dbContext.Epics.FirstOrDefaultAsync(x => x.Id == epic.Id);
            if (dbEpic == null)
                continue;

            dbEpic.Name = epic.Name;
            dbEpic.IsActive = string.Equals("Active", epic.Status.Trim(), StringComparison.OrdinalIgnoreCase);
            dbEpic.CompliancyLevel = epic.GetCompliancyLevel();
            dbEpic.CapabilityId = CapabilityExtensions.ParseCapabilityId(epic.CapabilityId);
        }

        await _dbContext.SaveChangesAsync();
    }
}
