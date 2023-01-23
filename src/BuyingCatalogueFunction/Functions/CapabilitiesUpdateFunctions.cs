using System;
using System.IO;
using System.Threading.Tasks;
using BuyingCatalogueFunction.Services.CapabilitiesUpdate.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace BuyingCatalogueFunction.Functions;

public class CapabilitiesUpdateFunctions
{
    private readonly ICapabilitiesService _capabilitiesService;
    private readonly ICapabilitiesUpdateService _capabilitiesUpdateService;
    private readonly ILogger<CapabilitiesUpdateFunctions> _logger;

    public CapabilitiesUpdateFunctions(
        ICapabilitiesService capabilitiesService,
        ICapabilitiesUpdateService capabilitiesUpdateService,
        ILogger<CapabilitiesUpdateFunctions> logger)
    {
        _capabilitiesService = capabilitiesService ?? throw new ArgumentNullException(nameof(capabilitiesService));
        _capabilitiesUpdateService = capabilitiesUpdateService ??
                                     throw new ArgumentNullException(nameof(capabilitiesUpdateService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [FunctionName(nameof(CapabilitiesUpdateBlobTrigger))]
    public async Task CapabilitiesUpdateBlobTrigger([BlobTrigger("capabilities-update/{name}")] Stream stream, string name)
    {
        _logger.LogInformation("Triggered {Function} for file {Name}", nameof(CapabilitiesUpdateBlobTrigger), name);
        var importedData = await _capabilitiesService.GetCapabilitiesAndEpics(stream);

        await _capabilitiesUpdateService.UpdateAsync(importedData);
    }
}
