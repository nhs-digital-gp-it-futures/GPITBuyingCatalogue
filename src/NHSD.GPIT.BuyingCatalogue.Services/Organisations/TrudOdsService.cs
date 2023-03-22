using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using OdsOrganisation = NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations.OdsOrganisation;

namespace NHSD.GPIT.BuyingCatalogue.Services.Organisations;

public class TrudOdsService : IOdsService
{
    internal const string InvalidOdsOrganisation = "Couldn't find organisation with ODS Code {OdsCode}";
    internal const string InvalidTrudOrganisation = "Couldn't find organisation in TRUD dataset with ODS Code {OdsCode}";
    internal const string InvalidOrganisationError = "Organisation not found";
    internal const string InvalidOrgTypeError = "Not a buyer organisation";
    internal const string InvalidIdExceptionMessage = "Invalid ODS Code specified";
    internal const string RelationshipType = "RE4";

    private const string ActiveStatus = "Active";
    private const string InactiveStatus = "Inactive";

    private readonly OdsSettings settings;
    private readonly BuyingCatalogueDbContext context;
    private readonly IOrganisationsService organisationsService;
    private readonly ILogger<TrudOdsService> logger;

    public TrudOdsService(
        OdsSettings settings,
        BuyingCatalogueDbContext context,
        IOrganisationsService organisationsService,
        ILogger<TrudOdsService> logger)
    {
        this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        this.organisationsService =
            organisationsService ?? throw new ArgumentNullException(nameof(organisationsService));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<(OdsOrganisation Organisation, string Error)> GetOrganisationByOdsCode(string odsCode)
    {
        var organisation = await context.OdsOrganisations.Include(x => x.Roles)
            .FirstOrDefaultAsync(x => string.Equals(x.Id, odsCode));

        if (organisation is null)
            return (null, InvalidOrganisationError);

        if (!organisation.IsActive || !IsBuyerOrganisation(organisation))
            return (null, InvalidOrgTypeError);

        var mappedOrganisation = MapOrganisation(organisation);

        return (mappedOrganisation, null);
    }

    public async Task<IEnumerable<ServiceRecipient>> GetServiceRecipientsByParentInternalIdentifier(
        string internalIdentifier)
    {
        var organisation = await context.Organisations.FirstOrDefaultAsync(x => x.InternalIdentifier == internalIdentifier);
        if (organisation is null)
            throw new ArgumentException(InvalidIdExceptionMessage, nameof(internalIdentifier));

        var odsCode = organisation.ExternalIdentifier;

        var serviceRecipients = await context
            .OrganisationRelationships
            .Include(x => x.TargetOrganisation)
            .Where(
                x =>
                    x.RelationshipTypeId == RelationshipType
                    && x.OwnerOrganisationId == odsCode
                    && x.TargetOrganisation.Roles.Any(y => y.IsPrimaryRole && y.RoleId == settings.GpPracticeRoleId))
            .Select(
                x => new ServiceRecipient
                {
                    Name = x.TargetOrganisation.Name,
                    OrgId = x.TargetOrganisationId,
                    OrganisationRoleId = x.TargetOrganisation.Roles.FirstOrDefault(y => y.IsPrimaryRole).RoleId,
                    Status = x.TargetOrganisation.IsActive ? ActiveStatus : InactiveStatus,
                })
            .ToListAsync();

        return serviceRecipients;
    }

    public async Task UpdateOrganisationDetails(string odsCode)
    {
        var organisation = await context.Organisations.FirstOrDefaultAsync(x => x.ExternalIdentifier == odsCode);
        if (organisation == null)
        {
            logger.LogWarning(InvalidOdsOrganisation, odsCode);
            return;
        }

        var trudOrganisation =
            await context.OdsOrganisations.FirstOrDefaultAsync(x => x.Id == organisation.ExternalIdentifier);
        if (trudOrganisation == null)
        {
            logger.LogWarning(InvalidTrudOrganisation, odsCode);
            return;
        }

        await organisationsService.UpdateCcgOrganisation(MapOrganisation(trudOrganisation));
    }

    internal static OdsOrganisation
        MapOrganisation(EntityFramework.OdsOrganisations.Models.OdsOrganisation organisation) => new()
    {
        IsActive = organisation.IsActive,
        OdsCode = organisation.Id,
        OrganisationName = organisation.Name,
        OrganisationRoleId = GetOrganisationRoleId(organisation),
        Address = new()
        {
            Line1 = organisation.AddressLine1,
            Line2 = organisation.AddressLine2,
            Line3 = organisation.AddressLine3,
            Town = organisation.Town,
            County = organisation.County,
            Postcode = organisation.Postcode,
            Country = organisation.Country,
        },
    };

    private static string GetOrganisationRoleId(EntityFramework.OdsOrganisations.Models.OdsOrganisation organisation) =>
        organisation.Roles.FirstOrDefault(x => x.IsPrimaryRole)?.RoleId;

    private bool IsBuyerOrganisation(EntityFramework.OdsOrganisations.Models.OdsOrganisation organisation) =>
        settings.BuyerOrganisationRoleIds.Contains(GetOrganisationRoleId(organisation));
}
