using System.Diagnostics;
using Microsoft.Extensions.Logging;
using OrganisationImporter.Interfaces;

namespace OrganisationImporter.Services;

public class OrganisationImportService
{
    private readonly ITrudService _trudService;
    private readonly ILogger<OrganisationImportService> _logger;

    public OrganisationImportService(
        ITrudService trudService,
        ILogger<OrganisationImportService> logger)
    {
        this._trudService = trudService ?? throw new ArgumentNullException(nameof(trudService));
        this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task ImportFromUrl(Uri url)
    {
        _logger.LogInformation("Received {Url}", url);

        var stopwatch = Stopwatch.StartNew();
        var trudData = await _trudService.GetTrudDataAsync(url);
        if (trudData is null)
        {
            stopwatch.Stop();
            _logger.LogError("Couldn't retrieve TRUD data from {Url}, see logs for more", url);

            return;
        }

        _logger.LogInformation("TRUD contains {Count} organisations", trudData.OrganisationsRoot.Organisations.Count.ToString("#,##"));

        await _trudService.SaveTrudDataAsync(new(trudData, _logger));

        stopwatch.Stop();

        _logger.LogInformation("Processing TRUD took {ElapsedTime}ms", stopwatch.ElapsedMilliseconds);
    }
}
