using System.Xml;
using System.Xml.Serialization;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using OrganisationImporter.Interfaces;
using OrganisationImporter.Models;

namespace OrganisationImporter.Services;

public class TrudService : ITrudService
{
    private const string NestedZipFileName = "fullfile.zip";
    private const string XmlFileExtension = ".xml";

    private readonly BuyingCatalogueDbContext _dbContext;
    private readonly IHttpService _httpService;
    private readonly ILogger<TrudService> _logger;

    public TrudService(
        BuyingCatalogueDbContext dbContext,
        IHttpService httpService,
        ILogger<TrudService> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<OrgRefData> GetTrudData(Uri url)
    {
        await using var dataStream = await GetDataStreamAsync(url);
        if (dataStream is null)
            return null;

        var orgRefData = DeserializeStream(dataStream);

        return orgRefData;
    }

    public async Task SaveTrudDataAsync(OdsOrganisationMapping mappedData)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            await ClearExistingEntitiesAsync(_dbContext);

            _dbContext.OrganisationRoleTypes.AddRange(mappedData.RoleTypes);
            _dbContext.OrganisationRelationshipTypes.AddRange(mappedData.RelationshipTypes);
            _dbContext.OdsOrganisations.AddRange(mappedData.OdsOrganisations);

            await _dbContext.SaveChangesAsync();

            _dbContext.OrganisationRoles.AddRange(mappedData.OrganisationRoles);
            _dbContext.OrganisationRelationships.AddRange(mappedData.OrganisationRelationships);

            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occurred when updating database {@Exception}", ex);

            await transaction.RollbackAsync();
        }
    }

    private async Task<Stream> GetDataStreamAsync(Uri url)
    {
        await using var zipStream = await _httpService.DownloadAsync(url);

        // Open the main Zip file and retrieve the nested Zip
        using var zipFile = new ZipFile(zipStream);

        var nestedZipFile = zipFile.GetEntry(NestedZipFileName);
        if (nestedZipFile is null)
        {
            _logger.LogError("Couldn't find {NestedZipFileName} in {ParentZipFileName}", NestedZipFileName,
                zipFile.Name);

            return null;
        }

        _logger.LogInformation("Retrieved nested zip file {NestedZipFileName}", nestedZipFile.Name);

        // Open the nested Zip and get the first XML file.
        // The file name isn't predictable and so GetEntry can't be used
        await using var fullFileStream = zipFile.GetInputStream(nestedZipFile);
        using var fullFileZip = new ZipFile(fullFileStream);

        // If there is more than 1 XML file, we can't predict which one to use
        var zipEntries = fullFileZip.Cast<ZipEntry>().Where(x => x.IsFile && x.Name.EndsWith(XmlFileExtension))
            .ToArray();

        if (zipEntries.Length > 1)
        {
            _logger.LogError("Nested archive contains more than one XML file. {@Files}",
                zipEntries.Select(x => x.Name));

            return null;
        }

        var dataset = zipEntries.First();
        await using var fullFileInputStream = fullFileZip.GetInputStream(dataset);

        _logger.LogInformation("Retrieved TRUD dataset file {DataSetFileName}", dataset.Name);

        var memoryStream = new MemoryStream();
        await fullFileInputStream.CopyToAsync(memoryStream);

        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }

    private OrgRefData DeserializeStream(Stream stream)
    {
        var serializer = new XmlSerializer(typeof(OrgRefData));

        _logger.LogInformation("Beginning deserialization of TRUD data");

        using var xmlReader = new XmlTextReader(stream);
        OrgRefData deserialized;
        try
        {
            deserialized = (OrgRefData)serializer.Deserialize(xmlReader);
        }
        catch (InvalidOperationException invalidOpEx)
        {
            _logger.LogError("Failed to deserialize TRUD data {@Exception}", invalidOpEx);

            return null;
        }

        _logger.LogInformation("Successfully deserialized TRUD data");

        return deserialized;
    }

    private static async Task ClearExistingEntitiesAsync(DbContext dbContext)
    {
        var entitiesToClear = new[]
        {
            typeof(OrganisationRelationship),
            typeof(OrganisationRole),
            typeof(OdsOrganisation),
            typeof(RelationshipType),
            typeof(RoleType),
        };

        foreach (var entityToClear in entitiesToClear)
        {
            var entityType = dbContext.Model.FindEntityType(entityToClear)!;
            await dbContext.Database.ExecuteSqlRawAsync($"DELETE FROM {entityType.GetSchemaQualifiedTableName()}");
        }

        await dbContext.SaveChangesAsync();
    }
}
