using System.Xml;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using OrganisationImporter.Interfaces;
using OrganisationImporter.Models;

namespace OrganisationImporter.Services;

public class TrudService : ITrudService
{

    private readonly BuyingCatalogueDbContext _dbContext;
    private readonly IHttpService _httpService;
    private readonly IZipService _zipService;
    private readonly ILogger<TrudService> _logger;

    public TrudService(
        BuyingCatalogueDbContext dbContext,
        IHttpService httpService,
        IZipService zipService,
        ILogger<TrudService> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
        _zipService = zipService ?? throw new ArgumentNullException(nameof(zipService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<OrgRefData> GetTrudDataAsync(Uri url)
    {
        await using var zipStream = await _httpService.DownloadAsync(url);
        if (zipStream is null)
            return null;

        await using var dataStream = await _zipService.GetTrudDataFileAsync(zipStream);
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

            if (mappedData.OrganisationRelationships.Any())
                _dbContext.OrganisationRelationships.AddRange(mappedData.OrganisationRelationships);

            await _dbContext.SaveChangesAsync();

            await transaction.CommitAsync();

            _logger.LogInformation("Committed TRUD changes to database");
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occurred when updating database {@Exception}", ex);

            await transaction.RollbackAsync();
        }
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
            var tableName = dbContext.Database.IsSqlServer()
                ? entityType.GetSchemaQualifiedTableName()
                : entityType.GetTableName();

            await dbContext.Database.ExecuteSqlRawAsync($"DELETE FROM {tableName}");
        }

        await dbContext.SaveChangesAsync();
    }
}
