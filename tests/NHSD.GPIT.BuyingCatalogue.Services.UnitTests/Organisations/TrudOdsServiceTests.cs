using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.Services.Organisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;
using OdsOrganisation = NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations.OdsOrganisation;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Organisations;

public class TrudOdsServiceTests
{
    [Theory]
    [InMemoryDbAutoData]
    public static async Task GetOrganisationByOdsCode_Null_ReturnsError(
        string odsCode,
        TrudOdsService service)
    {
        (OdsOrganisation _, string error) = await service.GetOrganisationByOdsCode(odsCode);

        error.Should().Be(TrudOdsService.InvalidOrganisationError);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task GetOrganisationByOdsCode_Inactive_ReturnsError(
        EntityFramework.OdsOrganisations.Models.OdsOrganisation organisation,
        [Frozen] BuyingCatalogueDbContext context,
        TrudOdsService service)
    {
        organisation.IsActive = false;

        context.Add(organisation);
        await context.SaveChangesAsync();

        (OdsOrganisation _, string error) = await service.GetOrganisationByOdsCode(organisation.Id);

        error.Should().Be(TrudOdsService.InvalidOrgTypeError);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task GetOrganisationByOdsCode_InvalidType_ReturnsError(
        EntityFramework.OdsOrganisations.Models.OdsOrganisation organisation,
        OrganisationRole organisationRole,
        [Frozen] BuyingCatalogueDbContext context,
        TrudOdsService service)
    {
        organisation.IsActive = true;
        organisation.Roles.Add(organisationRole);

        context.Add(organisation);
        await context.SaveChangesAsync();

        (OdsOrganisation _, string error) = await service.GetOrganisationByOdsCode(organisation.Id);

        error.Should().Be(TrudOdsService.InvalidOrgTypeError);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task GetOrganisationByOdsCode_Valid_ReturnsExpected(
        EntityFramework.OdsOrganisations.Models.OdsOrganisation organisation,
        RoleType roleType,
        [Frozen] OdsSettings settings,
        [Frozen] BuyingCatalogueDbContext context,
        TrudOdsService service)
    {
        roleType.Id = settings.BuyerOrganisationRoleIds[0];
        var organisationRole = new OrganisationRole
        {
            RoleId = roleType.Id, IsPrimaryRole = true, OrganisationId = organisation.Id,
        };

        organisation.IsActive = true;
        organisation.Roles.Clear();

        context.Add(roleType);
        context.Add(organisation);
        context.Add(organisationRole);
        await context.SaveChangesAsync();

        (OdsOrganisation mappedOrganisation, string _) = await service.GetOrganisationByOdsCode(organisation.Id);

        mappedOrganisation.Should().NotBeNull();

        var expectedOrg = new OdsOrganisation
        {
            IsActive = organisation.IsActive,
            OdsCode = organisation.Id,
            OrganisationName = organisation.Name,
            PrimaryRoleId = organisationRole.RoleId,
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
    public static async Task GetServiceRecipientsByParentInternalIdentifier_WithRelationships_ReturnsRecipients(
        EntityFramework.OdsOrganisations.Models.OdsOrganisation grandparentOrganisation,
        EntityFramework.OdsOrganisations.Models.OdsOrganisation parentOrganisation,
        List<EntityFramework.OdsOrganisations.Models.OdsOrganisation> childOrganisations,
        Organisation organisation,
        RoleType gpRoleType,
        RelationshipType commByRelType,
        RoleType subLocationRoleType,
        RelationshipType geogOfRelType,
        [Frozen] OdsSettings settings,
        [Frozen] BuyingCatalogueDbContext context,
        TrudOdsService service)
    {
        organisation.ExternalIdentifier = grandparentOrganisation.Id;

        gpRoleType.Id = settings.GpPracticeRoleId;
        commByRelType.Id = settings.IsCommissionedByRelType;
        subLocationRoleType.Id = settings.SubLocationRoleId;
        geogOfRelType.Id = settings.InGeographyOfRelType;

        grandparentOrganisation.Related.Clear();
        grandparentOrganisation.Parents.Clear();
        parentOrganisation.Related.Clear();
        parentOrganisation.Parents.Clear();

        var random = new Random();
        grandparentOrganisation.Related.Add(
            new()
            {
                Id = random.Next(5000, 10000),
                OwnerOrganisationId = grandparentOrganisation.Id,
                TargetOrganisationId = parentOrganisation.Id,
                RelationshipTypeId = geogOfRelType.Id,
            });

        parentOrganisation.Roles = new List<OrganisationRole>
        {
            new() { OrganisationId = parentOrganisation.Id, RoleId = subLocationRoleType.Id, IsPrimaryRole = false, },
        };

        childOrganisations.ForEach(
            x =>
            {
                x.Related.Clear();
                x.Parents.Clear();

                x.Roles = new List<OrganisationRole>
                {
                    new() { OrganisationId = x.Id, RoleId = gpRoleType.Id, IsPrimaryRole = true, },
                };

                parentOrganisation.Related.Add(
                    new()
                    {
                        Id = random.Next(5000, 10000),
                        OwnerOrganisationId = parentOrganisation.Id,
                        TargetOrganisationId = x.Id,
                        RelationshipTypeId = commByRelType.Id,
                    });
            });

        context.Add(gpRoleType);
        context.Add(subLocationRoleType);
        context.Add(organisation);
        context.Add(commByRelType);
        context.Add(geogOfRelType);
        context.Add(grandparentOrganisation);
        context.Add(parentOrganisation);
        context.AddRange(childOrganisations);
        await context.SaveChangesAsync();

        var results = (await service.GetServiceRecipientsByParentInternalIdentifier(organisation.InternalIdentifier))
            .ToList();

        results.Should().NotBeEmpty();
        results.Should().HaveCount(childOrganisations.Count);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task UpdateOrganisationDetails_InvalidOdsOrganisation_Returns(
        string odsCode,
        [Frozen] Mock<IOrganisationsService> organisationsService,
        TrudOdsService service)
    {
        await service.UpdateOrganisationDetails(odsCode);

        organisationsService.Verify(x => x.UpdateCcgOrganisation(It.IsAny<OdsOrganisation>()), Times.Never());
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

        await service.UpdateOrganisationDetails(organisation.InternalIdentifier);

        organisationsService.Verify(x => x.UpdateCcgOrganisation(It.IsAny<OdsOrganisation>()), Times.Never());
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task UpdateOrganisationDetails_ValidOrganisation_UpdatesOrganisationDetails(
        Organisation organisation,
        EntityFramework.OdsOrganisations.Models.OdsOrganisation trudOrganisation,
        [Frozen] Mock<IOrganisationsService> organisationsService,
        [Frozen] BuyingCatalogueDbContext context,
        TrudOdsService service)
    {
        trudOrganisation.Id = organisation.ExternalIdentifier;

        context.Organisations.Add(organisation);
        context.OdsOrganisations.Add(trudOrganisation);
        await context.SaveChangesAsync();

        OdsOrganisation actual = null;

        organisationsService
            .Setup(x => x.UpdateCcgOrganisation(It.IsAny<OdsOrganisation>()))
            .Callback<OdsOrganisation>(x => actual = x);

        await service.UpdateOrganisationDetails(organisation.ExternalIdentifier);

        organisationsService.VerifyAll();

        actual.Should().BeEquivalentTo(TrudOdsService.MapOrganisation(trudOrganisation));
    }
}
