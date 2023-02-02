using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using BuyingCatalogueFunction.Models.CapabilitiesUpdate.CsvModels;
using BuyingCatalogueFunction.Services.CapabilitiesUpdate;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace BuyingCatalogueFunctionTests.Services;

public static class CapabilitiesUpdateServiceTests
{
    [Fact]
    public static void Constructor_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(CapabilitiesUpdateService).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task UpdateStandards_Exists_UpdatesInDatabase(
        Standard standard,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CapabilitiesUpdateService service)
    {
        dbContext.Standards.Add(standard);
        await dbContext.SaveChangesAsync();

        var csvStandard = new CsvStandard
        {
            Id = standard.Id,
            Name = "Updated name",
            Description = standard.Description,
            Url = standard.Url,
            Type = "Interop Standard",
        };

        await service.UpdateStandardsAsync(new[] { csvStandard });

        var updated = await dbContext.Standards.AsNoTracking().FirstOrDefaultAsync(x => x.Id == standard.Id);
        updated.Should().NotBeNull();
        updated!.Name.Should().Be(csvStandard.Name);
        updated.Description.Should().Be(csvStandard.Description);
        updated.Url.Should().Be(csvStandard.Url);
        updated.StandardType.Should().Be(StandardType.Interoperability);
    }

    /// <summary>
    /// Should be removed when Gen2 support is implemented
    /// </summary>
    [Theory]
    [InMemoryDbAutoData]
    public static async Task UpdateStandards_NewStandard_DoesNotAdd(
        CsvStandard standard,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CapabilitiesUpdateService service)
    {
        await service.UpdateStandardsAsync(new[] { standard });

        dbContext.Standards.AsNoTracking().Count(x => x.Id == standard.Id).Should().Be(0);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task UpdateCapabilities_Exists_UpdatesInDatabase(
        Capability capability,
        CapabilityCategory category,
        Framework framework,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CapabilitiesUpdateService service)
    {
        capability.FrameworkCapabilities = new List<FrameworkCapability> { new(framework.Id, capability.Id) };
        capability.CategoryId = category.Id;

        dbContext.Frameworks.Add(framework);
        dbContext.CapabilityCategories.Add(category);
        dbContext.Capabilities.Add(capability);
        await dbContext.SaveChangesAsync();

        var csvCapability = new CsvCapability
        {
            Id = $"C{capability.Id}",
            Name = capability.Name,
            Description = "Updated Description",
            Version = capability.Version,
            Url = capability.SourceUrl,
            Category = category.Name,
            Framework = $"{framework.ShortName}"
        };

        await service.UpdateCapabilitiesAsync(new[] { csvCapability });

        var updatedCapability =
            await dbContext.Capabilities.AsNoTracking().FirstOrDefaultAsync(x => x.Id == capability.Id);
        updatedCapability.Should().NotBeNull();
        updatedCapability!.Name.Should().Be(csvCapability.Name);
        updatedCapability.Description.Should().Be(csvCapability.Description);
        updatedCapability.Version.Should().Be(csvCapability.Version);
        updatedCapability.SourceUrl.Should().Be(csvCapability.Url);
        updatedCapability.CategoryId.Should().Be(category.Id);
    }

    /// <summary>
    /// Should be removed when Gen2 support is implemented
    /// </summary>
    [Theory]
    [InMemoryDbAutoData]
    public static async Task UpdateCapabilities_NewCapability_DoesNotAdd(
        CsvCapability capability,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CapabilitiesUpdateService service)
    {
        const int capabilityId = 72;

        capability.Id = $"C{capabilityId}";
        await service.UpdateCapabilitiesAsync(new[] { capability });

        dbContext.Capabilities.AsNoTracking().Count(x => x.Id == capabilityId).Should().Be(0);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task UpdateEpics_Exists_UpdatesInDatabase(
        Epic epic,
        Capability capability,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CapabilitiesUpdateService service)
    {
        epic.CompliancyLevel = CompliancyLevel.Must;
        epic.CapabilityId = capability.Id;

        dbContext.Capabilities.Add(capability);
        dbContext.Epics.Add(epic);
        await dbContext.SaveChangesAsync();

        var csvEpic = new CsvEpic
        {
            Id = epic.Id,
            CapabilityId = $"C{epic.CapabilityId}",
            Level = epic.CompliancyLevel.ToString().ToUpperInvariant(),
            Name = "Updated Name",
            Status = "Active",
        };

        await service.UpdateEpicsAsync(new[] { csvEpic });

        var updatedEpic = await dbContext.Epics.AsNoTracking().FirstOrDefaultAsync(x => x.Id == epic.Id);
        updatedEpic.Should().NotBeNull();
        updatedEpic!.Name.Should().Be(csvEpic.Name);
        updatedEpic.CapabilityId.Should().Be(capability.Id);
        updatedEpic.IsActive.Should().BeTrue();
        updatedEpic.CompliancyLevel.Should().Be(CompliancyLevel.Must);
    }

    /// <summary>
    /// Should be removed when Gen2 support is implemented
    /// </summary>
    [Theory]
    [InMemoryDbAutoData]
    public static async Task UpdateEpics_NewEpic_DoesNotAdd(
        CsvEpic epic,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CapabilitiesUpdateService service)
    {
        await service.UpdateEpicsAsync(new[] { epic });

        dbContext.Epics.AsNoTracking().Count(x => x.Id == epic.Id).Should().Be(0);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task UpdateCapabilityRelationships_StaleRelationship_IsRemoved(
        Fixture fixture,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CapabilitiesUpdateService service)
    {
        const int numberOfItems = 10;

        var capabilities = fixture.CreateMany<Capability>(numberOfItems).ToArray();
        var standards = fixture.CreateMany<Standard>(numberOfItems).ToArray();

        var standardCapabilities = capabilities.Select((x, index) => new StandardCapability(standards[index].Id, x.Id)).ToArray();
        var staleRelationship = standardCapabilities.First();

        dbContext.StandardCapabilities.RemoveRange(dbContext.StandardCapabilities);
        await dbContext.SaveChangesAsync();

        dbContext.Capabilities.AddRange(capabilities);
        dbContext.Standards.AddRange(standards);
        dbContext.StandardCapabilities.AddRange(standardCapabilities);
        await dbContext.SaveChangesAsync();

        var csvRelationships = standardCapabilities.Skip(1).Select(x => new CsvRelationship
        {
            FromId = $"C{x.CapabilityId}", ToId = x.StandardId
        });

        await service.UpdateCapabilityRelationshipsAsync(csvRelationships);

        dbContext.StandardCapabilities.Contains(staleRelationship).Should().BeFalse();
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task UpdateCapabilityRelationships_NewRelationship_IsAddedToDatabase(
        Fixture fixture,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CapabilitiesUpdateService service)
    {
        const int numberOfItems = 10;

        var capabilities = fixture.CreateMany<Capability>(numberOfItems).ToArray();
        var standards = fixture.CreateMany<Standard>(numberOfItems).ToArray();

        var standardCapabilities = capabilities.Select((x, index) => new StandardCapability(standards[index].Id, x.Id)).ToArray();

        dbContext.Capabilities.AddRange(capabilities);
        dbContext.Standards.AddRange(standards);
        await dbContext.SaveChangesAsync();

        dbContext.StandardCapabilities.RemoveRange(dbContext.StandardCapabilities);
        await dbContext.SaveChangesAsync();

        dbContext.StandardCapabilities.AddRange(standardCapabilities.Take(5));
        await dbContext.SaveChangesAsync();

        var csvRelationships = standardCapabilities.Select(x => new CsvRelationship
        {
            FromId = $"C{x.CapabilityId}", ToId = x.StandardId
        });

        await service.UpdateCapabilityRelationshipsAsync(csvRelationships);

        standardCapabilities.Skip(5).All(x => dbContext.StandardCapabilities.Contains(x)).Should().BeTrue();
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task UpdateCapabilityRelationships_NonExistentCapability_DoesNotAddRelationship(
        Fixture fixture,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CapabilitiesUpdateService service)
    {
        const int numberOfItems = 10;

        var capabilities = fixture.CreateMany<Capability>(numberOfItems).ToArray();
        var standards = fixture.CreateMany<Standard>(numberOfItems).ToArray();

        var standardCapabilities = capabilities.Select((x, index) => new StandardCapability(standards[index].Id, x.Id)).ToArray();

        dbContext.Capabilities.AddRange(capabilities.Skip(1));
        dbContext.Standards.AddRange(standards);
        await dbContext.SaveChangesAsync();

        dbContext.StandardCapabilities.RemoveRange(dbContext.StandardCapabilities);
        await dbContext.SaveChangesAsync();

        dbContext.StandardCapabilities.AddRange(standardCapabilities.Skip(1));
        await dbContext.SaveChangesAsync();

        var csvRelationships = standardCapabilities.Select(x => new CsvRelationship
        {
            FromId = $"C{x.CapabilityId}", ToId = x.StandardId
        });

        await service.UpdateCapabilityRelationshipsAsync(csvRelationships);

        standardCapabilities.Take(1).All(x => !dbContext.StandardCapabilities.Contains(x)).Should().BeTrue();
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task UpdateCapabilityRelationships_NonExistentStandard_DoesNotAddRelationship(
        Fixture fixture,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CapabilitiesUpdateService service)
    {
        const int numberOfItems = 10;

        var capabilities = fixture.CreateMany<Capability>(numberOfItems).ToArray();
        var standards = fixture.CreateMany<Standard>(numberOfItems).ToArray();

        var standardCapabilities = capabilities.Select((x, index) => new StandardCapability(standards[index].Id, x.Id)).ToArray();

        dbContext.Capabilities.AddRange(capabilities);
        dbContext.Standards.AddRange(standards.Skip(1));
        await dbContext.SaveChangesAsync();

        dbContext.StandardCapabilities.RemoveRange(dbContext.StandardCapabilities);
        await dbContext.SaveChangesAsync();

        dbContext.StandardCapabilities.AddRange(standardCapabilities.Skip(1));
        await dbContext.SaveChangesAsync();

        var csvRelationships = standardCapabilities.Select(x => new CsvRelationship
        {
            FromId = $"C{x.CapabilityId}", ToId = x.StandardId
        });

        await service.UpdateCapabilityRelationshipsAsync(csvRelationships);

        standardCapabilities.Take(1).All(x => !dbContext.StandardCapabilities.Contains(x)).Should().BeTrue();
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task UpdateFrameworksForCapability_ValidFrameworks_AddsFrameworksToCapability(
        List<Framework> frameworks,
        Capability capability,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CapabilitiesUpdateService service)
    {
        var firstFrameworkId = frameworks.First().Id;
        capability.FrameworkCapabilities = new List<FrameworkCapability> { new(firstFrameworkId, capability.Id) };

        dbContext.Frameworks.AddRange(frameworks);
        dbContext.Capabilities.Add(capability);

        await dbContext.SaveChangesAsync();

        var csvCapability = new CsvCapability { Id = $"C{capability.Id}", Framework = string.Join("|", frameworks.Select(x => x.ShortName)) };

        await service.UpdateFrameworksForCapabilityAsync(csvCapability, capability);

        var dbCapability = dbContext.Capabilities.AsNoTracking().Include(x => x.FrameworkCapabilities).First(x => x.Id == capability.Id);

        dbCapability.FrameworkCapabilities.Should().HaveCount(frameworks.Count);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task UpdateFrameworksForCapability_StaleFrameworks_RemovesFromCapability(
        List<Framework> frameworks,
        Capability capability,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CapabilitiesUpdateService service)
    {
        capability.FrameworkCapabilities = frameworks.Take(3).Select(x => new FrameworkCapability(x.Id, capability.Id)).ToList();

        dbContext.Frameworks.AddRange(frameworks);
        dbContext.Capabilities.Add(capability);

        await dbContext.SaveChangesAsync();

        var csvCapability = new CsvCapability { Id = $"C{capability.Id}", Framework = string.Join("|", frameworks.Skip(2).Select(x => x.ShortName)) };

        await service.UpdateFrameworksForCapabilityAsync(csvCapability, capability);

        var dbCapability = dbContext.Capabilities.AsNoTracking().Include(x => x.FrameworkCapabilities).First(x => x.Id == capability.Id);

        dbCapability.FrameworkCapabilities.Should().HaveCount(frameworks.Skip(2).Count());
        frameworks.Take(2).ToList().ForEach(x => dbCapability.FrameworkCapabilities.Should().NotContain(y => y.FrameworkId == x.Id));
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task UpdateFrameworksForCapability_NoFrameworks_DoesNothing(
        List<Framework> frameworks,
        Capability capability,
        [Frozen] BuyingCatalogueDbContext dbContext,
        CapabilitiesUpdateService service)
    {
        const int numberOfFrameworks = 2;

        capability.FrameworkCapabilities = frameworks.Take(numberOfFrameworks).Select(x => new FrameworkCapability(x.Id, capability.Id)).ToList();

        dbContext.Frameworks.AddRange(frameworks);
        dbContext.Capabilities.Add(capability);

        await dbContext.SaveChangesAsync();

        var csvCapability = new CsvCapability { Id = $"C{capability.Id}", Framework = string.Empty };

        await service.UpdateFrameworksForCapabilityAsync(csvCapability, capability);

        var dbCapability = dbContext.Capabilities.AsNoTracking().Include(x => x.FrameworkCapabilities).First(x => x.Id == capability.Id);

        dbCapability.FrameworkCapabilities.Should().HaveCount(numberOfFrameworks);
    }
}
