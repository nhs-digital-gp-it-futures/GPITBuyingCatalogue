using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.Extensions.Caching.Memory;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.Services.Csv;

public class ServiceRecipientImportService : CsvServiceBase, IServiceRecipientImportService
{
    private readonly IMemoryCache memoryCache;

    public ServiceRecipientImportService(IMemoryCache memoryCache)
    {
        this.memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    public async Task CreateServiceRecipientTemplate(MemoryStream stream)
    {
        var recipients = Enumerable.Empty<ServiceRecipientImportModel>();

        await WriteRecordsAsync<ServiceRecipientImportModel, ServiceRecipientImportModelMap>(stream, recipients);
    }

    /// <summary>
    /// Reads the Service Recipient data from the specified Stream
    /// </summary>
    /// <param name="stream">The <see cref="Stream"/>.</param>
    /// <returns>
    /// Null if the file format was invalid.<br/>
    /// Alternatively, a <see cref="List{T}"/> containing zero or more <see cref="ServiceRecipientImportModel"/>.
    /// </returns>
    public async Task<IList<ServiceRecipientImportModel>> ReadFromStream(Stream stream)
    {
        using var streamReader = new StreamReader(stream);
        using var csvReader = new CsvReader(streamReader, new(CultureInfo.InvariantCulture));

        csvReader.Context.RegisterClassMap<ServiceRecipientImportModelMap>();

        IAsyncEnumerable<ServiceRecipientImportModel> records;
        try
        {
            records = csvReader.GetRecordsAsync<ServiceRecipientImportModel>();
        }
        catch (HeaderValidationException)
        {
            return null;
        }

        return await records.ToListAsync();
    }

    public void StoreOrUpdate(ServiceRecipientCacheKey cacheKey, IList<ServiceRecipientImportModel> importedServiceRecipients)
        => memoryCache.Set(cacheKey.ToString(), importedServiceRecipients, TimeSpan.FromMinutes(10));

    public void Clear(ServiceRecipientCacheKey cacheKey)
    {
        var key = cacheKey.ToString();
        if (!memoryCache.TryGetValue(key, out _))
        {
            return;
        }

        memoryCache.Remove(key);
    }

    public IList<ServiceRecipientImportModel> GetCached(ServiceRecipientCacheKey cacheKey)
        => memoryCache.Get<List<ServiceRecipientImportModel>>(cacheKey.ToString());
}
