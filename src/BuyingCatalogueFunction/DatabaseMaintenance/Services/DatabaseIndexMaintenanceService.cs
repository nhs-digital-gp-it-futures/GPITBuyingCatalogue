using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using BuyingCatalogueFunction.DatabaseMaintenance.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;

namespace BuyingCatalogueFunction.DatabaseMaintenance.Services;

[ExcludeFromCodeCoverage(Justification = "Executes stored procedure")]
public class DatabaseIndexMaintenanceService(
    BuyingCatalogueDbContext dbContext,
    ILogger<DatabaseIndexMaintenanceService> logger) : IDatabaseIndexMaintenanceService
{
    private readonly BuyingCatalogueDbContext dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    private readonly ILogger<DatabaseIndexMaintenanceService> logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task RebuildIndexes()
    {
        try
        {
            dbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(20));
            await dbContext.Database.ExecuteSqlRawAsync("EXEC catalogue.OptimizeIndexes");
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Index optimization failed");
            throw;
        }
    }
}
