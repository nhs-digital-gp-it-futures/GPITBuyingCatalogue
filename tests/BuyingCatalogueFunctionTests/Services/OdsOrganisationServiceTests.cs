using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using BuyingCatalogueFunction.Adapters;
using BuyingCatalogueFunction.Models.Ods;
using BuyingCatalogueFunction.Services.IncrementalUpdate;
using FluentAssertions;
using Moq;
using MoreLinq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace BuyingCatalogueFunctionTests.Services
{
    public static class OdsOrganisationServiceTests
    {
        [Fact]
        public static void Constructor_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OdsOrganisationService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static void AddRelationshipTypes_RelationshipsAreNull_ExpectedResult(
            OdsOrganisationService service)
        {
            FluentActions
                .Awaiting(() => service.AddRelationshipTypes(null))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddRelationshipTypes_NewRelationshipTypes_ExpectedResult(
            List<Relationship> relationships,
            [Frozen] BuyingCatalogueDbContext dbContext,
            OdsOrganisationService service)
        {
            dbContext.OrganisationRelationshipTypes.Should().BeEmpty();

            await service.AddRelationshipTypes(relationships);

            var expected = relationships.Select(x => new RelationshipType
            {
                Id = x.id,
                Description = x.displayName,
            });

            dbContext.OrganisationRelationshipTypes.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddRelationshipTypes_MixOfOldAndNewTypes_ExpectedResult(
            List<Relationship> existingRelationships,
            List<Relationship> newRelationships,
            [Frozen] BuyingCatalogueDbContext dbContext,
            OdsOrganisationService service)
        {
            var existing = existingRelationships.Select(x => new RelationshipType
            {
                Id = x.id,
                Description = x.displayName,
            });

            await dbContext.OrganisationRelationshipTypes.AddRangeAsync(existing);
            await dbContext.SaveChangesAsync();

            var allRelationships = existingRelationships.Union(newRelationships).ToList();

            await service.AddRelationshipTypes(allRelationships);

            var expected = allRelationships.Select(x => new RelationshipType
            {
                Id = x.id,
                Description = x.displayName,
            });

            dbContext.OrganisationRelationshipTypes.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void AddRoleTypes_RolesAreNull_ExpectedResult(
            OdsOrganisationService service)
        {
            FluentActions
                .Awaiting(() => service.AddRoleTypes(null))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddRoleTypes_NewRoleTypes_ExpectedResult(
            List<Role> roles,
            [Frozen] BuyingCatalogueDbContext dbContext,
            OdsOrganisationService service)
        {
            dbContext.OrganisationRoleTypes.Should().BeEmpty();

            await service.AddRoleTypes(roles);

            var expected = roles.Select(x => new RoleType
            {
                Id = x.id,
                Description = x.displayName,
            });

            dbContext.OrganisationRoleTypes.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddRoleTypes_MixOfOldAndNewTypes_ExpectedResult(
            List<Role> existingRoles,
            List<Role> newRoles,
            [Frozen] BuyingCatalogueDbContext dbContext,
            OdsOrganisationService service)
        {
            var existing = existingRoles.Select(x => new RoleType
            {
                Id = x.id,
                Description = x.displayName,
            });

            await dbContext.OrganisationRoleTypes.AddRangeAsync(existing);
            await dbContext.SaveChangesAsync();

            var allRoles = existingRoles.Union(newRoles).ToList();

            await service.AddRoleTypes(allRoles);

            var expected = allRoles.Select(x => new RoleType
            {
                Id = x.id,
                Description = x.displayName,
            });

            dbContext.OrganisationRoleTypes.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void AddOrganisationRelationships_RelationshipsAreNull_ExpectedResult(
            OdsOrganisationService service)
        {
            FluentActions
                .Awaiting(() => service.AddOrganisationRelationships(null))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddOrganisationRelationships_NewRelationships_ExpectedResult(
            Org organisation,
            List<OrganisationRelationship> expected,
            [Frozen] BuyingCatalogueDbContext dbContext,
            [Frozen] Mock<IAdapter<Org, IEnumerable<OrganisationRelationship>>> adapter,
            OdsOrganisationService service)
        {
            dbContext.OrganisationRelationships.Should().BeEmpty();

            adapter
                .Setup(x => x.Process(organisation))
                .Returns(expected);

            await service.AddOrganisationRelationships(new List<Org> { organisation });

            adapter.VerifyAll();

            dbContext.OrganisationRelationships.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddOrganisationRelationships_WithExistingRelationships_ExpectedResult(
            Org organisation,
            List<OrganisationRelationship> existing,
            List<OrganisationRelationship> expected,
            OdsOrganisation ownerOrganisation,
            OdsOrganisation targetOrganisation,
            [Frozen] BuyingCatalogueDbContext dbContext,
            [Frozen] Mock<IAdapter<Org, IEnumerable<OrganisationRelationship>>> adapter,
            OdsOrganisationService service)
        {
            dbContext.OrganisationRelationships.Should().BeEmpty();
            var firstOrg = existing.First();

            firstOrg.OwnerOrganisation = ownerOrganisation;
            firstOrg.TargetOrganisation = targetOrganisation;

            existing.Skip(1).ForEach(x =>
            {
                x.OwnerOrganisation = existing.First().OwnerOrganisation;
                x.TargetOrganisation = existing.First().TargetOrganisation;
            });

            existing.ForEach(x => x.OwnerOrganisation.Id = organisation.OrgId.extension);

            await dbContext.OrganisationRelationships.AddRangeAsync(existing);
            await dbContext.SaveChangesAsync();

            dbContext.OrganisationRelationships.Should().BeEquivalentTo(existing);

            adapter
                .Setup(x => x.Process(organisation))
                .Returns(expected);

            await service.AddOrganisationRelationships(new List<Org> { organisation });

            adapter.VerifyAll();

            dbContext.OrganisationRelationships.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void AddOrganisationRoles_RolesAreNull_ExpectedResult(
            OdsOrganisationService service)
        {
            FluentActions
                .Awaiting(() => service.AddOrganisationRoles(null))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddOrganisationRoles_NewRoleTypes_ExpectedResult(
            Org organisation,
            List<OrganisationRole> expected,
            [Frozen] BuyingCatalogueDbContext dbContext,
            [Frozen] Mock<IAdapter<Org, IEnumerable<OrganisationRole>>> adapter,
            OdsOrganisationService service)
        {
            dbContext.OrganisationRoles.Should().BeEmpty();

            adapter
                .Setup(x => x.Process(organisation))
                .Returns(expected);

            await service.AddOrganisationRoles(new List<Org> { organisation });

            adapter.VerifyAll();

            dbContext.OrganisationRoles.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddOrganisationRoles_WithExistingRoles_ExpectedResult(
            Org organisation,
            OdsOrganisation odsOrganisation,
            List<OrganisationRole> existing,
            List<OrganisationRole> expected,
            [Frozen] BuyingCatalogueDbContext dbContext,
            [Frozen] Mock<IAdapter<Org, IEnumerable<OrganisationRole>>> adapter,
            OdsOrganisationService service)
        {
            dbContext.OrganisationRoles.Should().BeEmpty();

            var firstOrg = existing.First();
            firstOrg.Organisation = odsOrganisation;

            existing.Skip(1).ForEach(x => x.Organisation = firstOrg.Organisation);
            existing.ForEach(x => x.Organisation.Id = organisation.OrgId.extension);

            await dbContext.OrganisationRoles.AddRangeAsync(existing);
            await dbContext.SaveChangesAsync();

            dbContext.OrganisationRoles.Should().BeEquivalentTo(existing);

            adapter
                .Setup(x => x.Process(organisation))
                .Returns(expected);

            await service.AddOrganisationRoles(new List<Org> { organisation });

            adapter.VerifyAll();

            dbContext.OrganisationRoles.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [CommonAutoData]
        public static void UpsertOrganisations_OrganisationsAreNull_ExpectedResult(
            OdsOrganisationService service)
        {
            FluentActions
                .Awaiting(() => service.UpsertOrganisations(null))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpsertOrganisations_NewOrganisation_ExpectedResult(
            Org organisation,
            OdsOrganisation expected,
            [Frozen] BuyingCatalogueDbContext dbContext,
            [Frozen] Mock<IAdapter<Org, OdsOrganisation>> adapter,
            OdsOrganisationService service)
        {
            dbContext.OdsOrganisations.Should().BeEmpty();

            adapter
                .Setup(x => x.Process(organisation))
                .Returns(expected);

            await service.UpsertOrganisations(new List<Org> { organisation });

            adapter.VerifyAll();

            dbContext.OdsOrganisations.Should().BeEquivalentTo(new[] { expected });
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task UpsertOrganisations_OrganisationAlreadyExists_ExpectedResult(
            Org organisation,
            OdsOrganisation existing,
            OdsOrganisation expected,
            [Frozen] BuyingCatalogueDbContext dbContext,
            [Frozen] Mock<IAdapter<Org, OdsOrganisation>> adapter,
            OdsOrganisationService service)
        {
            dbContext.OdsOrganisations.Should().BeEmpty();

            existing.Id = organisation.OrgId.extension;
            existing.Related.Clear();
            existing.Parents.Clear();

            expected.Id = organisation.OrgId.extension;
            expected.Related.Clear();
            expected.Parents.Clear();

            await dbContext.OdsOrganisations.AddAsync(existing);
            await dbContext.SaveChangesAsync();

            dbContext.OdsOrganisations.Should().BeEquivalentTo(new[] { existing });

            adapter
                .Setup(x => x.Process(organisation))
                .Returns(expected);

            await service.UpsertOrganisations(new List<Org> { organisation });

            adapter.VerifyAll();

            dbContext.OdsOrganisations.Should().BeEquivalentTo(new[] { expected }, opt => opt.Excluding(x => x.Roles));
        }
    }
}
