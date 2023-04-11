using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
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

        if (organisation.PrimaryRoleId == settings.GetPrimaryRoleId(OrganisationType.GP))
        {
            return new List<ServiceRecipient>()
            {
                new()
                {
                    Name = organisation.Name,
                    OrgId = organisation.ExternalIdentifier,
                    PrimaryRoleId = organisation.PrimaryRoleId,
                    Location = "GP Practice",
                },
            };
        }

        var subLocations = await context.OrganisationRelationships
            .AsNoTracking()
            .Where(x => x.OwnerOrganisationId == organisation.ExternalIdentifier
                && x.RelationshipTypeId == settings.InGeographyOfRelType
                && x.TargetOrganisation.IsActive
                && x.TargetOrganisation.Roles.Any(y => y.RoleId == settings.SubLocationRoleId))
            .Select(x => x.TargetOrganisation.Id)
            .ToListAsync();

        var serviceRecipients = await context.OrganisationRelationships.AsNoTracking()
            .Where(
                x => subLocations.Contains(x.OwnerOrganisationId)
                    && x.TargetOrganisation.IsActive
                    && x.RelationshipTypeId == settings.IsCommissionedByRelType
                    && x.TargetOrganisation.Roles.Any(y => y.RoleId == settings.GetPrimaryRoleId(OrganisationType.GP)))
            .Select(
                x => new ServiceRecipient
                {
                    Name = x.TargetOrganisation.Name,
                    OrgId = x.TargetOrganisationId,
                    PrimaryRoleId = x.TargetOrganisation.Roles.FirstOrDefault(y => y.IsPrimaryRole).RoleId,
                    Location = x.OwnerOrganisation.Name,
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
        PrimaryRoleId = GetPrimaryRoleId(organisation),
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

    private static string GetPrimaryRoleId(EntityFramework.OdsOrganisations.Models.OdsOrganisation organisation) =>
        organisation.Roles.FirstOrDefault(x => x.IsPrimaryRole)?.RoleId;

    private static bool HasSecondaryRole(EntityFramework.OdsOrganisations.Models.OdsOrganisation organisation, string roleId) =>
        organisation.Roles.Any(x => !x.IsPrimaryRole && x.RoleId == roleId);

    private bool IsBuyerOrganisation(EntityFramework.OdsOrganisations.Models.OdsOrganisation organisation)
    {
        return settings.BuyerOrganisationRoles.Any(
            x => x.PrimaryRoleId == GetPrimaryRoleId(organisation)
                && (x.SecondaryRoleId == null || HasSecondaryRole(organisation, x.SecondaryRoleId)));
    }
}
