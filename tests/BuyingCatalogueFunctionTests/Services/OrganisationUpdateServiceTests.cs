using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using BuyingCatalogueFunction.IncrementalUpdate.Interfaces;
using BuyingCatalogueFunction.IncrementalUpdate.Models;
using BuyingCatalogueFunction.IncrementalUpdate.Models.Ods;
using BuyingCatalogueFunction.IncrementalUpdate.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.OdsOrganisations.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace BuyingCatalogueFunctionTests.Services
{
    public static class OrganisationUpdateServiceTests
    {
        [Fact]
        public static void Constructor_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(OrganisationUpdateService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetLastRunDate_NoJournalEntryExists_ExpectedResult(
            OrganisationUpdateService service)
        {
            var result = await service.GetLastRunDate();

            result.Should().Be(DateTime.Today.AddDays(-1));
        }

        [Theory]
        [InMemoryDbInlineAutoData(0)]
        [InMemoryDbInlineAutoData(-1)]
        [InMemoryDbInlineAutoData(-10)]
        [InMemoryDbInlineAutoData(-180)]
        public static async Task GetLastRunDate_ValidJournalEntryExists_ExpectedResult(
            int offset,
            [Frozen] BuyingCatalogueDbContext dbContext,
            OrganisationUpdateService service)
        {
            var lastRunDate = DateTime.Today.AddDays(offset);

            dbContext.OrgImportJournal.Add(new OrgImportJournal
            {
                ImportDate = lastRunDate,
            });

            await dbContext.SaveChangesAsync();

            var result = await service.GetLastRunDate();

            result.Should().Be(lastRunDate);
        }

        [Theory]
        [InMemoryDbInlineAutoData(-185)]
        [InMemoryDbInlineAutoData(-190)]
        [InMemoryDbInlineAutoData(-200)]
        public static async Task GetLastRunDate_InvalidJournalEntryExists_ExpectedResult(
            int offset,
            [Frozen] BuyingCatalogueDbContext dbContext,
            OrganisationUpdateService service)
        {
            var lastRunDate = DateTime.Today.AddDays(offset);

            dbContext.OrgImportJournal.Add(new OrgImportJournal
            {
                ImportDate = lastRunDate,
            });

            await dbContext.SaveChangesAsync();

            var result = await service.GetLastRunDate();

            result.Should().Be(DateTime.Today.AddDays(OrganisationUpdateService.MaxDays));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetLastRunDate_NoJournalEntryExists_ExpectedResult(
            DateTime lastRunDate,
            [Frozen] BuyingCatalogueDbContext dbContext,
            OrganisationUpdateService service)
        {
            dbContext.OrgImportJournal.Should().BeEmpty();

            await service.SetLastRunDate(lastRunDate);

            dbContext.OrgImportJournal.Count().Should().Be(1);

            var result = await dbContext.OrgImportJournal.FirstOrDefaultAsync();

            result.Should().NotBeNull();
            result!.ImportDate.Should().Be(lastRunDate);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SetLastRunDate_JournalEntryExists_ExpectedResult(
            DateTime originalDate,
            DateTime lastRunDate,
            [Frozen] BuyingCatalogueDbContext dbContext,
            OrganisationUpdateService service)
        {
            dbContext.OrgImportJournal.Should().BeEmpty();

            await dbContext.OrgImportJournal.AddAsync(new OrgImportJournal
            {
                ImportDate = originalDate,
            });

            await dbContext.SaveChangesAsync();

            dbContext.OrgImportJournal.Count().Should().Be(1);

            await service.SetLastRunDate(lastRunDate);

            dbContext.OrgImportJournal.Count().Should().Be(1);

            var result = await dbContext.OrgImportJournal.FirstAsync();

            result.ImportDate.Should().Be(lastRunDate);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static void IncrementalUpdate_DataIsNull_ExpectedResult(
            OrganisationUpdateService service)
        {
            FluentActions
                .Awaiting(() => service.IncrementalUpdate(null))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static void IncrementalUpdate_ThrowsError_ExpectedResult(
            IncrementalUpdateData data,
            [Frozen] BuyingCatalogueDbContext dbContext,
            [Frozen] Mock<IOdsOrganisationService> odsOrganisationService,
            OrganisationUpdateService service)
        {
            odsOrganisationService
                .Setup(x => x.AddRelationshipTypes(data.Relationships))
                .Verifiable();

            odsOrganisationService
                .Setup(x => x.AddRoleTypes(data.Roles))
                .Throws<ArgumentException>();

            dbContext.Database.CurrentTransaction.Should().BeNull();

            FluentActions
                .Awaiting(() => service.IncrementalUpdate(data))
                .Should().ThrowAsync<ArgumentException>();

            odsOrganisationService.VerifyAll();
            odsOrganisationService.VerifyNoOtherCalls();

            dbContext.Database.CurrentTransaction.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task IncrementalUpdate_EverythingOk_ExpectedResult(
            IncrementalUpdateData data,
            [Frozen] BuyingCatalogueDbContext dbContext,
            [Frozen] Mock<IOdsOrganisationService> odsOrganisationService,
            OrganisationUpdateService service)
        {
            odsOrganisationService
                .Setup(x => x.AddRelationshipTypes(data.Relationships))
                .Verifiable();

            odsOrganisationService
                .Setup(x => x.AddRoleTypes(data.Roles))
                .Verifiable();

            var actual = new List<Org>();

            odsOrganisationService
                .Setup(x => x.UpsertOrganisations(It.IsAny<List<Org>>()))
                .Callback<List<Org>>(x => actual = x);

            odsOrganisationService
                .Setup(x => x.AddOrganisationRelationships(data.Organisations))
                .Verifiable();

            odsOrganisationService
                .Setup(x => x.AddOrganisationRoles(data.Organisations))
                .Verifiable();

            dbContext.Database.CurrentTransaction.Should().BeNull();

            await service.IncrementalUpdate(data);

            odsOrganisationService.VerifyAll();
            odsOrganisationService.VerifyNoOtherCalls();

            dbContext.Database.CurrentTransaction.Should().BeNull();

            actual.Should().BeEquivalentTo(data.Organisations.Union(data.RelatedOrganisations));
        }
    }
}
