using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Solutions;

public static class SolutionStandardsServiceTests
{
    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetSolutionStandards_ReturnsExpectedComplianceModels(
        Solution solution,
        Capability capability,
        List<Standard> otherStandards,
        List<Standard> overarchingStandards,
        [Frozen] BuyingCatalogueDbContext dbContext,
        SolutionStandardsService service)
    {
        otherStandards.ForEach(x =>
        {
            x.StandardType = StandardType.Other;
            x.StandardCapabilities = new List<StandardCapability>();
        });

        overarchingStandards.ForEach(x =>
        {
            x.StandardType = StandardType.Overarching;
            x.StandardCapabilities = new List<StandardCapability>();
        });

        var overarchingStandard = overarchingStandards.First();
        var otherStandard = otherStandards.First();
        capability.CatalogueItemCapabilities = new List<CatalogueItemCapability>();
        capability.StandardCapabilities = new List<StandardCapability> { new(otherStandard.Id, capability.Id) };

        solution.InProgressStandards = new List<Standard> { overarchingStandard };
        solution.CatalogueItem.CatalogueItemCapabilities =
            new List<CatalogueItemCapability> { new(solution.CatalogueItemId, capability.Id) };

        dbContext.AddRange(otherStandards);
        dbContext.AddRange(overarchingStandards);
        dbContext.Add(capability);
        dbContext.Add(solution);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var result = (await service.GetSolutionStandards(solution.CatalogueItemId)).ToList();

        result.Should().NotBeNullOrEmpty();
        result.Should().Contain(x => x.Compliance == StandardCompliance.InProgress && x.Id == overarchingStandard.Id);
        result.Should().Contain(x => x.Compliance == StandardCompliance.FullyMet && x.Id == otherStandard.Id);
        otherStandards.Skip(1)
            .ToList()
            .ForEach(x => result.Should().NotContain(y => y.Compliance == StandardCompliance.FullyMet && y.Id == x.Id));
        overarchingStandards.Skip(1)
            .ToList()
            .ForEach(x => result.Should().Contain(y => y.Compliance == StandardCompliance.FullyMet && y.Id == x.Id));
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetSolutionStandard_InProgressStandard_ReturnsExpectedComplianceModel(
        Solution solution,
        Standard standard,
        [Frozen] BuyingCatalogueDbContext dbContext,
        SolutionStandardsService service)
    {
        solution.InProgressStandards = new List<Standard> { standard };

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
        solution.InProgressStandards = Enumerable.Empty<Standard>().ToList();
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
        solution.InProgressStandards = standards;
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
    [MockInMemoryDbAutoData]
    public static async Task SetSolutionStandardStatus_InProgress_UpdatesSolutionStandardStatus(
        Solution solution,
        Standard standard,
        [Frozen] BuyingCatalogueDbContext dbContext,
        SolutionStandardsService service)
    {
        solution.InProgressStandards = Enumerable.Empty<Standard>().ToList();
        standard.StandardCapabilities = Enumerable.Empty<StandardCapability>().ToList();
        solution.CatalogueItem.CatalogueItemCapabilities = Enumerable.Empty<CatalogueItemCapability>().ToList();

        dbContext.Add(solution);
        dbContext.Add(standard);

        await dbContext.SaveChangesAsync();

        solution.InProgressStandards.Should().BeEmpty();
        dbContext.ChangeTracker.Clear();

        await service.SetSolutionStandardStatus(solution.CatalogueItemId, standard.Id, StandardCompliance.InProgress);

        var updatedSolution = await dbContext.Solutions.AsNoTracking()
            .Include(x => x.InProgressStandards)
            .FirstOrDefaultAsync(x => x.CatalogueItemId == solution.CatalogueItemId);

        updatedSolution.InProgressStandards.Should().NotBeNullOrEmpty();
        updatedSolution.InProgressStandards.Should().Contain(x => x.Id == standard.Id);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetSolutionStandardStatus_InProgressToInProgress_UpdatesSolutionStandardStatus(
        Solution solution,
        Standard standard,
        [Frozen] BuyingCatalogueDbContext dbContext,
        SolutionStandardsService service)
    {
        solution.InProgressStandards = new List<Standard> { standard };
        standard.StandardCapabilities = Enumerable.Empty<StandardCapability>().ToList();
        solution.CatalogueItem.CatalogueItemCapabilities = Enumerable.Empty<CatalogueItemCapability>().ToList();

        dbContext.Add(solution);
        dbContext.Add(standard);

        await dbContext.SaveChangesAsync();

        solution.InProgressStandards.Should().ContainSingle();
        dbContext.ChangeTracker.Clear();

        await service.SetSolutionStandardStatus(solution.CatalogueItemId, standard.Id, StandardCompliance.InProgress);

        var updatedSolution = await dbContext.Solutions.AsNoTracking()
            .Include(x => x.InProgressStandards)
            .FirstOrDefaultAsync(x => x.CatalogueItemId == solution.CatalogueItemId);

        updatedSolution.InProgressStandards.Should().NotBeNullOrEmpty();
        updatedSolution.InProgressStandards.Should().Contain(x => x.Id == standard.Id);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task SetSolutionStandardStatus_InProgressToFullyMet_UpdatesSolutionStandardStatus(
        Solution solution,
        Standard standard,
        [Frozen] BuyingCatalogueDbContext dbContext,
        SolutionStandardsService service)
    {
        solution.InProgressStandards = new List<Standard> { standard };
        standard.StandardCapabilities = Enumerable.Empty<StandardCapability>().ToList();
        solution.CatalogueItem.CatalogueItemCapabilities = Enumerable.Empty<CatalogueItemCapability>().ToList();

        dbContext.Add(solution);
        dbContext.Add(standard);

        await dbContext.SaveChangesAsync();

        solution.InProgressStandards.Should().ContainSingle();
        dbContext.ChangeTracker.Clear();

        await service.SetSolutionStandardStatus(solution.CatalogueItemId, standard.Id, StandardCompliance.FullyMet);

        var updatedSolution = await dbContext.Solutions.AsNoTracking()
            .Include(x => x.InProgressStandards)
            .FirstOrDefaultAsync(x => x.CatalogueItemId == solution.CatalogueItemId);

        updatedSolution.InProgressStandards.Should().BeEmpty();
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
        solution.InProgressStandards = new List<Standard> { standard };
        standard.StandardCapabilities = Enumerable.Empty<StandardCapability>().ToList();
        solution.CatalogueItem.CatalogueItemCapabilities = Enumerable.Empty<CatalogueItemCapability>().ToList();
        solution.WorkOffPlans = Enumerable.Empty<WorkOffPlan>().ToList();

        dbContext.Add(solution);
        dbContext.Add(standard);
        dbContext.WorkOffPlans.AddRange(workOffPlans);

        await dbContext.SaveChangesAsync();

        solution.InProgressStandards.Should().ContainSingle();
        solution.WorkOffPlans.Should().NotBeEmpty();
        dbContext.ChangeTracker.Clear();

        await service.SetSolutionStandardStatus(solution.CatalogueItemId, standard.Id, StandardCompliance.FullyMet);

        var updatedSolution = await dbContext.Solutions.AsNoTracking()
            .Include(x => x.InProgressStandards)
            .FirstOrDefaultAsync(x => x.CatalogueItemId == solution.CatalogueItemId);

        var updatedWorkOffPlans = await dbContext.WorkOffPlans
            .Where(x => x.SolutionId == solution.CatalogueItemId && x.StandardId == standard.Id)
            .ToListAsync();

        updatedSolution.InProgressStandards.Should().BeEmpty();
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
        solution.InProgressStandards = Enumerable.Empty<Standard>().ToList();
        standard.StandardCapabilities = Enumerable.Empty<StandardCapability>().ToList();
        solution.CatalogueItem.CatalogueItemCapabilities = Enumerable.Empty<CatalogueItemCapability>().ToList();

        dbContext.Add(solution);
        dbContext.Add(standard);

        await dbContext.SaveChangesAsync();

        solution.InProgressStandards.Should().BeEmpty();
        dbContext.ChangeTracker.Clear();

        await service.SetSolutionStandardStatus(solution.CatalogueItemId, standard.Id, StandardCompliance.FullyMet);

        var updatedSolution = await dbContext.Solutions.AsNoTracking()
            .Include(x => x.InProgressStandards)
            .FirstOrDefaultAsync(x => x.CatalogueItemId == solution.CatalogueItemId);

        updatedSolution.InProgressStandards.Should().BeEmpty();
    }
}
