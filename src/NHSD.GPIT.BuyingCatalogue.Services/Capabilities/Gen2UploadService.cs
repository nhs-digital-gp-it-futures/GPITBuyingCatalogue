using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using Microsoft.Extensions.Caching.Distributed;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;
using NHSD.GPIT.BuyingCatalogue.Services.Csv;

namespace NHSD.GPIT.BuyingCatalogue.Services.Capabilities;

public class Gen2UploadService : CsvServiceBase, IGen2UploadService
{
    private const string CapabilitiesCacheSuffix = "capabilities";
    private const string EpicsCacheSuffix = "Epics";

    private readonly IDistributedCache cache;

    public Gen2UploadService(
        IDistributedCache cache)
    {
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<Gen2CsvImportModel<Gen2CapabilitiesCsvModel>>
        GetCapabilitiesFromCsv(Stream capabilitiesStream) =>
        await ReadCsv<Gen2CapabilitiesCsvModel, Gen2CapabilitiesCsvClassMap>(
            capabilitiesStream,
            GetInvalidCapabilities);

    public async Task<Gen2CsvImportModel<Gen2EpicsCsvModel>>
        GetEpicsFromCsv(Stream epicsStream) =>
        await ReadCsv<Gen2EpicsCsvModel, Gen2EpicsCsvClassMap>(epicsStream, GetInvalidEpics);

    public async Task<Stream> WriteToCsv(IEnumerable<Gen2CapabilitiesCsvModel> records) =>
        await WriteToCsv<Gen2CapabilitiesCsvModel, Gen2CapabilitiesCsvClassMap>(records);

    public async Task<Stream> WriteToCsv(IEnumerable<Gen2EpicsCsvModel> records)
        => await WriteToCsv<Gen2EpicsCsvModel, Gen2EpicsCsvClassMap>(records);

    public async Task AddToCache(Guid id, Gen2CsvImportModel<Gen2CapabilitiesCsvModel> records) =>
        await AddToCache(id, CapabilitiesCacheSuffix, records);

    public async Task AddToCache(Guid id, Gen2CsvImportModel<Gen2EpicsCsvModel> records) =>
        await AddToCache(id, EpicsCacheSuffix, records);

    public async Task<Gen2CsvImportModel<Gen2CapabilitiesCsvModel>> GetCachedCapabilities(Guid id) =>
        await GetFromCache<Gen2CapabilitiesCsvModel>(new DistributedCacheKey(id, CapabilitiesCacheSuffix));

    public async Task<Gen2CsvImportModel<Gen2EpicsCsvModel>> GetCachedEpics(Guid id) =>
        await GetFromCache<Gen2EpicsCsvModel>(new DistributedCacheKey(id, EpicsCacheSuffix));

    private static async Task<Stream> WriteToCsv<T, TMap>(IEnumerable<T> records)
        where TMap : ClassMap<T>
    {
        var memoryStream = new MemoryStream();

        await WriteRecordsAsync<T, TMap>(memoryStream, records);

        memoryStream.Position = 0;

        return memoryStream;
    }

    private static bool IsBaseRecordValid(Gen2CsvBase baseRecord)
    {
        var keyIsValid = !string.IsNullOrWhiteSpace(baseRecord.Key);

        var supplierIdValid = !string.IsNullOrWhiteSpace(baseRecord.SupplierId)
            && int.TryParse(baseRecord.SupplierId, out _);

        var catalogueItemIdValid = !string.IsNullOrWhiteSpace(baseRecord.SolutionId)
            && Gen2ValidationRegex.CatalogueItemIdRegex().IsMatch(baseRecord.SolutionId)
            && CatalogueItemId.Parse(baseRecord.SolutionId).Success;

        var capabilityIdValid = !string.IsNullOrWhiteSpace(baseRecord.CapabilityId)
            && Gen2ValidationRegex.CapabilityIdRegex().IsMatch(baseRecord.CapabilityId);

        var additionalServiceIdValid = string.IsNullOrWhiteSpace(baseRecord.AdditionalServiceId)
            || Gen2ValidationRegex.AdditionalServiceRegex().IsMatch(baseRecord.AdditionalServiceId);

        var isRecordValid = keyIsValid && supplierIdValid && catalogueItemIdValid && capabilityIdValid
            && additionalServiceIdValid;

        return isRecordValid;
    }

    private static IEnumerable<Gen2CapabilitiesCsvModel> GetInvalidCapabilities(
        IEnumerable<Gen2CapabilitiesCsvModel> records) => records.Where(
        x => !IsBaseRecordValid(x) || (string.IsNullOrWhiteSpace(x.CapabilityAssessmentResult)
                || !x.IsValidAssessmentResult));

    private static IEnumerable<Gen2EpicsCsvModel> GetInvalidEpics(
        IEnumerable<Gen2EpicsCsvModel> records) => records.Where(
        x => !IsBaseRecordValid(x) || string.IsNullOrWhiteSpace(x.EpicId)
            || (string.IsNullOrWhiteSpace(x.EpicAssessmentResult)
                || !Gen2EpicsCsvModel.ValidAssessmentResults.Contains(x.EpicAssessmentResult)));

    private static async Task<Gen2CsvImportModel<T>> ReadCsv<T, TMap>(
        Stream stream,
        Func<IEnumerable<T>, IEnumerable<T>> filter)
        where T : Gen2CsvBase
        where TMap : ClassMap<T>
    {
        var records = await ReadCsv<T, TMap>(stream);

        return records == null
            ? null
            : new Gen2CsvImportModel<T>(records, filter(records));
    }

    private async Task AddToCache<T>(Guid id, string cacheSuffix, Gen2CsvImportModel<T> records)
        where T : Gen2CsvBase
    {
        var cacheKey = new DistributedCacheKey(id, cacheSuffix);
        var serializedRecords = JsonSerializer.Serialize(records);

        await cache.SetStringAsync(
            cacheKey.ToString(),
            serializedRecords,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });
    }

    private async Task<Gen2CsvImportModel<T>> GetFromCache<T>(DistributedCacheKey cacheKey)
        where T : Gen2CsvBase
    {
        var value = await cache.GetStringAsync(cacheKey.ToString());

        return string.IsNullOrWhiteSpace(value)
            ? null
            : JsonSerializer.Deserialize<Gen2CsvImportModel<T>>(value);
    }
}
