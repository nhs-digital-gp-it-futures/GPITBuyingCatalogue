using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;

namespace NHSD.GPIT.BuyingCatalogue.Services.Capabilities;

public class Gen2MappingService : IGen2MappingService
{
    private readonly BuyingCatalogueDbContext dbContext;
    private readonly ILogger<Gen2MappingService> logger;

    public Gen2MappingService(
        BuyingCatalogueDbContext dbContext,
        ILogger<Gen2MappingService> logger)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> MapToSolutions(Gen2MappingModel gen2Mapping)
    {
        ArgumentNullException.ThrowIfNull(gen2Mapping);

        if (gen2Mapping.Solutions.Count == 0)
            throw new ArgumentNullException(nameof(gen2Mapping), string.Empty);

        var success = false;

        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        var existingCapabilityIds = await dbContext.Capabilities.Select(x => x.Id).ToListAsync();
        var existingEpics = await dbContext.Epics.Select(x => x.Id).ToListAsync();

        try
        {
            foreach (var solutionMapping in gen2Mapping.Solutions)
            {
                logger.LogInformation("Updating Capabilities and Epics for {@Solution}", solutionMapping);

                var solutionToUpdate = await dbContext.Solutions
                    .Include(x => x.CatalogueItem.CatalogueItemCapabilities)
                    .Include(x => x.CatalogueItem.CatalogueItemEpics)
                    .Include(x => x.AdditionalServices)
                    .ThenInclude(x => x.CatalogueItem.CatalogueItemCapabilities)
                    .Include(x => x.AdditionalServices)
                    .ThenInclude(x => x.CatalogueItem.CatalogueItemEpics)
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(x => x.CatalogueItemId == solutionMapping.Id);

                if (solutionToUpdate == null)
                {
                    logger.LogWarning("Solution not found with ID {ID}", solutionMapping.Id);
                    continue;
                }

                UpdateCatalogueItem(
                    solutionToUpdate.CatalogueItem,
                    solutionMapping,
                    existingCapabilityIds,
                    existingEpics);

                if (solutionMapping.AdditionalServices.Count == 0) continue;

                foreach (var additionalServiceMapping in solutionMapping.AdditionalServices)
                {
                    var additionalServiceToUpdate =
                        solutionToUpdate.AdditionalServices.FirstOrDefault(
                            x => x.CatalogueItemId == additionalServiceMapping.Id);

                    if (additionalServiceToUpdate == null) continue;

                    UpdateCatalogueItem(
                        additionalServiceToUpdate.CatalogueItem,
                        additionalServiceMapping,
                        existingCapabilityIds,
                        existingEpics);
                }
            }

            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            success = true;
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(
                "Failed to update Capabilities for entries {@Rows}, {Exception}",
                ex.Entries.Select(x => x.ToString()),
                ex);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError("Failed to update Capabilities {Exception}", ex);
        }
#pragma warning disable CA1031
        catch (Exception ex)
#pragma warning restore CA1031
        {
            logger.LogCritical(
                "An unexpected error occurred when updating the Solution Capability data {Exception}",
                ex);
        }
        finally
        {
            if (!success)
                await transaction.RollbackAsync();
        }

        return success;
    }

    private void UpdateCatalogueItem(CatalogueItem catalogueItem, Gen2CatalogueItemMappingModel model, ICollection<int> validCapabilities, ICollection<string> validEpics)
    {
        dbContext.RemoveRange(catalogueItem.CatalogueItemCapabilities);
        dbContext.RemoveRange(catalogueItem.CatalogueItemEpics);

        var filteredCapabilities = model.Capabilities.Where(x => validCapabilities.Contains(x.CapabilityId)).ToList();
        var allEpics = filteredCapabilities.SelectMany(x => x.Epics.Select(y => (x.CapabilityId, EpicId: y))).ToList();
        var filteredEpics = allEpics.Where(x => validEpics.Contains(x.EpicId)).ToList();

        var missingCapabilities = model.Capabilities.Except(filteredCapabilities).ToList();
        var missingEpics = allEpics.Except(filteredEpics).ToList();

        if (missingCapabilities.Count > 0)
        {
            logger.LogWarning(
                "Capabilities not found in reference data {@Capabilities}",
                missingCapabilities.Select(x => x.CapabilityRef));
        }

        if (missingEpics.Count > 0)
        {
            logger.LogWarning(
                "Epics not found in reference data {@Epics}",
                missingEpics.Select(x => x.EpicId));
        }

        catalogueItem.CatalogueItemCapabilities =
            filteredCapabilities.Select(x => new CatalogueItemCapability(model.Id, x.CapabilityId) { StatusId = 1 })
                .ToList();

        catalogueItem.CatalogueItemEpics = filteredEpics
            .Select(x => new CatalogueItemEpic(model.Id, x.CapabilityId, x.EpicId) { StatusId = 1 })
            .ToList();
    }
}
