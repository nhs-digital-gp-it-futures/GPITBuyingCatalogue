using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Services.Integrations;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Integrations;

public static class IntegrationsServiceTests
{
    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetIntegrations_ReturnsIntegrations(
        List<Integration> integrations,
        [Frozen] BuyingCatalogueDbContext dbContext,
        IntegrationsService service)
    {
        dbContext.Integrations.RemoveRange(dbContext.Integrations);
        dbContext.Integrations.AddRange(integrations);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var serviceIntegrations = await service.GetIntegrations();

        serviceIntegrations.Should().BeEquivalentTo(integrations, opt => opt.Excluding(x => x.IntegrationTypes));
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetIntegrationsWithTypes_ReturnsIntegrations(
        List<Integration> integrations,
        [Frozen] BuyingCatalogueDbContext dbContext,
        IntegrationsService service)
    {
        integrations.ForEach(x => x.IntegrationTypes = new List<IntegrationType> { new() { Name = "Test" } });

        dbContext.Integrations.RemoveRange(dbContext.Integrations);
        dbContext.Integrations.AddRange(integrations);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var serviceIntegrations = (await service.GetIntegrationsWithTypes()).ToList();

        serviceIntegrations.Should().BeEquivalentTo(integrations, opt => opt.Excluding(x => x.IntegrationTypes));
        serviceIntegrations.SelectMany(x => x.IntegrationTypes)
            .Should()
            .BeEquivalentTo(integrations.SelectMany(x => x.IntegrationTypes), opt => opt.Excluding(x => x.Integration));
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetIntegrationAndTypeNames_ReturnsIntegrationAndTypeNames(
        List<Integration> integrations,
        [Frozen] BuyingCatalogueDbContext dbContext,
        IntegrationsService service)
    {
        integrations.ForEach(x => x.IntegrationTypes = new List<IntegrationType> { new() { Name = "Test" } });

        dbContext.Integrations.RemoveRange(dbContext.Integrations);
        dbContext.Integrations.AddRange(integrations);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var integrationAndTypeIds = integrations.ToDictionary(
            x => x.Id,
            x => x.IntegrationTypes.Select(y => y.Id).ToArray());

        var serviceIntegrations = (await service.GetIntegrationAndTypeNames(integrationAndTypeIds)).ToList();

        var expectedNames = integrations.ToDictionary(x => x.Name, x => x.IntegrationTypes.Select(y => y.Name).Order());

        serviceIntegrations.Should().BeEquivalentTo(expectedNames);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetIntegrationTypesByIntegration(
        List<Integration> integrations,
        [Frozen] BuyingCatalogueDbContext dbContext,
        IntegrationsService service)
    {
        integrations.ForEach(x => x.IntegrationTypes = new List<IntegrationType> { new() { Name = "Test" } });

        dbContext.Integrations.RemoveRange(dbContext.Integrations);
        dbContext.Integrations.AddRange(integrations);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var integration = integrations.First();

        var integrationTypes = await service.GetIntegrationTypesByIntegration(integration.Id);

        integrationTypes.Should()
            .BeEquivalentTo(integration.IntegrationTypes, opt => opt.Excluding(x => x.Integration));
    }
}
