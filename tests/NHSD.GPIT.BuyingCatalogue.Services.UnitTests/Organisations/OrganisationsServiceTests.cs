using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using Castle.Core.Logging;
using EnumsNET;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.Services.Organisations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Organisations
{
    public static class OrganisationsServiceTests
    {
        private const int OrganisationId = 2;
        private const int NominatedOrganisationId = 3;

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
        public static Task OrganisationExists_NullOdsOrganisation_ThrowsException(OrganisationsService service)
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() => service.OrganisationExists(null));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task OrganisationExists_OrganisationOdsAlreadyExists_ReturnsTrue(
            [Frozen] BuyingCatalogueDbContext context,
            OdsOrganisation odsOrganisation,
            Organisation organisation,
            OrganisationsService service)
        {
            organisation.ExternalIdentifier = odsOrganisation.OdsCode;
            context.Organisations.Add(organisation);
            await context.SaveChangesAsync();

            var result = await service.OrganisationExists(odsOrganisation);

            result.Should().BeTrue();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task OrganisationExists_OrganisationNameAlreadyExists_ReturnsTrue(
            [Frozen] BuyingCatalogueDbContext context,
            OdsOrganisation odsOrganisation,
            Organisation organisation,
            OrganisationsService service)
        {
            organisation.Name = odsOrganisation.OrganisationName;
            context.Organisations.Add(organisation);
            await context.SaveChangesAsync();

            var result = await service.OrganisationExists(odsOrganisation);

            result.Should().BeTrue();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task OrganisationExists_OrganisationDoesNotExist_ReturnsFalse(
            OdsOrganisation odsOrganisation,
            OrganisationsService service)
        {
            var result = await service.OrganisationExists(odsOrganisation);

            result.Should().BeFalse();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddOrganisation_OrganisationAlreadyExists_ReturnsError(
            [Frozen] BuyingCatalogueDbContext context,
            OdsOrganisation odsOrganisation,
            bool agreementSigned,
            Organisation organisation,
            OrganisationsService service)
        {
            organisation.ExternalIdentifier = odsOrganisation.OdsCode;
            context.Organisations.Add(organisation);
            await context.SaveChangesAsync();

            (int orgId, var error) = await service.AddOrganisation(odsOrganisation, agreementSigned);

            orgId.Should().Be(0);
            error.Should().Be($"The organisation with ODS code {odsOrganisation.OdsCode} already exists.");
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddOrganisation_OrganisationCreated(
            [Frozen] BuyingCatalogueDbContext context,
            ILogger<OrganisationsService> logger,
            OdsOrganisation odsOrganisation,
            bool agreementSigned,
            OdsSettings settings)
        {
            odsOrganisation.PrimaryRoleId = settings.BuyerOrganisationRoles[0].PrimaryRoleId;
            var expectedOrgType = settings.GetOrganisationType(odsOrganisation.PrimaryRoleId);

            var service = new OrganisationsService(context, logger, settings);
            (int orgId, var error) = await service.AddOrganisation(odsOrganisation, agreementSigned);

            orgId.Should().NotBe(0);
            error.Should().BeNull();

            var newOrganisation = await context.Organisations.FirstAsync(o => o.Id == orgId);

            newOrganisation.Address.Should().BeEquivalentTo(odsOrganisation.Address);
            newOrganisation.Id.Should().Be(orgId);
            newOrganisation.CatalogueAgreementSigned.Should().Be(agreementSigned);
            newOrganisation.LastUpdated.Date.Should().Be(DateTime.UtcNow.Date);
            newOrganisation.Name.Should().Be(odsOrganisation.OrganisationName);
            newOrganisation.ExternalIdentifier.Should().Be(odsOrganisation.OdsCode);
            newOrganisation.InternalIdentifier.Should().Be($"{expectedOrgType.AsString(EnumFormat.EnumMemberValue)}-{odsOrganisation.OdsCode}");
            newOrganisation.PrimaryRoleId.Should().Be(odsOrganisation.PrimaryRoleId);
            newOrganisation.OrganisationType.Should().Be(expectedOrgType);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static void UpdateOrganisation_OrganisationIsNull_ThrowsException(
            OrganisationsService service)
        {
            FluentActions
                .Awaiting(() => service.UpdateOrganisation(null))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpdateOrganisation_OrganisationNotInDatabase_NoActionTaken(
            OdsOrganisation organisation,
            [Frozen] BuyingCatalogueDbContext dbContext,
            OrganisationsService service)
        {
            var existing = await dbContext.Organisations
                .FirstOrDefaultAsync(x => x.ExternalIdentifier == organisation.OdsCode);

            existing.Should().BeNull();

            await service.UpdateOrganisation(organisation);

            var actual = await dbContext.Organisations
                .FirstOrDefaultAsync(x => x.ExternalIdentifier == organisation.OdsCode);

            actual.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpdateOrganisation_OrganisationInDatabase_ValuesUpdated(
            Organisation organisation,
            OdsOrganisation newOrganisation,
            [Frozen] BuyingCatalogueDbContext dbContext,
            OrganisationsService service)
        {
            dbContext.Organisations.Add(organisation);

            await dbContext.SaveChangesAsync();

            var existing = await dbContext.Organisations
                .FirstOrDefaultAsync(x => x.ExternalIdentifier == organisation.ExternalIdentifier);

            existing.Should().NotBeNull();
            existing!.Name.Should().Be(organisation.Name);
            existing.Address.Should().Be(organisation.Address);
            existing.PrimaryRoleId.Should().Be(organisation.PrimaryRoleId);

            newOrganisation.OdsCode = organisation.ExternalIdentifier;

            await service.UpdateOrganisation(newOrganisation);

            var actual = await dbContext.Organisations
                .FirstOrDefaultAsync(x => x.ExternalIdentifier == organisation.ExternalIdentifier);

            actual.Should().NotBeNull();
            actual!.Name.Should().Be(newOrganisation.OrganisationName);
            actual.Address.Should().Be(newOrganisation.Address);
            actual.PrimaryRoleId.Should().Be(newOrganisation.PrimaryRoleId);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetOrganisationsBySearchTerm_CorrectResultsReturned(
            [Frozen] BuyingCatalogueDbContext context,
            string searchTerm,
            OrganisationsService service)
        {
            var organisations = GetOrganisationsForSearchTerm(searchTerm);
            var noMatch = organisations.First(x => x.Name == OrganisationName && x.ExternalIdentifier == OdsCode);

            foreach (var organisation in organisations)
            {
                context.Organisations.Add(organisation);
            }

            await context.SaveChangesAsync();

            var results = await service.GetOrganisationsBySearchTerm(searchTerm);

            results.Should().BeEquivalentTo(organisations.Except(new[] { noMatch }));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetNominatedOrganisations_NoRelationshipsExist_EmptySetReturned(
            [Frozen] BuyingCatalogueDbContext context,
            OrganisationsService service)
        {
            var existing = await context.RelatedOrganisations
                .Where(x => x.RelatedOrganisationId == OrganisationId)
                .ToListAsync();

            if (existing.Any())
            {
                existing.ForEach(x => context.RelatedOrganisations.Remove(x));
                await context.SaveChangesAsync();
            }

            var actual = await service.GetNominatedOrganisations(OrganisationId);

            actual.Should().BeEmpty();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetNominatedOrganisations_RelationshipsExist_CorrectResultsReturned(
            Organisation organisation,
            [Frozen] BuyingCatalogueDbContext context,
            OrganisationsService service)
        {
            var existing = await context.RelatedOrganisations
                .Where(x => x.RelatedOrganisationId == OrganisationId)
                .ToListAsync();

            if (existing.Any())
            {
                existing.ForEach(x => context.RelatedOrganisations.Remove(x));
            }

            context.Organisations.Add(organisation);
            context.RelatedOrganisations.Add(new RelatedOrganisation(organisation.Id, OrganisationId));

            await context.SaveChangesAsync();

            var actual = await service.GetNominatedOrganisations(OrganisationId);

            actual.Should().ContainSingle().Which.Id.Should().Be(organisation.Id);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddNominatedOrganisation_RelationshipAlreadyExists_NoActionTaken(
            [Frozen] BuyingCatalogueDbContext context,
            OrganisationsService service)
        {
            var existing = await context.RelatedOrganisations
                .FirstOrDefaultAsync(x => x.OrganisationId == NominatedOrganisationId
                    && x.RelatedOrganisationId == OrganisationId);

            if (existing == null)
            {
                context.RelatedOrganisations.Add(new RelatedOrganisation(NominatedOrganisationId, OrganisationId));
                await context.SaveChangesAsync();
            }

            await service.AddNominatedOrganisation(OrganisationId, NominatedOrganisationId);

            var actual = await context.RelatedOrganisations
                .FirstOrDefaultAsync(x => x.OrganisationId == NominatedOrganisationId
                    && x.RelatedOrganisationId == OrganisationId);

            actual.Should().NotBeNull();
            actual!.OrganisationId.Should().Be(NominatedOrganisationId);
            actual.RelatedOrganisationId.Should().Be(OrganisationId);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddNominatedOrganisation_RelationshipDoesNotExist_CreatesNewRelationship(
            [Frozen] BuyingCatalogueDbContext context,
            OrganisationsService service)
        {
            var existing = await context.RelatedOrganisations
                .FirstOrDefaultAsync(x => x.OrganisationId == NominatedOrganisationId
                    && x.RelatedOrganisationId == OrganisationId);

            if (existing != null)
            {
                context.RelatedOrganisations.Remove(existing);
                await context.SaveChangesAsync();
            }

            await service.AddNominatedOrganisation(OrganisationId, NominatedOrganisationId);

            var actual = await context.RelatedOrganisations
                .FirstOrDefaultAsync(x => x.OrganisationId == NominatedOrganisationId
                    && x.RelatedOrganisationId == OrganisationId);

            actual.Should().NotBeNull();
            actual!.OrganisationId.Should().Be(NominatedOrganisationId);
            actual.RelatedOrganisationId.Should().Be(OrganisationId);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task RemoveNominatedOrganisation_RelationshipAlreadyExists_RemovesRelationship(
            [Frozen] BuyingCatalogueDbContext context,
            OrganisationsService service)
        {
            var existing = await context.RelatedOrganisations
                .FirstOrDefaultAsync(x => x.OrganisationId == NominatedOrganisationId
                    && x.RelatedOrganisationId == OrganisationId);

            if (existing == null)
            {
                context.RelatedOrganisations.Add(new RelatedOrganisation(NominatedOrganisationId, OrganisationId));
                await context.SaveChangesAsync();
            }

            await service.RemoveNominatedOrganisation(OrganisationId, NominatedOrganisationId);

            var actual = await context.RelatedOrganisations
                .FirstOrDefaultAsync(x => x.OrganisationId == NominatedOrganisationId
                    && x.RelatedOrganisationId == OrganisationId);

            actual.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task RemoveNominatedOrganisation_RelationshipDoesNotExist_NoActionTaken(
            [Frozen] BuyingCatalogueDbContext context,
            OrganisationsService service)
        {
            var existing = await context.RelatedOrganisations
                .FirstOrDefaultAsync(x => x.OrganisationId == NominatedOrganisationId
                    && x.RelatedOrganisationId == OrganisationId);

            if (existing != null)
            {
                context.RelatedOrganisations.Remove(existing);
                await context.SaveChangesAsync();
            }

            await service.RemoveNominatedOrganisation(OrganisationId, NominatedOrganisationId);

            var actual = await context.RelatedOrganisations
                .FirstOrDefaultAsync(x => x.OrganisationId == NominatedOrganisationId
                    && x.RelatedOrganisationId == OrganisationId);

            actual.Should().BeNull();
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
