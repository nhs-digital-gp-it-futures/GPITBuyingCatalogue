using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using BuyingCatalogueFunction.OrganisationImport.Interfaces;
using BuyingCatalogueFunction.OrganisationImport.Models;
using Microsoft.Data.SqlClient;
using MoreLinq;

namespace BuyingCatalogueFunction.OrganisationImport.Services;

public class TrudService(
    BuyingCatalogueDbContext dbContext,
    IHttpService httpService,
    IZipService zipService,
    ILogger<TrudService> logger)
    : ITrudService
{
    private readonly BuyingCatalogueDbContext dbContext =
        dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    private readonly IHttpService httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
    private readonly IZipService zipService = zipService ?? throw new ArgumentNullException(nameof(zipService));
    private readonly ILogger<TrudService> logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<bool> HasImportedLatestRelease(DateTime latestReleaseDate)
        => await dbContext.OrgImportJournal.AnyAsync(x => x.ImportDate >= latestReleaseDate);

    public async Task<OrgRefData> GetTrudDataAsync(Uri url)
    {
        await using var zipStream = await httpService.DownloadAsync(url);
        if (zipStream is null)
            return null;

        await using var dataStream = await zipService.GetTrudDataFileAsync(zipStream);
        if (dataStream is null)
            return null;

        var orgRefData = DeserializeStream(dataStream);

        return orgRefData;
    }

    [ExcludeFromCodeCoverage(Justification = "Can't unit test due to dependency on relational database")]
    public async Task SaveTrudDataAsync(OdsOrganisationMapping mappedData)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            await BulkWriteToDatabase(mappedData.RoleTypes);
            await BulkWriteToDatabase(mappedData.RelationshipTypes);
            await BulkWriteToDatabase(mappedData.OdsOrganisations);
            await BulkWriteToDatabase(mappedData.OrganisationRoles);
            await BulkWriteToDatabase(mappedData.OrganisationRelationships);

            await dbContext.Database.ExecuteSqlRawAsync("EXEC ods_organisations.MergeTrudData");

            dbContext.OrgImportJournal.Add(new OrgImportJournal(mappedData.ReleaseDate));

            await dbContext.SaveChangesAsync();

            await transaction.CommitAsync();

            logger.LogInformation("Committed TRUD changes to database");
        }
        catch (Exception ex)
        {
            logger.LogError("Error occurred when updating database {@Exception}", ex);

            await transaction.RollbackAsync();
        }
    }

    [ExcludeFromCodeCoverage(Justification = "Can't unit test due to dependency on relational database")]
    private async Task BulkWriteToDatabase<T>(IReadOnlyCollection<T> entities)
    {
        var entityType = dbContext.Model.FindEntityType(typeof(T));
        if (entityType is null) throw new InvalidOperationException();

        var entityProperties = entityType.GetProperties().Select(x => x.Name).ToArray();

        var dataTable = MapToDataTable(entities, entityProperties);

        using var bulkCopy = new SqlBulkCopy(dbContext.Database.GetConnectionString());

        bulkCopy.ColumnMappings.Clear();
        entityProperties.ForEach(x => bulkCopy.ColumnMappings.Add(x, x));

        var tableName = entityType.GetSchemaQualifiedTableName();
        bulkCopy.DestinationTableName = $"{tableName}_Staging";

        await bulkCopy.WriteToServerAsync(dataTable);
    }

    private static DataTable MapToDataTable<T>(IReadOnlyCollection<T> entities, string[] scalarProperties)
    {
        var dataTable = new DataTable();
        dataTable.Columns.AddRange(scalarProperties.Select(x => new DataColumn(x)).ToArray());
        foreach (var entity in entities)
        {
            var dataRow = dataTable.NewRow();
            foreach (var property in scalarProperties)
            {
                var objProperty = typeof(T).GetProperty(property);
                var value = objProperty?.GetValue(entity);

                dataRow[property] = value ?? DBNull.Value;
            }

            dataTable.Rows.Add(dataRow);
        }

        return dataTable;
    }

    private OrgRefData DeserializeStream(Stream stream)
    {
        var serializer = new XmlSerializer(typeof(OrgRefData));

        logger.LogInformation("Beginning deserialization of TRUD data");

        using var xmlReader = new XmlTextReader(stream);
        OrgRefData deserialized;
        try
        {
            deserialized = (OrgRefData)serializer.Deserialize(xmlReader);
        }
        catch (InvalidOperationException invalidOpEx)
        {
            logger.LogError("Failed to deserialize TRUD data {@Exception}", invalidOpEx);

            return null;
        }

        logger.LogInformation("Successfully deserialized TRUD data");

        return deserialized;
    }
}
