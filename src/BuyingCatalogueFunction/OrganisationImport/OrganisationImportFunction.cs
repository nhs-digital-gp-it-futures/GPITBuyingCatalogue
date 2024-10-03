using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BuyingCatalogueFunction.OrganisationImport.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace BuyingCatalogueFunction.OrganisationImport;

public class OrganisationImportFunction(
    ILoggerFactory loggerFactory,
    ITrudService trudService,
    ITrudApiService trudApiService)
{
    private readonly ILogger logger = loggerFactory.CreateLogger<OrganisationImportFunction>();
    private readonly ITrudService trudService = trudService ?? throw new ArgumentNullException(nameof(trudService));
    private readonly ITrudApiService trudApiService = trudApiService ?? throw new ArgumentNullException(nameof(trudApiService));

    [Function("OrganisationImportFunction")]
    public async Task Run([TimerTrigger("0 0 20 * * *")] TimerInfo timerInfo)
    {
        var stopwatch = Stopwatch.StartNew();
        var latestRelease = await trudApiService.GetLatestReleaseInfo();

        var alreadyImported = await trudService.HasImportedLatestRelease(latestRelease.ReleaseDate);
        if (alreadyImported)
        {
            stopwatch.Stop();
            logger.LogInformation("Latest TRUD release with release date {ReleaseDate} has already been imported", latestRelease.ReleaseDate);

            return;
        }

        var trudData = await trudService.GetTrudDataAsync(latestRelease.ArchiveFileUrl);
        if (trudData is null)
        {
            stopwatch.Stop();
            logger.LogError("Couldn't retrieve TRUD data from {Url}, see logs for more", latestRelease.ArchiveFileUrl);

            return;
        }

        logger.LogInformation("TRUD contains {Count} organisations",
            trudData.OrganisationsRoot.Organisations.Count.ToString("#,##"));

        await trudService.SaveTrudDataAsync(new(latestRelease.ReleaseDate, trudData, logger));

        stopwatch.Stop();

        logger.LogInformation("Processing TRUD took {ElapsedTime}ms", stopwatch.ElapsedMilliseconds);
    }
}
