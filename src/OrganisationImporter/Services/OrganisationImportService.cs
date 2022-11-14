using Microsoft.Extensions.Logging;
using OrganisationImporter.Interfaces;

namespace OrganisationImporter.Services;

public class OrganisationImportService
{
    private readonly ITrudService _trudService;
    private readonly ILogger<OrganisationImportService> logger;

    public OrganisationImportService(
        ITrudService _trudService,
        ILogger<OrganisationImportService> logger)
    {
        this._trudService = _trudService ?? throw new ArgumentNullException(nameof(_trudService));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task ImportFromUrl(Uri url)
    {
        logger.LogInformation("Received {Url}", url);

        var trudData = await _trudService.GetTrudData(url);

        var roleTypes = trudData.GetRoleTypes();

        logger.LogInformation("TRUD contains {Count} organisations", trudData.Organisations.Organisation.Count.ToString("#,##"));
    }
}
