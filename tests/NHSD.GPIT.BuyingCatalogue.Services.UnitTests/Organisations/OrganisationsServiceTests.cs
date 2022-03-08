﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.Services.Organisations;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Organisations
{
    public static class OrganisationsServiceTests
    {
        private const string OdsCode = "OdsCode";
        private const string OrganisationName = "OrganisationName";
        private const string Junk = "Junk";

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrganisationsService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static Task AddCcgOrganisation_NullOdsOrganisation_ThrowsException(OrganisationsService service)
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() => service.AddCcgOrganisation(null, true));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddCcgOrganisation_OrganisationAlreadyExists_ReturnsError(
            [Frozen] BuyingCatalogueDbContext context,
            OdsOrganisation odsOrganisation,
            bool agreementSigned,
            Organisation organisation,
            OrganisationsService service)
        {
            organisation.ExternalIdentifier = odsOrganisation.OdsCode;
            organisation.OrganisationType = OrganisationType.CCG;
            context.Organisations.Add(organisation);
            await context.SaveChangesAsync();

            (int orgId, var error) = await service.AddCcgOrganisation(odsOrganisation, agreementSigned);

            orgId.Should().Be(0);
            error.Should().Be($"The organisation with ODS code {odsOrganisation.OdsCode} already exists.");
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddCcgOrganisation_OrganisationCreated(
            [Frozen] BuyingCatalogueDbContext context,
            OdsOrganisation odsOrganisation,
            bool agreementSigned,
            OrganisationsService service)
        {
            (int orgId, var error) = await service.AddCcgOrganisation(odsOrganisation, agreementSigned);

            orgId.Should().NotBe(0);
            error.Should().BeNull();

            var newOrganisation = await context.Organisations.SingleAsync(o => o.Id == orgId);

            newOrganisation.Address.Should().BeEquivalentTo(odsOrganisation.Address);
            newOrganisation.Id.Should().Be(orgId);
            newOrganisation.CatalogueAgreementSigned.Should().Be(agreementSigned);
            newOrganisation.LastUpdated.Date.Should().Be(DateTime.UtcNow.Date);
            newOrganisation.Name.Should().Be(odsOrganisation.OrganisationName);
            newOrganisation.ExternalIdentifier.Should().Be(odsOrganisation.OdsCode);
            newOrganisation.InternalIdentifier.Should().Be($"CG-{odsOrganisation.OdsCode}");
            newOrganisation.PrimaryRoleId.Should().Be(odsOrganisation.PrimaryRoleId);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetOrganisationsBySearchTerm_CorrectResultsReturned(
            [Frozen] BuyingCatalogueDbContext context,
            string searchTerm,
            OrganisationsService service)
        {
            var organisations = GetOrganisationsForSearchTerm(searchTerm);
            var noMatch = organisations.Single(x => x.Name == OrganisationName && x.ExternalIdentifier == OdsCode);

            foreach (var organisation in organisations)
            {
                context.Organisations.Add(organisation);
            }

            await context.SaveChangesAsync();

            var results = await service.GetOrganisationsBySearchTerm(searchTerm);

            results.Should().BeEquivalentTo(organisations.Except(new[] { noMatch }));
        }

        private static List<Organisation> GetOrganisationsForSearchTerm(string searchTerm)
        {
            return new List<Organisation>
            {
                new() { Name = OrganisationName, ExternalIdentifier = OdsCode },
                new() { Name = $"{searchTerm}", ExternalIdentifier = OdsCode },
                new() { Name = $"{searchTerm}{Junk}", ExternalIdentifier = OdsCode },
                new() { Name = $"{Junk}{searchTerm}", ExternalIdentifier = OdsCode },
                new() { Name = $"{Junk}{searchTerm}{Junk}", ExternalIdentifier = OdsCode },
                new() { Name = OrganisationName, ExternalIdentifier = $"{searchTerm}" },
                new() { Name = OrganisationName, ExternalIdentifier = $"{searchTerm}{Junk}" },
                new() { Name = OrganisationName, ExternalIdentifier = $"{Junk}{searchTerm}" },
                new() { Name = OrganisationName, ExternalIdentifier = $"{Junk}{searchTerm}{Junk}" },
            };
        }
    }
}
