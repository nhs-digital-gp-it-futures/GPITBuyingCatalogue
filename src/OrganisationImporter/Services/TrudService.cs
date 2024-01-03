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
            await HandleOrganisationRoleTypes(mappedData.RoleTypes);
            await HandleOrganisationRelationshipTypes(mappedData.RelationshipTypes);
            await HandleOrganisations(mappedData.OdsOrganisations);

            await _dbContext.SaveChangesAsync();

            await HandleOrganisationRoles(mappedData.OrganisationRoles);
            await HandleOrganisationRelationships(mappedData.OrganisationRelationships);

            await _dbContext.SaveChangesAsync();

            _dbContext.OrgImportJournal.Add(new());

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

    private async Task HandleOrganisationRoleTypes(IEnumerable<RoleType> roleTypes)
    {
        foreach (var roleType in roleTypes)
        {
            var existing = await _dbContext.OrganisationRoleTypes.FirstOrDefaultAsync(x => x.Id == roleType.Id);
            if (existing is not null)
            {
                existing.Description = roleType.Description;
                continue;
            }

            _dbContext.OrganisationRoleTypes.Add(roleType);
        }
    }

    private async Task HandleOrganisationRelationshipTypes(IEnumerable<RelationshipType> relationshipTypes)
    {
        foreach (var relationshipType in relationshipTypes)
        {
            var existing = await _dbContext.OrganisationRelationshipTypes.FirstOrDefaultAsync(x => x.Id == relationshipType.Id);
            if (existing is not null)
            {
                existing.Description = relationshipType.Description;
                continue;
            }

            _dbContext.OrganisationRelationshipTypes.Add(relationshipType);
        }
    }

    private async Task HandleOrganisations(IEnumerable<OdsOrganisation> organisations)
    {
        foreach (var organisation in organisations)
        {
            var existing = await _dbContext.OdsOrganisations.FirstOrDefaultAsync(x => x.Id == organisation.Id);
            if (existing is not null)
            {
                existing.UpdateFrom(organisation);
                continue;
            }

            _dbContext.OdsOrganisations.Add(organisation);
        }
    }

    private async Task HandleOrganisationRoles(IEnumerable<OrganisationRole> organisationRoles)
    {
        foreach (var organisationRole in organisationRoles)
        {
            var existing = await _dbContext.OrganisationRoles.FirstOrDefaultAsync(x => x.Id == organisationRole.Id);
            if (existing is not null)
            {
                existing.IsPrimaryRole = organisationRole.IsPrimaryRole;

                continue;
            }

            _dbContext.OrganisationRoles.Add(organisationRole);
        }
    }

    private async Task HandleOrganisationRelationships(IEnumerable<OrganisationRelationship> organisationRelationships)
    {
        foreach (var organisationRelationship in organisationRelationships)
        {
            var existing =
                await _dbContext.OrganisationRelationships.FirstOrDefaultAsync(x =>
                    x.Id == organisationRelationship.Id);
            if (existing is not null)
            {
                existing.TargetOrganisationId = organisationRelationship.TargetOrganisationId;
                existing.OwnerOrganisationId = organisationRelationship.OwnerOrganisationId;
                existing.RelationshipTypeId = organisationRelationship.RelationshipTypeId;
                continue;
            }

            _dbContext.OrganisationRelationships.Add(organisationRelationship);
        }
    }
}
