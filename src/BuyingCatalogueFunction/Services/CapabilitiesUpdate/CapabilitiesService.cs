using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BuyingCatalogueFunction.Models.CapabilitiesUpdate;
using BuyingCatalogueFunction.Models.CapabilitiesUpdate.CsvModels;
using BuyingCatalogueFunction.Services.CapabilitiesUpdate.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using ICSharpCode.SharpZipLib.Zip;

namespace BuyingCatalogueFunction.Services.CapabilitiesUpdate;

public class CapabilitiesService : ICapabilitiesService
{
    public async Task<CapabilitiesImportModel> GetCapabilitiesAndEpics(Stream stream)
    {
        static async Task<List<T>> ReadDatasetInternalAsync<T>(ZipFile zipFile, string dataset)
        {
            var zipEntry = zipFile.GetEntry(dataset);
            if (zipEntry == null) throw new ZipException($"Entry {dataset} does not exist in {zipFile.Name}");

            await using var stream = zipFile.GetInputStream(zipEntry);

            return await ReadCsvAsync<T>(stream);
        }

        using var zipFile = new ZipFile(stream);

        var csvCapabilities = await ReadDatasetInternalAsync<CsvCapability>(zipFile, "Capabilities.csv");
        var csvStandards = await ReadDatasetInternalAsync<CsvStandard>(zipFile, "Standards.csv");
        var csvEpics = await ReadDatasetInternalAsync<CsvEpic>(zipFile, "Epics.csv");
        var csvRelationships = await ReadDatasetInternalAsync<CsvRelationship>(zipFile, "Relationships.csv");

        return new(csvCapabilities, csvStandards, csvEpics, csvRelationships);
    }

        private static async Task<List<T>> ReadCsvAsync<T>(Stream stream)
    {
        using var streamReader = new StreamReader(stream);
        using var csvReader = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            TrimOptions = TrimOptions.Trim,
        });

        csvReader.Context.RegisterClassMap<CsvRelationshipMap>();
        csvReader.Context.RegisterClassMap<CsvCapabilityMap>();
        csvReader.Context.RegisterClassMap<CsvStandardMap>();
        csvReader.Context.RegisterClassMap<CsvEpicMap>();

        return await csvReader.GetRecordsAsync<T>().ToListAsync();
    }
}
