using System;
using System.Linq;
using BuyingCatalogueFunction.OrganisationImport.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using Xunit;

namespace BuyingCatalogueFunctionTests.OrganisationImport.Models;

public static class OdsOrganisationMappingTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_MapsCodeSystems(
        DateTime releaseDate,
        OrgRefData trudData,
        ILogger logger)
    {
        var roles = trudData.CodeSystems.CodeSystem.First();
        var relationships = trudData.CodeSystems.CodeSystem.Skip(1).First();

        roles.Name = TrudCodeSystemKeys.RolesKey;
        relationships.Name = TrudCodeSystemKeys.RelationshipKey;

        var odsMapping = new OdsOrganisationMapping(releaseDate, trudData, logger);

        odsMapping.ReleaseDate.Should().Be(releaseDate);
        odsMapping.RoleTypes.Should()
            .BeEquivalentTo(roles.Concept.Select(x => new RoleType { Id = x.Id, Description = x.DisplayName }));
        odsMapping.RelationshipTypes.Should()
            .BeEquivalentTo(relationships.Concept.Select(x => new RoleType { Id = x.Id, Description = x.DisplayName }));
    }

    [Theory]
    [MockAutoData]
    public static void Construct_SetsOrganisations(
        DateTime releaseDate,
        OrgRefData trudData,
        ILogger logger)
    {
        var roles = trudData.CodeSystems.CodeSystem.First();
        var relationships = trudData.CodeSystems.CodeSystem.Skip(1).First();

        roles.Name = TrudCodeSystemKeys.RolesKey;
        relationships.Name = TrudCodeSystemKeys.RelationshipKey;

        var expectedOrganisations = trudData.OrganisationsRoot.Organisations.Select(x => new OdsOrganisation
        {
            Id = x.OrgId.Extension,
            Name = x.Name,
            IsActive = string.Equals(
                x.Status.Value,
                TrudCodeSystemKeys.ActiveStatus,
                StringComparison.OrdinalIgnoreCase),
            AddressLine1 = x.GeoLoc.Location.AddrLn1,
            AddressLine2 = x.GeoLoc.Location.AddrLn2,
            AddressLine3 = x.GeoLoc.Location.AddrLn3,
            Town = x.GeoLoc.Location.Town,
            County = x.GeoLoc.Location.County,
            Postcode = x.GeoLoc.Location.PostCode,
            Country = x.GeoLoc.Location.Country,
        }).ToList();

        var odsMapping = new OdsOrganisationMapping(releaseDate, trudData, logger);

        odsMapping.OdsOrganisations.Should().BeEquivalentTo(expectedOrganisations);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_SetsOrganisationRelationships(
        DateTime releaseDate,
        OrgRefData trudData,
        ILogger logger)
    {
        trudData.OrganisationsRoot.Organisations.ForEach(x =>
            x.RelationshipsRoot.Relationship = []);
        var roles = trudData.CodeSystems.CodeSystem.First();
        var relationships = trudData.CodeSystems.CodeSystem.Skip(1).First();

        roles.Name = TrudCodeSystemKeys.RolesKey;
        relationships.Name = TrudCodeSystemKeys.RelationshipKey;

        var parentOrganisation = trudData.OrganisationsRoot.Organisations.First();
        var childOrganisation = trudData.OrganisationsRoot.Organisations.Skip(1).First();

        var relationship = new Relationship()
        {
            UniqueRelId = 5,
            Id = "RE4",
            Target = new Target { OrgId = parentOrganisation.OrgId },
            Status = new Status { Value = "Active" },
        };

        childOrganisation.RelationshipsRoot.Relationship = [relationship];

        var expectedRelationship = new OrganisationRelationship
        {
            Id = relationship.UniqueRelId,
            RelationshipTypeId = relationship.Id,
            OwnerOrganisationId = parentOrganisation.OrgId.Extension,
            TargetOrganisationId = childOrganisation.OrgId.Extension,
            IsActive = true,
        };

        var odsMapping = new OdsOrganisationMapping(releaseDate, trudData, logger);

        odsMapping.OrganisationRelationships.Should().ContainSingle();
        odsMapping.OrganisationRelationships.First().Should().BeEquivalentTo(expectedRelationship);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_InvalidParentId_NoRelationshipsDefined(
        DateTime releaseDate,
        OrgRefData trudData,
        ILogger logger)
    {
        trudData.OrganisationsRoot.Organisations.ForEach(x =>
            x.RelationshipsRoot.Relationship = []);
        var roles = trudData.CodeSystems.CodeSystem.First();
        var relationships = trudData.CodeSystems.CodeSystem.Skip(1).First();

        roles.Name = TrudCodeSystemKeys.RolesKey;
        relationships.Name = TrudCodeSystemKeys.RelationshipKey;

        var childOrganisation = trudData.OrganisationsRoot.Organisations.Skip(1).First();

        var relationship = new Relationship()
        {
            UniqueRelId = 5,
            Id = "RE4",
            Target = new Target { OrgId = new() { Extension = Guid.NewGuid().ToString() } },
            Status = new Status { Value = "Active" },
        };

        childOrganisation.RelationshipsRoot.Relationship = [relationship];

        var odsMapping = new OdsOrganisationMapping(releaseDate, trudData, logger);

        odsMapping.OrganisationRelationships.Should().BeEmpty();
    }

    [Theory]
    [MockAutoData]
    public static void Construct_SetsOrganisationRoles(
        DateTime releaseDate,
        RolesRoot rolesRoot,
        OrgRefData trudData,
        ILogger logger)
    {
        trudData.OrganisationsRoot.Organisations.ForEach(x =>
        {
            x.RelationshipsRoot.Relationship = [];
            x.RolesRoot.Roles = [];
        });

        var roles = trudData.CodeSystems.CodeSystem.First();
        var relationships = trudData.CodeSystems.CodeSystem.Skip(1).First();

        roles.Name = TrudCodeSystemKeys.RolesKey;
        relationships.Name = TrudCodeSystemKeys.RelationshipKey;

        var organisation = trudData.OrganisationsRoot.Organisations.First();
        organisation.RolesRoot = rolesRoot;

        var expectedRoles = rolesRoot.Roles.Select(x => new OrganisationRole
        {
            Id = x.UniqueRoleId,
            RoleId = x.Id,
            OrganisationId = organisation.OrgId.Extension,
            IsPrimaryRole = x.PrimaryRole,
            IsActive = string.Equals(
                x.Status.Value,
                TrudCodeSystemKeys.ActiveStatus,
                StringComparison.OrdinalIgnoreCase),
        });

        var odsMapping = new OdsOrganisationMapping(releaseDate, trudData, logger);

        odsMapping.OrganisationRoles.Should().HaveCount(rolesRoot.Roles.Count);
        odsMapping.OrganisationRoles.Should().BeEquivalentTo(expectedRoles);
    }
}
