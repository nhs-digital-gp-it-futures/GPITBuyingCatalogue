using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.Services.Organisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;
using MappedOdsOrganisation = NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations.OdsOrganisation;
using OdsOrganisation = NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models.OdsOrganisation;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Organisations;

public class TrudOdsServiceTests
{
    [Theory]
    [InMemoryDbAutoData]
    public static async Task GetOrganisationByOdsCode_Null_ReturnsError(
        string odsCode,
        TrudOdsService service)
    {
        (MappedOdsOrganisation _, string error) = await service.GetOrganisationByOdsCode(odsCode);

        error.Should().Be(TrudOdsService.InvalidOrganisationError);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task GetOrganisationByOdsCode_Inactive_ReturnsError(
        OdsOrganisation organisation,
        [Frozen] BuyingCatalogueDbContext context,
        TrudOdsService service)
    {
        organisation.IsActive = false;

        context.Add(organisation);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        (MappedOdsOrganisation _, string error) = await service.GetOrganisationByOdsCode(organisation.Id);

        error.Should().Be(TrudOdsService.InvalidOrgTypeError);
    }

    [Theory]
    [InMemoryDbInlineAutoData(null, null)]
    [InMemoryDbInlineAutoData(null, "DEF")]
    [InMemoryDbInlineAutoData(null, "RO318")]
    [InMemoryDbInlineAutoData("ABC", null)]
    [InMemoryDbInlineAutoData("ABC", "DEF")]
    [InMemoryDbInlineAutoData("ABC", "RO318")]
    [InMemoryDbInlineAutoData("RO261", null)]
    [InMemoryDbInlineAutoData("RO261", "DEF")]
    public static async Task GetOrganisationByOdsCode_InvalidType_ReturnsError(
        string primaryRoleId,
        string secondaryRoleId,
        OdsOrganisation organisation,
        RoleType primaryRoleType,
        RoleType secondaryRoleType,
        [Frozen] BuyingCatalogueDbContext context,
        TrudOdsService service)
    {
        organisation.IsActive = true;
        organisation.Roles.Clear();
        AddRole(context, organisation, primaryRoleType, primaryRoleId, true);
        AddRole(context, organisation, secondaryRoleType, secondaryRoleId, false);
        context.Add(organisation);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        (MappedOdsOrganisation _, string error) = await service.GetOrganisationByOdsCode(organisation.Id);

        error.Should().Be(TrudOdsService.InvalidOrgTypeError);
    }

    [Theory]
    [InMemoryDbInlineAutoData("RO213", null)]
    [InMemoryDbInlineAutoData("RO261", "RO318")]
    [InMemoryDbInlineAutoData("RO177", "RO76")]
    public static async Task GetOrganisationByOdsCode_Valid_ReturnsExpected(
        string primaryRoleId,
        string secondaryRoleId,
        OdsOrganisation organisation,
        RoleType primaryRoleType,
        RoleType secondaryRoleType,
        [Frozen] BuyingCatalogueDbContext context,
        TrudOdsService service)
    {
        organisation.IsActive = true;
        organisation.Roles.Clear();
        AddRole(context, organisation, primaryRoleType, primaryRoleId, true);
        AddRole(context, organisation, secondaryRoleType, secondaryRoleId, false);
        context.Add(organisation);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var expectedOrg = new MappedOdsOrganisation
        {
            IsActive = organisation.IsActive,
            OdsCode = organisation.Id,
            OrganisationName = organisation.Name,
            PrimaryRoleId = primaryRoleId,
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

        (MappedOdsOrganisation mappedOrganisation, string _) = await service.GetOrganisationByOdsCode(organisation.Id);

        mappedOrganisation.Should().NotBeNull();
        mappedOrganisation.Should().BeEquivalentTo(expectedOrg);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static Task GetServiceRecipientsByParentInternalIdentifier_InvalidId_ThrowsException(
        string internalIdentifier,
        TrudOdsService service) =>
        service.Invoking(x => x.GetServiceRecipientsByParentInternalIdentifier(internalIdentifier))
            .Should()
            .ThrowAsync<ArgumentException>(TrudOdsService.InvalidIdExceptionMessage);

    [Theory]
    [InMemoryDbAutoData]
    public static async Task GetServiceRecipientsByParentInternalIdentifier_GPPractice_ReturnsItself(
        Organisation organisation,
        [Frozen] BuyingCatalogueDbContext context,
        [Frozen] OdsSettings settings,
        TrudOdsService service)
    {
        organisation.PrimaryRoleId = settings.GetPrimaryRoleId(OrganisationType.GP);

        context.Add(organisation);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var results = (await service.GetServiceRecipientsByParentInternalIdentifier(organisation.InternalIdentifier))
            .ToList();

        results.Should().NotBeEmpty();
        results.Should().HaveCount(1);
        results.First().Name.Should().Be(organisation.Name);
        results.First().OrgId.Should().Be(organisation.ExternalIdentifier);
        results.First().PrimaryRoleId.Should().Be(organisation.PrimaryRoleId);
        results.First().Location.Should().Be("GP Practice");
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task GetServiceRecipientsByParentInternalIdentifier_WithRelationships_ReturnsRecipients(
        RelationshipType relationshipType,
        RoleType roleType,
        Organisation organisation,
        OdsOrganisation subLocation,
        List<OdsOrganisation> organisations,
        [Frozen] OdsSettings settings,
        [Frozen] BuyingCatalogueDbContext context,
        TrudOdsService service)
    {
        var roleId = settings.GetPrimaryRoleId(OrganisationType.GP);
        var relationshipTypeId = settings.IsCommissionedByRelType;

        relationshipType.Id = relationshipTypeId;
        roleType.Id = roleId;

        organisations.ForEach(
            x =>
            {
                x.IsActive = true;
                x.Roles = new List<OrganisationRole> { new(x.Id, roleId) };
            });

        subLocation.IsActive = true;
        subLocation.Roles = new List<OrganisationRole> { new(subLocation.Id, settings.SubLocationRoleId) };

        var subLocationRelationship = new OrganisationRelationship(
            settings.InGeographyOfRelType,
            organisation.ExternalIdentifier,
            subLocation.Id);

        var organisationRelationships =
            organisations.Select(x => new OrganisationRelationship(relationshipTypeId, subLocation.Id, x.Id))
                .ToList();

        context.Organisations.Add(organisation);
        context.OrganisationRelationshipTypes.Add(relationshipType);
        context.OrganisationRoleTypes.Add(roleType);
        context.OdsOrganisations.AddRange(organisations);
        context.OdsOrganisations.Add(subLocation);
        context.OrganisationRelationships.AddRange(organisationRelationships);
        context.OrganisationRelationships.Add(subLocationRelationship);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var results = (await service.GetServiceRecipientsByParentInternalIdentifier(organisation.InternalIdentifier))
            .ToList();

        results.Should().NotBeEmpty();
        results.Should().HaveCount(organisations.Count);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task UpdateOrganisationDetails_InvalidOdsOrganisation_Returns(
        string odsCode,
        [Frozen] Mock<IOrganisationsService> organisationsService,
        TrudOdsService service)
    {
        await service.UpdateOrganisationDetails(odsCode);

        organisationsService.Verify(x => x.UpdateOrganisation(It.IsAny<MappedOdsOrganisation>()), Times.Never());
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task UpdateOrganisationDetails_InvalidTrudOrganisation_Returns(
        Organisation organisation,
        [Frozen] Mock<IOrganisationsService> organisationsService,
        [Frozen] BuyingCatalogueDbContext context,
        TrudOdsService service)
    {
        context.Organisations.Add(organisation);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        await service.UpdateOrganisationDetails(organisation.InternalIdentifier);

        organisationsService.Verify(x => x.UpdateOrganisation(It.IsAny<MappedOdsOrganisation>()), Times.Never());
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task UpdateOrganisationDetails_ValidOrganisation_UpdatesOrganisationDetails(
        Organisation organisation,
        OdsOrganisation trudOrganisation,
        [Frozen] Mock<IOrganisationsService> organisationsService,
        [Frozen] BuyingCatalogueDbContext context,
        TrudOdsService service)
    {
        trudOrganisation.Id = organisation.ExternalIdentifier;

        context.Organisations.Add(organisation);
        context.OdsOrganisations.Add(trudOrganisation);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        MappedOdsOrganisation actual = null;

        organisationsService
            .Setup(x => x.UpdateOrganisation(It.IsAny<MappedOdsOrganisation>()))
            .Callback<MappedOdsOrganisation>(x => actual = x);

        await service.UpdateOrganisationDetails(organisation.ExternalIdentifier);

        organisationsService.VerifyAll();

        actual.Should().BeEquivalentTo(TrudOdsService.MapOrganisation(trudOrganisation));
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task GetServiceRecipientsById_InvalidOrganisation_ReturnsEmpty(
        string internalOrgId,
        TrudOdsService service)
    {
        var result = await service.GetServiceRecipientsById(internalOrgId, Enumerable.Empty<string>());

        result.Should().BeEmpty();
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task GetServiceRecipientsById_ReturnsExpected(
        RelationshipType relationshipType,
        RoleType roleType,
        Organisation organisation,
        OdsOrganisation subLocation,
        List<OdsOrganisation> organisations,
        [Frozen] OdsSettings settings,
        [Frozen] BuyingCatalogueDbContext context,
        TrudOdsService service)
    {
        var roleId = settings.GetPrimaryRoleId(OrganisationType.GP);
        var relationshipTypeId = settings.IsCommissionedByRelType;

        relationshipType.Id = relationshipTypeId;
        roleType.Id = roleId;

        organisations.ForEach(
            x =>
            {
                x.IsActive = true;
                x.Roles = new List<OrganisationRole> { new(x.Id, roleId) };
            });

        subLocation.IsActive = true;
        subLocation.Roles = new List<OrganisationRole> { new(subLocation.Id, settings.SubLocationRoleId) };

        var subLocationRelationship = new OrganisationRelationship(
            settings.InGeographyOfRelType,
            organisation.ExternalIdentifier,
            subLocation.Id);

        var organisationRelationships =
            organisations.Select(x => new OrganisationRelationship(relationshipTypeId, subLocation.Id, x.Id))
                .ToList();

        context.Organisations.Add(organisation);
        context.OrganisationRelationshipTypes.Add(relationshipType);
        context.OrganisationRoleTypes.Add(roleType);
        context.OdsOrganisations.AddRange(organisations);
        context.OdsOrganisations.Add(subLocation);
        context.OrganisationRelationships.AddRange(organisationRelationships);
        context.OrganisationRelationships.Add(subLocationRelationship);

        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var selectedOrgs = organisations.Take(2).ToList();

        var result = await service.GetServiceRecipientsById(organisation.InternalIdentifier, selectedOrgs.Select(x => x.Id));

        result.Should().NotBeEmpty();
        result.Should()
            .BeEquivalentTo(
                selectedOrgs.Select(
                    x => new ServiceRecipient { Name = x.Name, OrgId = x.Id, Location = subLocation.Name, }),
                opt => opt.Excluding(m => m.PrimaryRoleId));
    }

    private static void AddRole(DbContext context, OdsOrganisation organisation, RoleType roleType, string roleId, bool isPrimaryRole)
    {
        if (roleId is null) return;

        roleType.Id = roleId;
        var role = new OrganisationRole
        {
            OrganisationId = organisation.Id,
            RoleId = roleType.Id,
            IsPrimaryRole = isPrimaryRole,
        };

        organisation.Roles.Add(role);
        context.Add(roleType);
    }
}
