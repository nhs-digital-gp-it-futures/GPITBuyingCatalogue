using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;
using NHSD.GPIT.BuyingCatalogue.Services.Capabilities;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Capabilities;

public static class Gen2MappingServiceTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(Gen2MappingService).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockAutoData]
    public static Task MapToSolutions_NullModel_ThrowsArgumentNullException(
        Gen2MappingService service) => FluentActions.Invoking(() => service.MapToSolutions(null))
        .Should()
        .ThrowAsync<ArgumentNullException>();

    [Theory]
    [MockAutoData]
    public static Task MapToSolutions_EmptySolutions_ThrowsArgumentNullException(
        Gen2MappingService service) => FluentActions
        .Invoking(
            () => service.MapToSolutions(
                new Gen2MappingModel(
                    Enumerable.Empty<Gen2CapabilitiesCsvModel>(),
                    Enumerable.Empty<Gen2EpicsCsvModel>().ToList())))
        .Should()
        .ThrowAsync<ArgumentException>();

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task MapToSolutions_WithUnknownCapability_MapsExpected(
        int unknownCapabilityId,
        List<Solution> solutions,
        List<Capability> capabilities,
        List<Epic> epics,
        [Frozen] BuyingCatalogueDbContext dbContext,
        Gen2MappingService service)
    {
        capabilities.ForEach(x => x.Epics = Enumerable.Empty<Epic>().ToList());
        epics.ForEach(
            x =>
            {
                x.Capabilities = Enumerable.Empty<Capability>().ToList();
                x.CapabilityEpics = Enumerable.Empty<CapabilityEpic>().ToList();
            });

        var capabilityEpicRelationships = capabilities.Zip(epics)
            .Select(x => new CapabilityEpic { CapabilityId = x.First.Id, EpicId = x.Second.Id })
            .ToList();

        dbContext.AddRange(solutions);
        dbContext.AddRange(capabilities);
        dbContext.AddRange(epics);
        dbContext.AddRange(capabilityEpicRelationships);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var solutionsAndCapabilities = solutions.Zip(capabilityEpicRelationships).ToList();

        var capabilitiesImport = solutionsAndCapabilities.Select(
                x => new Gen2CapabilitiesCsvModel
                {
                    SolutionId = x.First.CatalogueItemId.ToString(), CapabilityId = $"C{x.Second.CapabilityId}",
                })
            .ToList();

        var epicsImport = solutionsAndCapabilities.Select(
                x => new Gen2EpicsCsvModel
                {
                    SolutionId = x.First.CatalogueItemId.ToString(),
                    CapabilityId = $"C{x.Second.CapabilityId}",
                    EpicId = x.Second.EpicId,
                })
            .ToList();

        var solutionWithUnknownCapability = solutions.First();

        capabilitiesImport.Add(
            new()
            {
                SolutionId = solutionWithUnknownCapability.CatalogueItemId.ToString(),
                CapabilityId = $"C{unknownCapabilityId}",
            });

        await service.MapToSolutions(new Gen2MappingModel(capabilitiesImport, epicsImport));

        var actualSolution = await dbContext.Solutions.Include(x => x.CatalogueItem.CatalogueItemCapabilities)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CatalogueItemId == solutionWithUnknownCapability.CatalogueItemId);

        actualSolution.CatalogueItem.CatalogueItemCapabilities.Should().HaveCount(1);
        actualSolution.CatalogueItem.CatalogueItemCapabilities.Should()
            .NotContain(x => x.CapabilityId == unknownCapabilityId);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task MapToSolutions_WithUnknownEpic_MapsExpected(
        string unknownEpicId,
        List<Solution> solutions,
        List<Capability> capabilities,
        List<Epic> epics,
        [Frozen] BuyingCatalogueDbContext dbContext,
        Gen2MappingService service)
    {
        capabilities.ForEach(x => x.Epics = Enumerable.Empty<Epic>().ToList());
        epics.ForEach(
            x =>
            {
                x.Capabilities = Enumerable.Empty<Capability>().ToList();
                x.CapabilityEpics = Enumerable.Empty<CapabilityEpic>().ToList();
            });

        var capabilityEpicRelationships = capabilities.Zip(epics)
            .Select(x => new CapabilityEpic { CapabilityId = x.First.Id, EpicId = x.Second.Id })
            .ToList();

        dbContext.AddRange(solutions);
        dbContext.AddRange(capabilities);
        dbContext.AddRange(epics);
        dbContext.AddRange(capabilityEpicRelationships);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var solutionsAndCapabilities = solutions.Zip(capabilityEpicRelationships).ToList();

        var capabilitiesImport = solutionsAndCapabilities.Select(
                x => new Gen2CapabilitiesCsvModel
                {
                    SolutionId = x.First.CatalogueItemId.ToString(), CapabilityId = $"C{x.Second.CapabilityId}",
                })
            .ToList();

        var epicsImport = solutionsAndCapabilities.Select(
                x => new Gen2EpicsCsvModel
                {
                    SolutionId = x.First.CatalogueItemId.ToString(),
                    CapabilityId = $"C{x.Second.CapabilityId}",
                    EpicId = x.Second.EpicId,
                })
            .ToList();

        var solutionWithUnknownCapability = solutions.First();

        epicsImport.Add(
            new()
            {
                SolutionId = solutionWithUnknownCapability.CatalogueItemId.ToString(),
                CapabilityId = epicsImport.First().CapabilityId,
                EpicId = unknownEpicId,
            });

        await service.MapToSolutions(new Gen2MappingModel(capabilitiesImport, epicsImport));

        var actualSolution = await dbContext.Solutions.Include(x => x.CatalogueItem.CatalogueItemEpics)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.CatalogueItemId == solutionWithUnknownCapability.CatalogueItemId);

        actualSolution.CatalogueItem.CatalogueItemEpics.Should().HaveCount(1);
        actualSolution.CatalogueItem.CatalogueItemEpics.Should()
            .NotContain(x => x.EpicId == unknownEpicId);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task MapToSolutions_MapsExpectedCapabilitiesAndEpics(
        List<Solution> solutions,
        List<Capability> capabilities,
        List<Epic> epics,
        [Frozen] BuyingCatalogueDbContext dbContext,
        Gen2MappingService service)
    {
        capabilities.ForEach(x => x.Epics = Enumerable.Empty<Epic>().ToList());
        epics.ForEach(
            x =>
            {
                x.Capabilities = Enumerable.Empty<Capability>().ToList();
                x.CapabilityEpics = Enumerable.Empty<CapabilityEpic>().ToList();
            });

        var capabilityEpicRelationships = capabilities.Zip(epics)
            .Select(x => new CapabilityEpic { CapabilityId = x.First.Id, EpicId = x.Second.Id })
            .ToList();

        dbContext.AddRange(solutions);
        dbContext.AddRange(capabilities);
        dbContext.AddRange(epics);
        dbContext.AddRange(capabilityEpicRelationships);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var solutionsAndCapabilities = solutions.Zip(capabilityEpicRelationships).ToList();

        var capabilitiesImport = solutionsAndCapabilities.Select(
                x => new Gen2CapabilitiesCsvModel
                {
                    SolutionId = x.First.CatalogueItemId.ToString(), CapabilityId = $"C{x.Second.CapabilityId}",
                })
            .ToList();

        var epicsImport = solutionsAndCapabilities.Select(
                x => new Gen2EpicsCsvModel
                {
                    SolutionId = x.First.CatalogueItemId.ToString(),
                    CapabilityId = $"C{x.Second.CapabilityId}",
                    EpicId = x.Second.EpicId,
                })
            .ToList();

        await service.MapToSolutions(new Gen2MappingModel(capabilitiesImport, epicsImport));

        var actualCapabilityLinks = await dbContext.CatalogueItemCapabilities.AsNoTracking().ToListAsync();
        var actualEpicLinks = await dbContext.CatalogueItemEpics.AsNoTracking().ToListAsync();

        capabilitiesImport.Should()
            .AllSatisfy(
                x => actualCapabilityLinks.Should().Contain(y => string.Equals($"C{y.CapabilityId}", x.CapabilityId)));

        epicsImport.Should()
            .AllSatisfy(
                x => actualEpicLinks.Should()
                    .Contain(
                        y => string.Equals($"C{y.CapabilityId}", x.CapabilityId) && string.Equals(y.EpicId, x.EpicId)));
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task MapToSolutions_WithAdditionalServices_MapsExpectedCapabilitiesAndEpics(
        List<Solution> solutions,
        List<Capability> capabilities,
        List<Epic> epics,
        [Frozen] BuyingCatalogueDbContext dbContext,
        Gen2MappingService service)
    {
        solutions.ForEach(x => x.AdditionalServices.Select((y, i) => (Service: y, Index: i)).ToList().ForEach(y => y.Service.CatalogueItem.Id = CatalogueItemId.ParseExact($"{x.CatalogueItemId}A{y.Index:D3}")));
        capabilities.ForEach(x => x.Epics = Enumerable.Empty<Epic>().ToList());
        epics.ForEach(
            x =>
            {
                x.Capabilities = Enumerable.Empty<Capability>().ToList();
                x.CapabilityEpics = Enumerable.Empty<CapabilityEpic>().ToList();
            });

        var capabilityEpicRelationships = capabilities.Zip(epics)
            .Select(x => new CapabilityEpic { CapabilityId = x.First.Id, EpicId = x.Second.Id })
            .ToList();

        dbContext.AddRange(solutions);
        dbContext.AddRange(capabilities);
        dbContext.AddRange(epics);
        dbContext.AddRange(capabilityEpicRelationships);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var solutionsAndCapabilities = solutions.Zip(capabilityEpicRelationships).ToList();

        var capabilitiesImport = solutionsAndCapabilities.Select(
                x => new Gen2CapabilitiesCsvModel
                {
                    SolutionId = x.First.CatalogueItemId.ToString(),
                    CapabilityId = $"C{x.Second.CapabilityId}",
                    AdditionalServiceId = x.First.AdditionalServices.First().CatalogueItemId.ToString().AsSpan()[^4..].ToString(),
                })
            .ToList();

        var epicsImport = solutionsAndCapabilities.Select(
                x => new Gen2EpicsCsvModel
                {
                    SolutionId = x.First.CatalogueItemId.ToString(),
                    CapabilityId = $"C{x.Second.CapabilityId}",
                    EpicId = x.Second.EpicId,
                    AdditionalServiceId = x.First.AdditionalServices.First().CatalogueItemId.ToString().AsSpan()[^4..].ToString(),
                })
            .ToList();

        await service.MapToSolutions(new Gen2MappingModel(capabilitiesImport, epicsImport));

        var actualCapabilityLinks = await dbContext.CatalogueItemCapabilities.AsNoTracking().ToListAsync();
        var actualEpicLinks = await dbContext.CatalogueItemEpics.AsNoTracking().ToListAsync();

        capabilitiesImport.Should()
            .AllSatisfy(
                x => actualCapabilityLinks.Should().Contain(y => string.Equals($"C{y.CapabilityId}", x.CapabilityId)));

        epicsImport.Should()
            .AllSatisfy(
                x => actualEpicLinks.Should()
                    .Contain(
                        y => string.Equals($"C{y.CapabilityId}", x.CapabilityId) && string.Equals(y.EpicId, x.EpicId)));
    }
}
