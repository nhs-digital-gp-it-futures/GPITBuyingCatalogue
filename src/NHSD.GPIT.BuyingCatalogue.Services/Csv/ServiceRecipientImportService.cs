using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.Services.Csv;

public class ServiceRecipientImportService : CsvServiceBase, IServiceRecipientImportService
{
    private readonly IDistributedCache distributedCache;

    public ServiceRecipientImportService(IDistributedCache distributedCache)
    {
        this.distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
    }

    public async Task CreateServiceRecipientTemplate(MemoryStream stream, IEnumerable<ServiceRecipientImportModel> serviceRecipients = null)
    {
        var recipients = serviceRecipients ?? Enumerable.Empty<ServiceRecipientImportModel>();

        await WriteRecordsAsync<ServiceRecipientImportModel, ServiceRecipientImportModelMap>(stream, recipients);
    }

    public async Task<IList<ServiceRecipientImportModel>> ReadFromStream(Stream stream)
    {
        using var streamReader = new StreamReader(stream);
        using var csvReader = new CsvReader(streamReader, new(CultureInfo.InvariantCulture)
        {
            TrimOptions = TrimOptions.Trim,
            ShouldSkipRecord = args => args.Record.All(string.IsNullOrWhiteSpace),
        });

        csvReader.Context.RegisterClassMap<ServiceRecipientImportModelMap>();

        List<ServiceRecipientImportModel> records;
        try
        {
            records = await csvReader
                .GetRecordsAsync<ServiceRecipientImportModel>()
                .Where(x => !string.IsNullOrWhiteSpace(x.Organisation) && !string.IsNullOrWhiteSpace(x.OdsCode))
                .ToListAsync();
        }
        catch (HeaderValidationException)
        {
            return null;
        }

        return records;
    }

    public async Task Store(ServiceRecipientCacheKey cacheKey, IList<ServiceRecipientImportModel> importedServiceRecipients)
    {
        var serializedRecipients = JsonSerializer.Serialize(importedServiceRecipients);

        await distributedCache.SetStringAsync(
            cacheKey.ToString(),
            serializedRecipients,
            new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });
    }

    public async Task Clear(ServiceRecipientCacheKey cacheKey)
    {
        var key = cacheKey.ToString();
        var cachedValue = await distributedCache.GetStringAsync(key);

        if (string.IsNullOrWhiteSpace(cachedValue))
            return;

        await distributedCache.RemoveAsync(key);
    }

    public async Task<IList<ServiceRecipientImportModel>> GetCached(ServiceRecipientCacheKey cacheKey)
    {
        var value = await distributedCache.GetStringAsync(cacheKey.ToString());
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return JsonSerializer.Deserialize<List<ServiceRecipientImportModel>>(value);
    }
}
