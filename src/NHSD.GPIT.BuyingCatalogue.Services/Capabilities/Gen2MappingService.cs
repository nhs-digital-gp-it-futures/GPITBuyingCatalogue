using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;
using NHSD.GPIT.BuyingCatalogue.Services.Csv;

namespace NHSD.GPIT.BuyingCatalogue.Services.Capabilities;

public class Gen2MappingService(IDistributedCache cache) : CsvServiceBase, IGen2MappingService
{
    private const string CapabilitiesCacheSuffix = "capabilities";

    private readonly IDistributedCache cache = cache ?? throw new ArgumentNullException(nameof(cache));

    public async Task<Gen2CsvImportModel<Gen2CapabilitiesCsvModel>> GetCapabilitiesFromCsv(string fileName, Stream capabilitiesStream)
    {
        var records = await ReadCsv<Gen2CapabilitiesCsvModel, Gen2CapabilitiesCsvClassMap>(capabilitiesStream);

        return records == null
            ? null
            : new Gen2CsvImportModel<Gen2CapabilitiesCsvModel>(fileName, records, GetInvalidCapabilities(records));
    }

    public async Task<Stream> WriteCapabilitiesToCsv(IEnumerable<Gen2CapabilitiesCsvModel> records) =>
        await WriteToCsv<Gen2CapabilitiesCsvModel, Gen2CapabilitiesCsvClassMap>(records);

    public async Task<Guid> AddToCache(Gen2CsvImportModel<Gen2CapabilitiesCsvModel> records)
    {
        var serializedRecords = JsonSerializer.Serialize(records);

        var id = Guid.NewGuid();
        var cacheKey = ConstructCapabilitiesCacheKey(id);

        await cache.SetStringAsync(
            cacheKey,
            serializedRecords,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });

        return id;
    }

    public async Task<Gen2CsvImportModel<Gen2CapabilitiesCsvModel>> GetCachedCapabilities(Guid id)
    {
        var cacheKey = ConstructCapabilitiesCacheKey(id);
        var value = await cache.GetStringAsync(cacheKey);

        return string.IsNullOrWhiteSpace(value)
            ? null
            : JsonSerializer.Deserialize<Gen2CsvImportModel<Gen2CapabilitiesCsvModel>>(value);
    }

    private static async Task<Stream> WriteToCsv<T, TMap>(IEnumerable<T> records)
        where TMap : ClassMap<T>
    {
        var memoryStream = new MemoryStream();

        await WriteRecordsAsync<T, TMap>(memoryStream, records);

        memoryStream.Position = 0;

        return memoryStream;
    }

    private static IEnumerable<Gen2CapabilitiesCsvModel> GetInvalidCapabilities(
        IEnumerable<Gen2CapabilitiesCsvModel> records) => records.Where(
        x => (string.IsNullOrWhiteSpace(x.CapabilityAssessmentResult)
                || !Gen2CapabilitiesCsvModel.ValidAssessmentResults.Contains(x.CapabilityAssessmentResult))
            || (string.IsNullOrWhiteSpace(x.SupplierId) || !int.TryParse(x.SupplierId, out _))
            || (string.IsNullOrWhiteSpace(x.SolutionId) || !CatalogueItemId.Parse(x.SolutionId).Success)
            || (string.IsNullOrWhiteSpace(x.CapabilityId) || !x.CapabilityId.StartsWith('C')
                || !int.TryParse(x.CapabilityId.AsSpan()[1..], out _))
            || (!string.IsNullOrWhiteSpace(x.AdditionalServiceId) && x.AdditionalServiceId.Length != 4));

    private static string ConstructCapabilitiesCacheKey(Guid id) =>
        new DistributedCacheKey(id, CapabilitiesCacheSuffix).ToString();
}
