using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Solutions;

public static class SolutionStandardsServiceTests
{
    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetSolutionStandards_WithOverarchingStandard_ReturnsComplianceModelsWithOverarchingStandard(
        Solution solution,
        List<Standard> standards,
        [Frozen] BuyingCatalogueDbContext dbContext,
        SolutionStandardsService service)
    {
        var otherStandards = standards.Skip(1).ToList();
        otherStandards.ForEach(x =>
        {
            x.StandardType = StandardType.Other;
        });

        var overarchingStandard = standards.First();
        overarchingStandard.StandardType = StandardType.Overarching;

        solution.SolutionStandards = [new SolutionStandard { Standard = overarchingStandard, Status = StandardCompliance.FullyMet }];
        solution.CatalogueItem.CatalogueItemCapabilities = [];

        await dbContext.SaveChangesAsync();

        dbContext.AddRange(standards);
        dbContext.Add(solution);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var result = (await service.GetSolutionStandards(solution.CatalogueItemId)).ToList();

        result.Should().NotBeNullOrEmpty();
        result.Should().Contain(x => x.Compliance == StandardCompliance.FullyMet && x.Id == overarchingStandard.Id);
        otherStandards
            .ForEach(x => result.Should().NotContain(y => y.Compliance == StandardCompliance.InProgress && y.Id == x.Id));
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetSolutionStandards_WithNewOverarchingStandard_ReturnsComplianceModelsWithOverarchingStandard(
        Solution solution,
        List<Standard> standards,
        [Frozen] BuyingCatalogueDbContext dbContext,
        SolutionStandardsService service)
    {
        standards.ForEach(x =>
        {
            x.StandardType = StandardType.Overarching;
        });

        var overarchingStandard = standards.First();
        var trackedStandards = standards.Skip(1).ToList();

        solution.SolutionStandards = trackedStandards.Select(x => new SolutionStandard { Standard = x, Status = StandardCompliance.FullyMet }).ToList();
        solution.CatalogueItem.CatalogueItemCapabilities = [];

        await dbContext.SaveChangesAsync();

        dbContext.AddRange(standards);
        dbContext.Add(solution);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var result = (await service.GetSolutionStandards(solution.CatalogueItemId)).ToList();

        result.Should().NotBeNullOrEmpty();
        result.Should().Contain(x => x.Compliance == StandardCompliance.NotYetSelected && x.Id == overarchingStandard.Id);
        trackedStandards
            .ToList()
            .ForEach(x => result.Should().Contain(y => y.Compliance == StandardCompliance.FullyMet && y.Id == x.Id));
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetSolutionStandards_WithCapabilityStandards_ReturnsComplianceModelContainingCapabilitySpecificStandards(
        Solution solution,
        Capability capability,
        List<Standard> standards,
        [Frozen] BuyingCatalogueDbContext dbContext,
        SolutionStandardsService service)
    {
        var otherStandards = standards.Skip(1).ToList();
        otherStandards.ForEach(x =>
        {
            x.StandardType = StandardType.Other;
            x.StandardCapabilities = [new StandardCapability { Capability = capability, }];
        });

        var overarchingStandard = standards.First();
        overarchingStandard.StandardType = StandardType.Overarching;

        var solutionStandards = otherStandards
            .Select(x => new SolutionStandard { Standard = x, Status = StandardCompliance.InProgress })
            .ToList();

        solutionStandards.Add(new SolutionStandard { Standard = overarchingStandard, Status = StandardCompliance.FullyMet });

        solution.SolutionStandards = solutionStandards;
        solution.CatalogueItem.CatalogueItemCapabilities = [new CatalogueItemCapability { Capability = capability, }];

        await dbContext.SaveChangesAsync();

        dbContext.AddRange(standards);
        dbContext.Add(solution);
        dbContext.Add(capability);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var result = (await service.GetSolutionStandards(solution.CatalogueItemId)).ToList();

        result.Should().NotBeNullOrEmpty();
        result.Should().Contain(x => x.Compliance == StandardCompliance.FullyMet && x.Id == overarchingStandard.Id);
        standards.Skip(1)
            .ToList()
            .ForEach(x => result.Should().Contain(y => y.Compliance == StandardCompliance.InProgress && y.Id == x.Id));
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetSolutionStandards_WithNewCapabilityStandard_ReturnsComplianceModelContainingCapabilitySpecificStandards(
        Solution solution,
        Capability capability,
        List<Standard> standards,
        [Frozen] BuyingCatalogueDbContext dbContext,
        SolutionStandardsService service)
    {
        var otherStandards = standards.Skip(1).ToList();
        otherStandards.ForEach(x =>
        {
            x.StandardType = StandardType.Other;
            x.StandardCapabilities = [new StandardCapability { Capability = capability, }];
        });

        var nonMappedStandard = otherStandards.First();

        var overarchingStandard = standards.First();
        overarchingStandard.StandardType = StandardType.Overarching;

        var solutionStandards = otherStandards.Skip(1)
            .Select(x => new SolutionStandard { Standard = x, Status = StandardCompliance.InProgress })
            .ToList();

        solutionStandards.Add(new SolutionStandard { Standard = overarchingStandard, Status = StandardCompliance.FullyMet });

        solution.SolutionStandards = solutionStandards;
        solution.CatalogueItem.CatalogueItemCapabilities = [new CatalogueItemCapability { Capability = capability, }];

        await dbContext.SaveChangesAsync();

        dbContext.AddRange(standards);
        dbContext.Add(solution);
        dbContext.Add(capability);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var result = (await service.GetSolutionStandards(solution.CatalogueItemId)).ToList();

        result.Should().NotBeNullOrEmpty();
        result.Should().Contain(x => x.Compliance == StandardCompliance.FullyMet && x.Id == overarchingStandard.Id);
        result.Should().Contain(x => x.Compliance == StandardCompliance.NotYetSelected && x.Id == nonMappedStandard.Id);
        otherStandards.Skip(1)
            .ToList()
            .ForEach(x => result.Should().Contain(y => y.Compliance == StandardCompliance.InProgress && y.Id == x.Id));
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetSolutionStandard_InProgressStandard_ReturnsExpectedComplianceModel(
        Solution solution,
        Standard standard,
        [Frozen] BuyingCatalogueDbContext dbContext,
        SolutionStandardsService service)
    {
        solution.SolutionStandards =
            [new SolutionStandard { Standard = standard, Status = StandardCompliance.InProgress }];

        standard.StandardCapabilities = Enumerable.Empty<StandardCapability>().ToList();
        solution.CatalogueItem.CatalogueItemCapabilities = Enumerable.Empty<CatalogueItemCapability>().ToList();

        dbContext.Add(standard);
        dbContext.Add(solution);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var result = await service.GetSolutionStandard(solution.CatalogueItemId, standard.Id);

        result.Should().NotBeNull();
        result.Id.Should().Be(standard.Id);
        result.Name.Should().Be(standard.Name);
        result.Description.Should().Be(standard.Description);
        result.Url.Should().Be(standard.Url);
        result.Compliance.Should().Be(StandardCompliance.InProgress);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetSolutionStandard_FullyMetStandard_ReturnsExpectedComplianceModel(
        Solution solution,
        Standard standard,
        [Frozen] BuyingCatalogueDbContext dbContext,
        SolutionStandardsService service)
    {
        solution.SolutionStandards =
            [new SolutionStandard { Standard = standard, Status = StandardCompliance.FullyMet }];
        standard.StandardCapabilities = Enumerable.Empty<StandardCapability>().ToList();
        solution.CatalogueItem.CatalogueItemCapabilities = Enumerable.Empty<CatalogueItemCapability>().ToList();

        dbContext.Add(standard);
        dbContext.Add(solution);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var result = await service.GetSolutionStandard(solution.CatalogueItemId, standard.Id);

        result.Should().NotBeNull();
        result.Id.Should().Be(standard.Id);
        result.Name.Should().Be(standard.Name);
        result.Description.Should().Be(standard.Description);
        result.Url.Should().Be(standard.Url);
        result.Compliance.Should().Be(StandardCompliance.FullyMet);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetInProgressStandards(
        Solution solution,
        List<Standard> standards,
        [Frozen] BuyingCatalogueDbContext dbContext,
        SolutionStandardsService service)
    {
        solution.SolutionStandards = standards
            .Select(x => new SolutionStandard { Standard = x, Status = StandardCompliance.InProgress })
            .ToList();
        standards.ForEach(x => x.StandardCapabilities = Enumerable.Empty<StandardCapability>().ToList());
        solution.CatalogueItem.CatalogueItemCapabilities = Enumerable.Empty<CatalogueItemCapability>().ToList();

        dbContext.AddRange(standards);
        dbContext.Add(solution);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var result = (await service.GetInProgressStandards(solution.CatalogueItemId)).ToList();

        result.Should().NotBeNullOrEmpty();
        result.Should().BeEquivalentTo(standards);
    }

    [Theory]
    [MockInMemoryDbInlineAutoData(StandardCompliance.InProgress)]
    [MockInMemoryDbInlineAutoData(StandardCompliance.FullyMet)]
    [MockInMemoryDbInlineAutoData(StandardCompliance.NotMet)]
    [MockInMemoryDbInlineAutoData(StandardCompliance.NotApplicable)]
    public static async Task SetSolutionStandardStatus_NewStandard_AddsSolutionStandardStatus(
        StandardCompliance compliance,
        Solution solution,
        Standard standard,
        [Frozen] BuyingCatalogueDbContext dbContext,
        SolutionStandardsService service)
    {
        solution.SolutionStandards = Enumerable.Empty<SolutionStandard>().ToList();
        standard.StandardCapabilities = Enumerable.Empty<StandardCapability>().ToList();
        solution.CatalogueItem.CatalogueItemCapabilities = Enumerable.Empty<CatalogueItemCapability>().ToList();

        dbContext.Add(solution);
        dbContext.Add(standard);

        await dbContext.SaveChangesAsync();

        solution.SolutionStandards.Should().BeEmpty();
        dbContext.ChangeTracker.Clear();

        await service.SetSolutionStandardStatus(solution.CatalogueItemId, standard.Id, compliance);

        var updatedSolution = await dbContext.Solutions.AsNoTracking()
            .Include(x => x.SolutionStandards)
            .FirstOrDefaultAsync(x => x.CatalogueItemId == solution.CatalogueItemId);

        updatedSolution.SolutionStandards.Should().NotBeNullOrEmpty();
        updatedSolution.SolutionStandards.Should().Contain(x => x.StandardId == standard.Id);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetSolutionStandardStatus_InProgressToInProgress_UpdatesSolutionStandardStatus(
        Solution solution,
        Standard standard,
        [Frozen] BuyingCatalogueDbContext dbContext,
        SolutionStandardsService service)
    {
        solution.SolutionStandards = [new SolutionStandard { Standard = standard }];
        standard.StandardCapabilities = Enumerable.Empty<StandardCapability>().ToList();
        solution.CatalogueItem.CatalogueItemCapabilities = Enumerable.Empty<CatalogueItemCapability>().ToList();

        dbContext.Add(solution);
        dbContext.Add(standard);

        await dbContext.SaveChangesAsync();

        solution.SolutionStandards.Should().ContainSingle();
        dbContext.ChangeTracker.Clear();

        await service.SetSolutionStandardStatus(solution.CatalogueItemId, standard.Id, StandardCompliance.InProgress);

        var updatedSolution = await dbContext.Solutions.AsNoTracking()
            .Include(x => x.SolutionStandards)
            .FirstOrDefaultAsync(x => x.CatalogueItemId == solution.CatalogueItemId);

        updatedSolution.SolutionStandards.Should().NotBeNullOrEmpty();
        updatedSolution.SolutionStandards.Should().Contain(x => x.StandardId == standard.Id);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetSolutionStandardStatus_InProgressToFullyMet_UpdatesSolutionStandardStatus(
        Solution solution,
        Standard standard,
        [Frozen] BuyingCatalogueDbContext dbContext,
        SolutionStandardsService service)
    {
        solution.SolutionStandards = [new SolutionStandard { Standard = standard }];
        standard.StandardCapabilities = Enumerable.Empty<StandardCapability>().ToList();
        solution.CatalogueItem.CatalogueItemCapabilities = Enumerable.Empty<CatalogueItemCapability>().ToList();

        dbContext.Add(solution);
        dbContext.Add(standard);

        await dbContext.SaveChangesAsync();

        solution.SolutionStandards.Should().ContainSingle();
        dbContext.ChangeTracker.Clear();

        await service.SetSolutionStandardStatus(solution.CatalogueItemId, standard.Id, StandardCompliance.FullyMet);

        var updatedSolution = await dbContext.Solutions.AsNoTracking()
            .Include(x => x.SolutionStandards)
            .FirstOrDefaultAsync(x => x.CatalogueItemId == solution.CatalogueItemId);

        updatedSolution.SolutionStandards.Should()
            .Contain(x => x.StandardId == standard.Id && x.Status == StandardCompliance.FullyMet);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetSolutionStandardStatus_InProgressToFullyMet_RemovesWorkOffPlans(
        Solution solution,
        Standard standard,
        List<WorkOffPlan> workOffPlans,
        [Frozen] BuyingCatalogueDbContext dbContext,
        SolutionStandardsService service)
    {
        workOffPlans.ForEach(x =>
        {
            x.SolutionId = solution.CatalogueItemId;
            x.StandardId = standard.Id;
            x.Solution = null;
            x.Standard = null;
        });
        solution.SolutionStandards =
            [new SolutionStandard { Standard = standard, Status = StandardCompliance.InProgress }];
        standard.StandardCapabilities = Enumerable.Empty<StandardCapability>().ToList();
        solution.CatalogueItem.CatalogueItemCapabilities = Enumerable.Empty<CatalogueItemCapability>().ToList();
        solution.WorkOffPlans = Enumerable.Empty<WorkOffPlan>().ToList();

        dbContext.Add(solution);
        dbContext.Add(standard);
        dbContext.WorkOffPlans.AddRange(workOffPlans);

        await dbContext.SaveChangesAsync();

        solution.SolutionStandards.Should().ContainSingle();
        solution.WorkOffPlans.Should().NotBeEmpty();
        dbContext.ChangeTracker.Clear();

        await service.SetSolutionStandardStatus(solution.CatalogueItemId, standard.Id, StandardCompliance.FullyMet);

        var updatedSolution = await dbContext.Solutions.AsNoTracking()
            .Include(x => x.SolutionStandards)
            .FirstOrDefaultAsync(x => x.CatalogueItemId == solution.CatalogueItemId);

        var updatedWorkOffPlans = await dbContext.WorkOffPlans
            .Where(x => x.SolutionId == solution.CatalogueItemId && x.StandardId == standard.Id)
            .ToListAsync();

        updatedSolution.SolutionStandards.Should()
            .Contain(x => x.StandardId == standard.Id && x.Status == StandardCompliance.FullyMet);
        updatedWorkOffPlans.Should().BeEmpty();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetSolutionStandardStatus_FullyMetToFullyMet_DoesNothing(
        Solution solution,
        Standard standard,
        [Frozen] BuyingCatalogueDbContext dbContext,
        SolutionStandardsService service)
    {
        solution.SolutionStandards = Enumerable.Empty<SolutionStandard>().ToList();
        standard.StandardCapabilities = Enumerable.Empty<StandardCapability>().ToList();
        solution.CatalogueItem.CatalogueItemCapabilities = Enumerable.Empty<CatalogueItemCapability>().ToList();

        dbContext.Add(solution);
        dbContext.Add(standard);

        await dbContext.SaveChangesAsync();

        solution.SolutionStandards.Should().BeEmpty();
        dbContext.ChangeTracker.Clear();

        await service.SetSolutionStandardStatus(solution.CatalogueItemId, standard.Id, StandardCompliance.FullyMet);

        var updatedSolution = await dbContext.Solutions.AsNoTracking()
            .Include(x => x.SolutionStandards)
            .FirstOrDefaultAsync(x => x.CatalogueItemId == solution.CatalogueItemId);

        updatedSolution.SolutionStandards.Should()
            .Contain(x => x.StandardId == standard.Id && x.Status == StandardCompliance.FullyMet);
    }
}
