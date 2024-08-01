using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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
    public static async Task GetIntegrationWithTypes_ReturnsIntegration(
        Integration integration,
        List<IntegrationType> integrationTypes,
        [Frozen] BuyingCatalogueDbContext dbContext,
        IntegrationsService service)
    {
        integrationTypes.ForEach(
            x =>
            {
                x.Integration = null;
                x.IntegrationId = integration.Id;
            });

        integration.IntegrationTypes.Clear();

        dbContext.Add(integration);
        dbContext.AddRange(integrationTypes);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var result = await service.GetIntegrationWithTypes(integration.Id);

        result.Should().BeEquivalentTo(integration, opt => opt.Excluding(m => m.IntegrationTypes));
        result.IntegrationTypes.Should().BeEquivalentTo(integrationTypes, opt => opt.Excluding(m => m.Integration));
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

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetIntegrationTypeById_ReturnsIntegrationType(
        Integration integration,
        IntegrationType integrationType,
        [Frozen] BuyingCatalogueDbContext dbContext,
        IntegrationsService service)
    {
        integration.IntegrationTypes.Clear();
        integrationType.Integration = null;
        integrationType.IntegrationId = integration.Id;

        dbContext.Add(integration);
        dbContext.Add(integrationType);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var result = await service.GetIntegrationTypeById(integration.Id, integrationType.Id);

        result.Should().BeEquivalentTo(integrationType, opt => opt.Excluding(m => m.Integration));
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task GetIntegrationTypeById_WithReferences_ReturnsIntegrationType(
        Solution solution,
        Integration integration,
        IntegrationType integrationType,
        [Frozen] BuyingCatalogueDbContext dbContext,
        IntegrationsService service)
    {
        integration.IntegrationTypes.Clear();
        integrationType.Integration = null;
        integrationType.IntegrationId = integration.Id;
        integrationType.Solutions.Add(solution);

        dbContext.Add(solution);
        dbContext.Add(integration);
        dbContext.Add(integrationType);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var result = await service.GetIntegrationTypeById(integration.Id, integrationType.Id);

        result.Solutions.Should().ContainSingle();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task IntegrationTypeExists_DuplicateIntegrationTypeName_ReturnsExpected(
        Integration integration,
        IntegrationType integrationType,
        [Frozen] BuyingCatalogueDbContext dbContext,
        IntegrationsService service)
    {
        integration.IntegrationTypes.Clear();
        integrationType.Integration = null;
        integrationType.IntegrationId = integration.Id;

        dbContext.Add(integration);
        dbContext.Add(integrationType);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var result = await service.IntegrationTypeExists(integration.Id, integrationType.Name, null);

        result.Should().BeTrue();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task IntegrationTypeExists_NewIntegrationTypeName_ReturnsExpected(
        Integration integration,
        IntegrationType integrationType,
        string newIntegrationTypeName,
        [Frozen] BuyingCatalogueDbContext dbContext,
        IntegrationsService service)
    {
        integration.IntegrationTypes.Clear();
        integrationType.Integration = null;
        integrationType.IntegrationId = integration.Id;

        dbContext.Add(integration);
        dbContext.Add(integrationType);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var result = await service.IntegrationTypeExists(integration.Id, newIntegrationTypeName, null);

        result.Should().BeFalse();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task IntegrationTypeExists_ReturnsExpected(
        Integration integration,
        IntegrationType integrationType,
        [Frozen] BuyingCatalogueDbContext dbContext,
        IntegrationsService service)
    {
        integration.IntegrationTypes.Clear();
        integrationType.Integration = null;
        integrationType.IntegrationId = integration.Id;

        dbContext.Add(integration);
        dbContext.Add(integrationType);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var result = await service.IntegrationTypeExists(integration.Id, integrationType.Name, integrationType.Id);

        result.Should().BeFalse();
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static Task AddIntegrationType_NullName_ThrowsArgumentNullException(
        SupportedIntegrations integrationId,
        IntegrationsService service) => FluentActions
        .Awaiting(() => service.AddIntegrationType(integrationId, null, null))
        .Should()
        .ThrowAsync<ArgumentNullException>();

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task AddIntegrationType_AddsIntegrationTypeToIntegration(
        string name,
        string description,
        Integration integration,
        [Frozen] BuyingCatalogueDbContext dbContext,
        IntegrationsService service)
    {
        dbContext.Add(integration);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        await service.AddIntegrationType(integration.Id, name, description);

        var integrationWithTypes = await dbContext.Integrations.Include(x => x.IntegrationTypes)
            .FirstOrDefaultAsync(x => x.Id == integration.Id);

        integrationWithTypes.Should().NotBeNull();
        integrationWithTypes.IntegrationTypes.Should().Contain(x => x.Name == name && x.Description == description);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static Task EditIntegrationType_NullName_ThrowsArgumentNullException(
        SupportedIntegrations integrationId,
        int integrationTypeId,
        IntegrationsService service) => FluentActions
        .Awaiting(() => service.EditIntegrationType(integrationId, integrationTypeId, null, null))
        .Should()
        .ThrowAsync<ArgumentNullException>();

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task EditIntegrationType_InvalidIntegrationTypeId_DoesNotEdit(
        string name,
        string description,
        int invalidIntegrationTypeId,
        Integration integration,
        IntegrationType integrationType,
        [Frozen] BuyingCatalogueDbContext dbContext,
        IntegrationsService service)
    {
        integrationType.IntegrationId = integration.Id;

        dbContext.Add(integration);
        dbContext.Add(integrationType);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        await service.EditIntegrationType(integration.Id, invalidIntegrationTypeId, name, description);

        var integrationWithTypes = await dbContext.Integrations.Include(x => x.IntegrationTypes)
            .FirstOrDefaultAsync(x => x.Id == integration.Id);

        integrationWithTypes.Should().NotBeNull();
        integrationWithTypes.IntegrationTypes.Should().NotContain(x => x.Name == name && x.Description == description);
        integrationWithTypes.IntegrationTypes.Should().Contain(x => x.Name == integrationType.Name && x.Description == integrationType.Description);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task EditIntegrationType_EditsIntegrationType(
        string name,
        string description,
        Integration integration,
        IntegrationType integrationType,
        [Frozen] BuyingCatalogueDbContext dbContext,
        IntegrationsService service)
    {
        integrationType.IntegrationId = integration.Id;

        dbContext.Add(integration);
        dbContext.Add(integrationType);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        await service.EditIntegrationType(integration.Id, integrationType.Id, name, description);

        var integrationWithTypes = await dbContext.Integrations.Include(x => x.IntegrationTypes)
            .FirstOrDefaultAsync(x => x.Id == integration.Id);

        integrationWithTypes.Should().NotBeNull();
        integrationWithTypes.IntegrationTypes.Should().Contain(x => x.Name == name && x.Description == description);
        integrationWithTypes.IntegrationTypes.Should().NotContain(x => x.Name == integrationType.Name && x.Description == integrationType.Description);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task EditIntegrationType_WithWhitespaceInDescription_TrimsDescription(
        string name,
        string description,
        Integration integration,
        IntegrationType integrationType,
        [Frozen] BuyingCatalogueDbContext dbContext,
        IntegrationsService service)
    {
        integrationType.IntegrationId = integration.Id;

        dbContext.Add(integration);
        dbContext.Add(integrationType);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        var paddedDescription = $"    {description}    ";

        await service.EditIntegrationType(integration.Id, integrationType.Id, name, paddedDescription);

        var integrationWithTypes = await dbContext.Integrations.Include(x => x.IntegrationTypes)
            .FirstOrDefaultAsync(x => x.Id == integration.Id);

        integrationWithTypes.Should().NotBeNull();
        integrationWithTypes.IntegrationTypes.Should().Contain(x => x.Name == name && x.Description == description);
    }

    [Theory]
    [MockInMemoryDbAutoData]
    public static async Task EditIntegrationType_WithNullDescription_UpdatesDescription(
        string name,
        Integration integration,
        IntegrationType integrationType,
        [Frozen] BuyingCatalogueDbContext dbContext,
        IntegrationsService service)
    {
        integrationType.IntegrationId = integration.Id;

        dbContext.Add(integration);
        dbContext.Add(integrationType);

        await dbContext.SaveChangesAsync();
        dbContext.ChangeTracker.Clear();

        await service.EditIntegrationType(integration.Id, integrationType.Id, name, null);

        var integrationWithTypes = await dbContext.Integrations.Include(x => x.IntegrationTypes)
            .FirstOrDefaultAsync(x => x.Id == integration.Id);

        integrationWithTypes.Should().NotBeNull();
        integrationWithTypes.IntegrationTypes.Should().Contain(x => x.Name == name && x.Description == null);
    }
}
