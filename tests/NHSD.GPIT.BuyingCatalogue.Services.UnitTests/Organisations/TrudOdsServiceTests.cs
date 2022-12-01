﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
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
        List<EntityFramework.OdsOrganisations.Models.OdsOrganisation> organisations,
        Organisation organisation,
        RoleType roleType,
        RelationshipType relationshipType,
        [Frozen] OdsSettings settings,
        [Frozen] BuyingCatalogueDbContext context,
        TrudOdsService service)
    {
        var parentOrganisation = organisations.First();
        var childOrganisations = organisations.Skip(1).ToList();

        organisation.ExternalIdentifier = parentOrganisation.Id;

        roleType.Id = settings.GpPracticeRoleId;
        relationshipType.Id = TrudOdsService.RelationshipType;
        parentOrganisation.Related.Clear();
        parentOrganisation.Parents.Clear();

        var random = new Random();
        childOrganisations.ForEach(
            x =>
            {
                x.Related.Clear();
                x.Parents.Clear();

                x.Roles = new List<OrganisationRole>
                {
                    new() { OrganisationId = x.Id, RoleId = roleType.Id, IsPrimaryRole = true, },
                };

                parentOrganisation.Related.Add(
                    new()
                    {
                        Id = random.Next(5000, 10000),
                        OwnerOrganisationId = parentOrganisation.Id,
                        TargetOrganisationId = x.Id,
                        RelationshipTypeId = relationshipType.Id,
                    });
            });

        context.Add(roleType);
        context.Add(organisation);
        context.Add(relationshipType);
        context.Add(parentOrganisation);
        context.AddRange(childOrganisations);
        await context.SaveChangesAsync();

        var results = (await service.GetServiceRecipientsByParentInternalIdentifier(organisation.InternalIdentifier)).ToList();

        results.Should().NotBeEmpty();
        results.Should().HaveCount(childOrganisations.Count);
    }
}
