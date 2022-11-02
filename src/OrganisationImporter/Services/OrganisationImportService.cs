using Microsoft.Extensions.Logging;

namespace OrganisationImporter.Services;

public class OrganisationImportService
{
    private readonly ILogger<OrganisationImportService> logger;

    public OrganisationImportService(ILogger<OrganisationImportService> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task ImportFromUrl(Uri url)
    {
        logger.LogInformation($"Received {url}");

        return Task.CompletedTask;
    }
}