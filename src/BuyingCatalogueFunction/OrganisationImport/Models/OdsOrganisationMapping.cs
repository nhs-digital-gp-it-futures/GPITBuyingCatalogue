using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;

namespace BuyingCatalogueFunction.OrganisationImport.Models;

public class OdsOrganisationMapping
{
    public OdsOrganisationMapping(
        DateTime releaseDate,
        OrgRefData trudData,
        ILogger logger)
    {
        var roleTypes = MapCodeSystemTo<RoleType>(trudData, TrudCodeSystemKeys.RolesKey);

        logger.LogInformation("Mapped Role Types {@RoleTypes}", roleTypes);

        var relationshipTypes = MapCodeSystemTo<RelationshipType>(trudData, TrudCodeSystemKeys.RelationshipKey);

        logger.LogInformation("Mapped Relationship Types {@RelationshipTypes}", relationshipTypes);

        var mappedOrganisations = MapToOdsOrganisations(trudData);

        logger.LogInformation("Mapped {Count} number of organisations", mappedOrganisations.Values.Count.ToString("#,##"));

        var mappedRelationships =
            MapOrganisationRelationships(trudData.OrganisationsRoot.Organisations, mappedOrganisations, logger)
                .ToList();

        logger.LogInformation("Mapped {Count} number of active organisation relationships",
            mappedRelationships.Count.ToString("#,##"));

        var mappedRoles = MapOrganisationRoles(trudData.OrganisationsRoot.Organisations).ToList();

        logger.LogInformation("Mapped {Count} number of organisation roles", mappedRoles.Count.ToString("#,##"));

        ReleaseDate = releaseDate;
        RoleTypes = roleTypes.ToList();
        RelationshipTypes = relationshipTypes.ToList();
        OdsOrganisations = mappedOrganisations.Values;
        OrganisationRelationships = mappedRelationships;
        OrganisationRoles = mappedRoles;
    }

    public DateTime ReleaseDate { get; }

    public IReadOnlyCollection<RoleType> RoleTypes { get; }

    public IReadOnlyCollection<RelationshipType> RelationshipTypes { get; }

    public IReadOnlyCollection<OdsOrganisation> OdsOrganisations { get; }

    public IReadOnlyCollection<OrganisationRelationship> OrganisationRelationships { get; }

    public IReadOnlyCollection<OrganisationRole> OrganisationRoles { get; }

    private static Dictionary<string, OdsOrganisation> MapToOdsOrganisations(OrgRefData trudData)
    {
        static OdsOrganisation MapToOdsOrganisationInternal(Organisation trudOrganisation)
        {
            static void MapIdentifiers(OdsOrganisation odsOrganisation, Organisation trudOrganisation)
            {
                odsOrganisation.Id = trudOrganisation.OrgId.Extension;
                odsOrganisation.Name = trudOrganisation.Name;
                odsOrganisation.IsActive = string.Equals(
                    trudOrganisation.Status.Value,
                    TrudCodeSystemKeys.ActiveStatus,
                    StringComparison.OrdinalIgnoreCase);
            }

            static void MapAddress(OdsOrganisation odsOrganisation, Organisation trudOrganisation)
            {
                var trudAddress = trudOrganisation.GeoLoc.Location;

                odsOrganisation.AddressLine1 = trudAddress.AddrLn1;
                odsOrganisation.AddressLine2 = trudAddress.AddrLn2;
                odsOrganisation.AddressLine3 = trudAddress.AddrLn3;
                odsOrganisation.Town = trudAddress.Town;
                odsOrganisation.County = trudAddress.County;
                odsOrganisation.Postcode = trudAddress.PostCode;
                odsOrganisation.Country = trudAddress.Country;
            }

            var odsOrganisation = new OdsOrganisation();
            MapIdentifiers(odsOrganisation, trudOrganisation);
            MapAddress(odsOrganisation, trudOrganisation);

            return odsOrganisation;
        }

        return trudData.OrganisationsRoot.Organisations.ToDictionary(x => x.OrgId.Extension, v => MapToOdsOrganisationInternal(v));
    }

    private static IEnumerable<OrganisationRelationship> MapOrganisationRelationships(
        IEnumerable<Organisation> trudOrganisations,
        Dictionary<string, OdsOrganisation> mappedOrganisations,
        ILogger logger)
    {
        foreach (var trudOrganisation in trudOrganisations.Where(x => x.RelationshipsRoot?.Relationship?.Count > 0))
        {
            foreach (var trudRelationship in trudOrganisation.RelationshipsRoot.Relationship.Where(x => string.Equals(TrudCodeSystemKeys.ActiveStatus, x.Status.Value)))
            {
                var parentOrgId = trudRelationship.Target.OrgId.Extension;
                if (!mappedOrganisations.TryGetValue(parentOrgId, out _))
                {
                    logger.LogWarning("Relationship with no valid parent, Parent ID {ParentID}\r\nChild ID {ChildID}",
                        parentOrgId, trudOrganisation.OrgId.Extension);

                    continue;
                }

                var relationship = new OrganisationRelationship
                {
                    Id = trudRelationship.UniqueRelId,
                    RelationshipTypeId = trudRelationship.Id,
                    OwnerOrganisationId = parentOrgId,
                    TargetOrganisationId = trudOrganisation.OrgId.Extension,
                };

                yield return relationship;
            }
        }
    }

    private static IEnumerable<OrganisationRole> MapOrganisationRoles(IEnumerable<Organisation> trudOrganisations)
    {
        foreach (var trudOrganisation in trudOrganisations.Where(x => x.RolesRoot?.Roles?.Count > 0))
        {
            foreach (var role in trudOrganisation.RolesRoot.Roles)
            {
                yield return new()
                {
                    Id = role.UniqueRoleId,
                    RoleId = role.Id,
                    OrganisationId = trudOrganisation.OrgId.Extension,
                    IsPrimaryRole = role.PrimaryRole
                };
            }
        }
    }

    private static IEnumerable<T> MapCodeSystemTo<T>(OrgRefData trudData, string key)
        where T : ILookupDefinition, new()
        => trudData.CodeSystems.CodeSystem.First(x => string.Equals(x.Name, key))
            .Concept.Select(x => new T { Id = x.Id, Description = x.DisplayName });
}
