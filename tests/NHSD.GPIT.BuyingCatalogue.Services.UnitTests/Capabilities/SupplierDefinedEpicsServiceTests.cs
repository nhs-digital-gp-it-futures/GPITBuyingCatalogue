using System;
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
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.SupplierDefinedEpics;
using NHSD.GPIT.BuyingCatalogue.Services.Capabilities;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Capabilities
{
    public static class SupplierDefinedEpicsServiceTests
    {
        private const string CapabilityName = "CapabilityName";
        private const string EpicName = "EpicName";
        private const string Junk = "Junk";

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(SupplierDefinedEpicsService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetSupplierDefinedEpics_ReturnsExpectedResult(
            [Frozen] BuyingCatalogueDbContext context,
            SupplierDefinedEpicsService service)
        {
            var expectedCount = context.Epics.Where(e => e.SupplierDefined).Count();

            var supplierDefinedEpics = await service.GetSupplierDefinedEpics();

            supplierDefinedEpics.Count.Should().Be(expectedCount);
        }

        [Theory]
        [CommonInlineAutoData(null)]
        [CommonInlineAutoData("")]
        [CommonInlineAutoData(" ")]
        public static void GetSupplierDefinedEpicsBySearchTerm_SearchTermNull_ThrowsException(
            string searchTerm,
            SupplierDefinedEpicsService service)
        {
            FluentActions
                .Awaiting(() => service.GetSupplierDefinedEpicsBySearchTerm(searchTerm))
                .Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetSupplierDefinedEpicsBySearchTerm_ReturnsExpectedResult(
            string searchTerm,
            [Frozen] BuyingCatalogueDbContext context,
            SupplierDefinedEpicsService service)
        {
            var epics = GetEpicsForSearchTerm(searchTerm);
            var noMatch = epics.Single(x => x.Name == EpicName && x.Capability.Name == CapabilityName);

            for (var i = 0; i < epics.Count; i++)
            {
                var capability = epics[i].Capability;

                capability.Id = i;
                capability.CapabilityRef = $"{i}";
                capability.Description = $"{i}";
                capability.Version = $"{i}";

                context.Capabilities.Add(capability);

                var epic = epics[i];

                epic.CapabilityId = epic.Capability.Id;
                epic.Id = $"S{i}";
                epic.SupplierDefined = true;

                context.Epics.Add(epic);
            }

            await context.SaveChangesAsync();

            var results = await service.GetSupplierDefinedEpicsBySearchTerm(searchTerm);

            foreach (var result in results)
            {
                epics.Should().Contain(x => x.Name == result.Name
                    && x.Capability.Name == result.Capability.Name);
            }

            results.Should().NotContain(x => x.Name == noMatch.Name
                && x.Capability.Name == noMatch.Capability.Name);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task EpicExists_NullId_DuplicateContent_ReturnsTrue(
            Capability capability,
            Epic epic,
            [Frozen] BuyingCatalogueDbContext context,
            SupplierDefinedEpicsService service)
        {
            epic.SupplierDefined = true;
            epic.CapabilityId = capability.Id;

            context.Capabilities.Add(capability);
            context.Epics.Add(epic);
            await context.SaveChangesAsync();

            var epicExists = await service.EpicExists(null, capability.Id, epic.Name, epic.Description, epic.IsActive);

            epicExists.Should().BeTrue();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task EpicExists_ValidId_DuplicateContent_ReturnsFalse(
            Capability capability,
            Epic epic,
            [Frozen] BuyingCatalogueDbContext context,
            SupplierDefinedEpicsService service)
        {
            epic.SupplierDefined = true;
            epic.CapabilityId = capability.Id;

            context.Capabilities.Add(capability);
            context.Epics.Add(epic);
            await context.SaveChangesAsync();

            var epicExists = await service.EpicExists(epic.Id, capability.Id, epic.Name, epic.Description, epic.IsActive);

            epicExists.Should().BeFalse();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task EpicExists_NullId_ReturnsFalse(
            Capability capability,
            Epic epic,
            SupplierDefinedEpicsService service)
        {
            var epicExists = await service.EpicExists(null, capability.Id, epic.Name, epic.Description, epic.IsActive);

            epicExists.Should().BeFalse();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task EpicExists_ValidId_ReturnsFalse(
            Capability capability,
            Epic epic,
            SupplierDefinedEpicsService service)
        {
            var epicExists = await service.EpicExists(epic.Id, capability.Id, epic.Name, epic.Description, epic.IsActive);

            epicExists.Should().BeFalse();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static Task AddSupplierDefinedEpic_NullModel_ThrowsArgumentNullException(SupplierDefinedEpicsService service)
            => Assert.ThrowsAsync<ArgumentNullException>(() => service.AddSupplierDefinedEpic(null));

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddSupplierDefinedEpic_ValidModel_AddsEpic(
            AddEditSupplierDefinedEpic addEpicModel,
            [Frozen] BuyingCatalogueDbContext context,
            SupplierDefinedEpicsService service)
        {
            await service.AddSupplierDefinedEpic(addEpicModel);

            var epic = await context.Epics.FirstAsync();

            epic.Id.Should().Be("S00001");
            epic.Name.Should().Be(addEpicModel.Name);
            epic.Description.Should().Be(addEpicModel.Description);
            epic.IsActive.Should().Be(addEpicModel.IsActive);
            epic.SupplierDefined.Should().BeTrue();
            epic.CompliancyLevel.Should().Be(CompliancyLevel.May);
            epic.CapabilityId.Should().Be(addEpicModel.CapabilityId);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddSupplierDefinedEpic_GeneratesIncrementedIds(
            AddEditSupplierDefinedEpic addFirstEpicModel,
            AddEditSupplierDefinedEpic addSecondEpicModel,
            [Frozen] BuyingCatalogueDbContext context,
            SupplierDefinedEpicsService service)
        {
            await service.AddSupplierDefinedEpic(addFirstEpicModel);
            await service.AddSupplierDefinedEpic(addSecondEpicModel);

            var firstEpic = context.Epics.First();
            var secondEpic = context.Epics.Skip(1).First();

            firstEpic.Id.Should().Be("S00001");
            secondEpic.Id.Should().Be("S00002");
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddSupplierDefinedEpic_ExistingEpicInDb_IncrementsId(
            Capability capability,
            Epic epic,
            AddEditSupplierDefinedEpic addEpicModel,
            [Frozen] BuyingCatalogueDbContext context,
            SupplierDefinedEpicsService service)
        {
            epic.Id = "S00012";
            epic.CapabilityId = capability.Id;
            epic.SupplierDefined = true;

            context.Capabilities.Add(capability);
            context.Epics.Add(epic);
            await context.SaveChangesAsync();

            await service.AddSupplierDefinedEpic(addEpicModel);

            var addedEpic = context.Epics.FirstOrDefault(e => string.Equals(e.Id, "S00013"));

            addedEpic.Should().NotBeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddSupplierDefinedEpic_LegacyEpicInDb_IncrementsId(
            Capability capability,
            Epic epic,
            AddEditSupplierDefinedEpic addEpicModel,
            [Frozen] BuyingCatalogueDbContext context,
            SupplierDefinedEpicsService service)
        {
            epic.Id = "S020X01E01";
            epic.CapabilityId = capability.Id;
            epic.SupplierDefined = true;

            context.Capabilities.Add(capability);
            context.Epics.Add(epic);
            await context.SaveChangesAsync();

            await service.AddSupplierDefinedEpic(addEpicModel);

            var addedEpic = context.Epics.FirstOrDefault(e => string.Equals(e.Id, "S00001"));

            addedEpic.Should().NotBeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddSupplierDefinedEpic_LegacyAndNewEpicInDb_IncrementsId(
            Capability capability,
            Epic legacyEpic,
            Epic newEpic,
            AddEditSupplierDefinedEpic addEpicModel,
            [Frozen] BuyingCatalogueDbContext context,
            SupplierDefinedEpicsService service)
        {
            legacyEpic.Id = "S020X01E01";
            legacyEpic.CapabilityId = capability.Id;
            legacyEpic.SupplierDefined = true;

            newEpic.Id = "S00010";
            newEpic.CapabilityId = capability.Id;
            newEpic.SupplierDefined = true;

            context.Capabilities.Add(capability);
            context.Epics.Add(legacyEpic);
            context.Epics.Add(newEpic);
            await context.SaveChangesAsync();

            await service.AddSupplierDefinedEpic(addEpicModel);

            var addedEpic = context.Epics.FirstOrDefault(e => string.Equals(e.Id, "S00011"));

            addedEpic.Should().NotBeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetEpic_EpicIdNullOrEmpty_ThrowsArgumentException(
            SupplierDefinedEpicsService service)
        {
            await Assert.ThrowsAsync<ArgumentException>(() => service.GetEpic(null));
            await Assert.ThrowsAsync<ArgumentException>(() => service.GetEpic(string.Empty));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetEpic_ValidEpicId_ReturnsEpic(
            Capability capability,
            Epic epic,
            [Frozen] BuyingCatalogueDbContext context,
            SupplierDefinedEpicsService service)
        {
            epic.CapabilityId = capability.Id;
            epic.SupplierDefined = true;

            context.Capabilities.Add(capability);
            context.Epics.Add(epic);
            await context.SaveChangesAsync();

            var actualEpic = await service.GetEpic(epic.Id);

            actualEpic
                .Should()
                .BeEquivalentTo(
                    epic,
                    opt => opt
                        .Excluding(m => m.Capability));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetEpic_ValidEpicId_NotSupplierDefined_ReturnsNull(
            Capability capability,
            Epic epic,
            [Frozen] BuyingCatalogueDbContext context,
            SupplierDefinedEpicsService service)
        {
            epic.CapabilityId = capability.Id;
            epic.SupplierDefined = false;

            context.Capabilities.Add(capability);
            context.Epics.Add(epic);
            await context.SaveChangesAsync();

            var actualEpic = await service.GetEpic(epic.Id);

            actualEpic.Should().BeNull();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetItemsReferencingEpic_EpicIdNullOrEmpty_ThrowsArgumentException(
            SupplierDefinedEpicsService service)
        {
            await Assert.ThrowsAsync<ArgumentException>(() => service.GetItemsReferencingEpic(null));
            await Assert.ThrowsAsync<ArgumentException>(() => service.GetItemsReferencingEpic(string.Empty));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetItemsReferencingEpic_ValidEpicId_ReturnsItems(
            Epic epic,
            Capability capability,
            List<CatalogueItem> catalogueItems,
            [Frozen] BuyingCatalogueDbContext context,
            SupplierDefinedEpicsService service)
        {
            epic.CapabilityId = capability.Id;
            epic.SupplierDefined = true;

            catalogueItems.ForEach(c => c.CatalogueItemEpics.Add(new CatalogueItemEpic
            {
                CapabilityId = capability.Id,
                EpicId = epic.Id,
                StatusId = 1,
            }));

            context.Capabilities.Add(capability);
            context.Epics.Add(epic);
            context.CatalogueItems.AddRange(catalogueItems);

            await context.SaveChangesAsync();

            var relatedItems = await service.GetItemsReferencingEpic(epic.Id);

            relatedItems.Should().NotBeNull();
            relatedItems.Should().NotBeEmpty();
            relatedItems.Count.Should().Be(catalogueItems.Count);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetItemsReferencingEpic_ValidEpicId_NotSupplierDefined_ReturnsEmpty(
            Epic epic,
            Capability capability,
            List<CatalogueItem> catalogueItems,
            [Frozen] BuyingCatalogueDbContext context,
            SupplierDefinedEpicsService service)
        {
            epic.CapabilityId = capability.Id;
            epic.SupplierDefined = false;

            catalogueItems.ForEach(c => c.CatalogueItemEpics.Add(new CatalogueItemEpic
            {
                CapabilityId = capability.Id,
                EpicId = epic.Id,
                StatusId = 1,
            }));

            context.Capabilities.Add(capability);
            context.Epics.Add(epic);
            context.CatalogueItems.AddRange(catalogueItems);

            await context.SaveChangesAsync();

            var relatedItems = await service.GetItemsReferencingEpic(epic.Id);

            relatedItems.Should().NotBeNull();
            relatedItems.Should().BeEmpty();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static Task EditSupplierDefinedEpic_NullModel_ThrowsArgumentNullException(SupplierDefinedEpicsService service)
            => Assert.ThrowsAsync<ArgumentNullException>(() => service.EditSupplierDefinedEpic(null));

        [Theory]
        [InMemoryDbAutoData]
        public static Task EditSupplierDefinedEpic_InvalidEpicId_ThrowsKeyNotFoundException(
            AddEditSupplierDefinedEpic editEpicModel,
            SupplierDefinedEpicsService service)
        {
            editEpicModel.Id = "invalid-id";

            return Assert.ThrowsAsync<KeyNotFoundException>(() => service.EditSupplierDefinedEpic(editEpicModel));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task EditSupplierDefinedEpic_ValidRequest_UpdatesEpic(
            Capability capability,
            Epic epic,
            AddEditSupplierDefinedEpic editEpicModel,
            [Frozen] BuyingCatalogueDbContext context,
            SupplierDefinedEpicsService service)
        {
            var expectedEpic = new Epic
            {
                Id = epic.Id,
                Name = editEpicModel.Name,
                Description = editEpicModel.Description,
                CapabilityId = editEpicModel.CapabilityId,
                IsActive = editEpicModel.IsActive,
            };

            editEpicModel.Id = epic.Id;
            epic.CapabilityId = capability.Id;
            epic.SupplierDefined = true;

            context.Capabilities.Add(capability);
            context.Epics.Add(epic);

            await context.SaveChangesAsync();
            await service.EditSupplierDefinedEpic(editEpicModel);

            var updatedEpic = await context.Epics.FindAsync(editEpicModel.Id);

            updatedEpic
                .Should()
                .BeEquivalentTo(
                    expectedEpic,
                    opt => opt.Excluding(m => m.SourceUrl)
                              .Excluding(m => m.Capability)
                              .Excluding(m => m.LastUpdated)
                              .Excluding(m => m.LastUpdatedBy)
                              .Excluding(m => m.SupplierDefined)
                              .Excluding(m => m.CompliancyLevel)
                              .Excluding(m => m.LastUpdatedByUser));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task DeleteSupplierDefinedEpic_ValidId_DeletesEpic(
            Capability capability,
            Epic epic,
            [Frozen] BuyingCatalogueDbContext context,
            SupplierDefinedEpicsService service)
        {
            epic.CapabilityId = capability.Id;
            epic.SupplierDefined = true;

            context.Capabilities.Add(capability);
            context.Epics.Add(epic);

            await service.DeleteSupplierDefinedEpic(epic.Id);

            context.Epics.Any(e => e.Id == epic.Id).Should().BeFalse();
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task DeleteSupplierDefinedEpic_InvalidId_Retuirns(
            string invalidEpicId,
            Capability capability,
            Epic epic,
            [Frozen] BuyingCatalogueDbContext context,
            SupplierDefinedEpicsService service)
        {
            epic.CapabilityId = capability.Id;
            epic.SupplierDefined = true;

            context.Capabilities.Add(capability);
            context.Epics.Add(epic);

            var expectedCount = context.Epics.AsNoTracking().Count();

            await service.DeleteSupplierDefinedEpic(invalidEpicId);

            context.Epics.AsNoTracking().Count().Should().Be(expectedCount);
        }

        private static List<Epic> GetEpicsForSearchTerm(string searchTerm)
        {
            return new List<Epic>
            {
                new() { Name = EpicName, Capability = new Capability { Name = CapabilityName } },
                new() { Name = $"{searchTerm}", Capability = new Capability { Name = CapabilityName } },
                new() { Name = $"{searchTerm}{Junk}", Capability = new Capability { Name = CapabilityName } },
                new() { Name = $"{Junk}{searchTerm}", Capability = new Capability { Name = CapabilityName } },
                new() { Name = $"{Junk}{searchTerm}{Junk}", Capability = new Capability { Name = CapabilityName } },
                new() { Name = EpicName, Capability = new Capability { Name = $"{searchTerm}" } },
                new() { Name = EpicName, Capability = new Capability { Name = $"{searchTerm}{Junk}" } },
                new() { Name = EpicName, Capability = new Capability { Name = $"{Junk}{searchTerm}" } },
                new() { Name = EpicName, Capability = new Capability { Name = $"{Junk}{searchTerm}{Junk}" } },
            };
        }
    }
}
