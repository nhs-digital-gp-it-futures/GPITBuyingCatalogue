using System;
using System.Threading.Tasks;
using BuyingCatalogueFunction.DatabaseMaintenance.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace BuyingCatalogueFunction.DatabaseMaintenance;

public class OptimizeIndexesFunction(
    IDatabaseIndexMaintenanceService databaseIndexMaintenanceService,
    ILogger<OptimizeIndexesFunction> logger)
{
    private readonly IDatabaseIndexMaintenanceService databaseIndexMaintenanceService =
        databaseIndexMaintenanceService ?? throw new ArgumentNullException(nameof(databaseIndexMaintenanceService));

    [Function("OptimizeIndexesFunction")]
    public async Task Run([TimerTrigger("0 0 20 * * Sat")] TimerInfo myTimer)
    {
        logger.LogInformation("Beginning optimize indexes function");

        await databaseIndexMaintenanceService.RebuildIndexes();
    }
}
