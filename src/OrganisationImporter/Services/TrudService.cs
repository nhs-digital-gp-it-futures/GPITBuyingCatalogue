using System.Xml;
using System.Xml.Serialization;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Extensions.Logging;
using OrganisationImporter.Interfaces;
using OrganisationImporter.Models;

namespace OrganisationImporter.Services;

public class TrudService : ITrudService
{
    private const string NestedZipFileName = "fullfile.zip";
    private readonly IHttpService _httpService;
    private readonly ILogger<TrudService> _logger;

    public TrudService(
        IHttpService httpService,
        ILogger<TrudService> logger)
    {
        _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<OrgRefData> GetTrudData(Uri url)
    {
        await using var dataStream = await GetDataStreamAsync(url);
        var orgRefData = ReadFromStream(dataStream);

        return orgRefData;
    }

    private async Task<Stream> GetDataStreamAsync(Uri url)
    {
        await using var zipStream = await _httpService.DownloadAsync(url);

        // Open the main Zip file and retrieve the nested Zip
        using var zipFile = new ZipFile(zipStream);
        var nestedZipFile = zipFile.GetEntry(NestedZipFileName);

        _logger.LogInformation("Retrieved nested zip file {NestedZipFileName}", nestedZipFile.Name);

        await using var fullFileStream = zipFile.GetInputStream(nestedZipFile);

        // Open the nested Zip and get the first file.
        // The file name isn't predictable and so GetEntry can't be used
        using var fullFileZip = new ZipFile(fullFileStream);
        var dataset = fullFileZip.Cast<ZipEntry>().First();

        _logger.LogInformation("Retrieved TRUD dataset file {DataSetFileName}", dataset.Name);

        await using var fullFileInputStream = fullFileZip.GetInputStream(dataset);

        var memoryStream = new MemoryStream();
        await fullFileInputStream.CopyToAsync(memoryStream);

        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }

    private OrgRefData ReadFromStream(Stream stream)
    {
        var serializer = new XmlSerializer(typeof(OrgRefData));

        _logger.LogInformation("Beginning deserialization of TRUD data");
        using var xmlReader = new XmlTextReader(stream);
        return (OrgRefData)serializer.Deserialize(xmlReader);
    }
}
