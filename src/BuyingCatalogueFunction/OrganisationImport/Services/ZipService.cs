using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.Extensions.Logging;
using BuyingCatalogueFunction.OrganisationImport.Interfaces;

namespace BuyingCatalogueFunction.OrganisationImport.Services;

public class ZipService(ILogger<ZipService> logger) : IZipService
{
    private const string NestedZipFileName = "fullfile.zip";
    private const string XmlFileExtension = ".xml";

    private readonly ILogger<ZipService> logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<Stream> GetTrudDataFileAsync(Stream zipStream)
    {
        // Open the main Zip file and retrieve the nested Zip
        using var zipFile = new ZipFile(zipStream);

        var nestedZipFile = zipFile.GetEntry(NestedZipFileName);
        if (nestedZipFile is null)
        {
            logger.LogError("Couldn't find {NestedZipFileName} in {ParentZipFileName}", NestedZipFileName,
                zipFile.Name);

            return null;
        }

        logger.LogInformation("Retrieved nested zip file {NestedZipFileName}", nestedZipFile.Name);

        // Open the nested Zip and get the first XML file.
        // The file name isn't predictable and so GetEntry can't be used
        await using var fullFileStream = zipFile.GetInputStream(nestedZipFile);
        using var fullFileZip = new ZipFile(fullFileStream);

        // If there is more than 1 XML file, we can't predict which one to use
        var zipEntries = fullFileZip.Cast<ZipEntry>().Where(x => x.IsFile && x.Name.EndsWith(XmlFileExtension))
            .ToArray();

        if (zipEntries.Length > 1)
        {
            logger.LogError("Nested archive contains more than one XML file. {@Files}",
                zipEntries.Select(x => x.Name));

            return null;
        }

        var dataset = zipEntries.First();
        await using var fullFileInputStream = fullFileZip.GetInputStream(dataset);

        logger.LogInformation("Retrieved TRUD dataset file {DataSetFileName}", dataset.Name);

        var memoryStream = new MemoryStream();
        await fullFileInputStream.CopyToAsync(memoryStream);

        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }
}
