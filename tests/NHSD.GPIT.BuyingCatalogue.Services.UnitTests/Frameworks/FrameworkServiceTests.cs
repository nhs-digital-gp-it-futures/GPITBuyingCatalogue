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
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.Services.Framework;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Frameworks;

public static class FrameworkServiceTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(FrameworkService).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task GetFrameworksByCatalogueItems_PublishedItem_ReturnsExpected(
        EntityFramework.Catalogue.Models.Framework framework,
        FrameworkSolution frameworkSolution,
        CatalogueItem catalogueItem,
        Solution solution,
        [Frozen] BuyingCatalogueDbContext dbContext,
        FrameworkService service)
    {
        dbContext.FrameworkSolutions.RemoveRange(dbContext.FrameworkSolutions);

        framework.IsExpired = false;
        catalogueItem.CatalogueItemType = CatalogueItemType.Solution;
        catalogueItem.PublishedStatus = PublicationStatus.Published;
        solution.FrameworkSolutions.Clear();

        solution.CatalogueItem = catalogueItem;
        frameworkSolution.Solution = solution;
        frameworkSolution.Framework = framework;

        dbContext.FrameworkSolutions.Add(frameworkSolution);
        dbContext.Frameworks.Add(framework);
        dbContext.CatalogueItems.Add(catalogueItem);
        dbContext.Solutions.Add(solution);

        await dbContext.SaveChangesAsync();

        var expectedFrameworks = new List<FrameworkFilterInfo>
        {
            new() { Id = framework.Id, ShortName = framework.ShortName, CountOfActiveSolutions = 1 },
        };

        var result = await service.GetFrameworksByCatalogueItems(new List<CatalogueItemId> { catalogueItem.Id });

        result.Should().HaveCount(1);
        result.Should().BeEquivalentTo(expectedFrameworks);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task GetFrameworksByCatalogueItems_ExpiredFramework_ReturnsExpected(
        EntityFramework.Catalogue.Models.Framework framework,
        FrameworkSolution frameworkSolution,
        CatalogueItem catalogueItem,
        Solution solution,
        [Frozen] BuyingCatalogueDbContext dbContext,
        FrameworkService service)
    {
        dbContext.FrameworkSolutions.RemoveRange(dbContext.FrameworkSolutions);

        framework.IsExpired = true;
        catalogueItem.CatalogueItemType = CatalogueItemType.Solution;
        catalogueItem.PublishedStatus = PublicationStatus.Published;
        solution.FrameworkSolutions.Clear();

        solution.CatalogueItem = catalogueItem;
        frameworkSolution.Solution = solution;
        frameworkSolution.Framework = framework;

        dbContext.FrameworkSolutions.Add(frameworkSolution);
        dbContext.Frameworks.Add(framework);
        dbContext.CatalogueItems.Add(catalogueItem);
        dbContext.Solutions.Add(solution);

        await dbContext.SaveChangesAsync();

        var result = await service.GetFrameworksByCatalogueItems(new List<CatalogueItemId> { catalogueItem.Id });

        result.Should().HaveCount(1);
        result.All(f => f.Expired);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task GetFrameworksByCatalogueItems_NoCatalogueItems_ReturnsExpected(
    EntityFramework.Catalogue.Models.Framework framework,
    FrameworkSolution frameworkSolution,
    Solution solution,
    [Frozen] BuyingCatalogueDbContext dbContext,
    FrameworkService service)
    {
        dbContext.FrameworkSolutions.RemoveRange(dbContext.FrameworkSolutions);

        framework.IsExpired = false;
        solution.FrameworkSolutions.Clear();
        frameworkSolution.Solution = solution;
        frameworkSolution.Framework = framework;

        dbContext.FrameworkSolutions.Add(frameworkSolution);
        dbContext.Frameworks.Add(framework);
        dbContext.Solutions.Add(solution);

        await dbContext.SaveChangesAsync();

        var expectedFrameworks = new List<FrameworkFilterInfo>
        {
            new() { Id = framework.Id, ShortName = framework.ShortName, CountOfActiveSolutions = 0 },
        };

        var result = await service.GetFrameworksByCatalogueItems(new List<CatalogueItemId>());

        result.Should().HaveCount(1);
        result.Should().BeEquivalentTo(expectedFrameworks);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task GetFrameworksByCatalogueItem_UnpublishedItem_ReturnsExpected(
        EntityFramework.Catalogue.Models.Framework framework,
        FrameworkSolution frameworkSolution,
        CatalogueItem catalogueItem,
        Solution solution,
        [Frozen] BuyingCatalogueDbContext dbContext,
        FrameworkService service)
    {
        dbContext.FrameworkSolutions.RemoveRange(dbContext.FrameworkSolutions);

        catalogueItem.CatalogueItemType = CatalogueItemType.Solution;
        catalogueItem.PublishedStatus = PublicationStatus.Unpublished;
        solution.FrameworkSolutions.Clear();

        solution.CatalogueItem = catalogueItem;
        frameworkSolution.Solution = solution;
        frameworkSolution.Framework = framework;

        dbContext.FrameworkSolutions.Add(frameworkSolution);
        dbContext.Frameworks.Add(framework);
        dbContext.CatalogueItems.Add(catalogueItem);
        dbContext.Solutions.Add(solution);

        await dbContext.SaveChangesAsync();

        var result = await service.GetFrameworksByCatalogueItems(new List<CatalogueItemId> { catalogueItem.Id });

        result.Should().BeEmpty();
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task GetFramework_ReturnsExpected(
        EntityFramework.Catalogue.Models.Framework framework,
        [Frozen] BuyingCatalogueDbContext dbContext,
        FrameworkService service)
    {
        dbContext.Frameworks.Add(framework);
        await dbContext.SaveChangesAsync();

        var result = await service.GetFramework(framework.Id);

        result.Should().BeEquivalentTo(framework);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task GetFrameworks_ReturnsExpected(
        List<EntityFramework.Catalogue.Models.Framework> frameworks,
        [Frozen] BuyingCatalogueDbContext dbContext,
        FrameworkService service)
    {
        dbContext.Frameworks.RemoveRange(dbContext.Frameworks);
        dbContext.Frameworks.AddRange(frameworks);
        await dbContext.SaveChangesAsync();

        var result = await service.GetFrameworks();

        result.Should().BeEquivalentTo(frameworks);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static Task AddFramework_NullName_ThrowsException(FrameworkService service) => FluentActions
        .Invoking(() => service.AddFramework(null, Enumerable.Empty<FundingType>()))
        .Should()
        .ThrowAsync<ArgumentException>();

    [Theory]
    [InMemoryDbAutoData]
    public static Task AddFramework_NullFundingType_ThrowsException(string name, FrameworkService service) => FluentActions
        .Invoking(() => service.AddFramework(name, null))
        .Should()
        .ThrowAsync<ArgumentException>();

    [Theory]
    [InMemoryDbAutoData]
    public static async Task<Task<ArgumentException>> EditFramework_NullName_ThrowsException(
        FrameworkService service,
        List<EntityFramework.Catalogue.Models.Framework> frameworks,
        [Frozen] BuyingCatalogueDbContext dbContext)
    {
        string id = frameworks.FirstOrDefault().Id;
        dbContext.Frameworks.RemoveRange(dbContext.Frameworks);
        dbContext.Frameworks.AddRange(frameworks);
        await dbContext.SaveChangesAsync();

        return Assert.ThrowsAsync<ArgumentException>(() => service.UpdateFramework(id, null, Enumerable.Empty<FundingType>()));
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task EditFramework_Valid_UpdatesFramework(
        FrameworkService service,
        List<EntityFramework.Catalogue.Models.Framework> frameworks,
        [Frozen] BuyingCatalogueDbContext dbContext,
        List<FundingType> fundingTypes)
    {
        var frameworkId = frameworks.First().Id;
        string newName = "New Name";

        dbContext.Frameworks.RemoveRange(dbContext.Frameworks);
        dbContext.Frameworks.AddRange(frameworks);
        await dbContext.SaveChangesAsync();

        await service.UpdateFramework(frameworkId, newName, fundingTypes);

        dbContext.Frameworks.AsNoTracking()
            .FirstOrDefault(x => x.Id == frameworkId && x.Name == newName && x.FundingTypes == fundingTypes)
            .Should()
            .NotBeNull();
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task MarkAsExpired_InvalidFramework_DoesNothing(
        string frameworkId,
        List<EntityFramework.Catalogue.Models.Framework> frameworks,
        [Frozen] BuyingCatalogueDbContext dbContext,
        FrameworkService service)
    {
        var activeFrameworks = frameworks.Count(x => !x.IsExpired);

        dbContext.Frameworks.RemoveRange(dbContext.Frameworks);
        dbContext.Frameworks.AddRange(frameworks);
        await dbContext.SaveChangesAsync();

        await service.MarkAsExpired(frameworkId);

        dbContext.Frameworks.AsNoTracking().Count(x => !x.IsExpired).Should().Be(activeFrameworks);
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task MarkAsExpired_Valid_ExpiresFramework(
        List<EntityFramework.Catalogue.Models.Framework> frameworks,
        [Frozen] BuyingCatalogueDbContext dbContext,
        FrameworkService service)
    {
        var frameworkId = frameworks.First().Id;

        dbContext.Frameworks.RemoveRange(dbContext.Frameworks);
        dbContext.Frameworks.AddRange(frameworks);
        await dbContext.SaveChangesAsync();

        await service.MarkAsExpired(frameworkId);

        dbContext.Frameworks.AsNoTracking()
            .FirstOrDefault(x => x.Id == frameworkId && x.IsExpired == true)
            .Should()
            .NotBeNull();
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task FrameworkNameExists_ReturnsTrue(
        EntityFramework.Catalogue.Models.Framework framework,
        [Frozen] BuyingCatalogueDbContext dbContext,
        FrameworkService service)
    {
        dbContext.Frameworks.Add(framework);
        await dbContext.SaveChangesAsync();

        var result = await service.FrameworkNameExists(framework.ShortName);

        result.Should().BeTrue();
    }

    [Theory]
    [InMemoryDbAutoData]
    public static async Task FrameworkNameExists_ReturnsFalse(
        string frameworkName,
        FrameworkService service)
    {
        var result = await service.FrameworkNameExists(frameworkName);

        result.Should().BeFalse();
    }
}
