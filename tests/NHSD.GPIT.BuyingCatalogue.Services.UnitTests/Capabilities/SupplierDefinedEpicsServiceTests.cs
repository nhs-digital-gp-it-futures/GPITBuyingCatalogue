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
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Capabilities
{
    public static class SupplierDefinedEpicsServiceTests
    {
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
            Capability capability,
            List<Epic> epics,
            [Frozen] BuyingCatalogueDbContext context,
            SupplierDefinedEpicsService service)
        {
            epics.ForEach(e =>
            {
                e.SupplierDefined = true;
                e.CapabilityId = capability.Id;
            });

            context.Capabilities.Add(capability);
            context.Epics.AddRange(epics);
            await context.SaveChangesAsync();

            var supplierDefinedEpics = await service.GetSupplierDefinedEpics();

            supplierDefinedEpics.Count.Should().Be(epics.Count);
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
    }
}
