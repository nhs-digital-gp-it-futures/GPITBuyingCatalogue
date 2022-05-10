using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Services.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Solutions
{
    public static class InteroperabilityServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(InteroperabilityService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task SaveIntegrationLink_UpdatesDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            string integrationLink,
            InteroperabilityService service)
        {
            context.Solutions.Add(solution);
            context.SaveChanges();

            await service.SaveIntegrationLink(solution.CatalogueItemId, integrationLink);

            var updatedSolution = await context.Solutions.SingleAsync();
            updatedSolution.IntegrationsUrl.Should().Be(integrationLink);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static Task AddIntegration_NullIntegration_ThrowsException(InteroperabilityService service)
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() => service.AddIntegration(default, null));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task AddIntegration_UpdatesDatabase(
           [Frozen] BuyingCatalogueDbContext context,
           Solution solution,
           List<Integration> currentIntegrations,
           Integration newIntegration,
           InteroperabilityService service)
        {
            solution.Integrations = JsonSerializer.Serialize(currentIntegrations);
            solution.AdditionalServices.Clear();

            context.Solutions.Add(solution);
            await context.SaveChangesAsync();

            await service.AddIntegration(solution.CatalogueItemId, newIntegration);
            var updatedSolution = await context.Solutions.SingleAsync(s => s.CatalogueItemId == solution.CatalogueItemId);
            updatedSolution.GetIntegrations().Should().Contain(i => i.Description == newIntegration.Description);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task EditIntegration_UpdatesDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            List<Integration> integrations,
            Integration updatedIntegration,
            InteroperabilityService service)
        {
            solution.Integrations = JsonSerializer.Serialize(integrations);
            updatedIntegration.Id = integrations[0].Id;

            context.Solutions.Add(solution);
            await context.SaveChangesAsync();

            await service.EditIntegration(solution.CatalogueItemId, updatedIntegration.Id, updatedIntegration);
            var updatedSolution = await context.Solutions.SingleAsync(s => s.CatalogueItemId == solution.CatalogueItemId);
            updatedSolution.GetIntegrations().Should().ContainEquivalentOf(updatedIntegration);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static Task EditIntegration_NullIntegrationThrowsException(InteroperabilityService service)
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() => service.EditIntegration(default, default, null));
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task GetIntegrationById_ReturnsIntegration(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            List<Integration> integrations,
            InteroperabilityService service)
        {
            solution.Integrations = JsonSerializer.Serialize(integrations);
            context.Solutions.Add(solution);

            await context.SaveChangesAsync();

            var integration = await service.GetIntegrationById(solution.CatalogueItemId, integrations[0].Id);

            integration.Should().BeEquivalentTo(integrations[0]);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task DeleteIntegration_UpdatesDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            List<Integration> integrations,
            InteroperabilityService service)
        {
            solution.Integrations = JsonSerializer.Serialize(integrations);
            var deletedIntegration = integrations[0];

            context.Solutions.Add(solution);
            await context.SaveChangesAsync();

            await service.DeleteIntegration(solution.CatalogueItemId, deletedIntegration.Id);
            var updatedSolution = await context.Solutions.SingleAsync(s => s.CatalogueItemId == solution.CatalogueItemId);
            updatedSolution.GetIntegrations().Should().NotContainEquivalentOf(deletedIntegration);
        }

        [Theory]
        [InMemoryDbAutoData]
        public static async Task DeleteIntegration_NullIntegrationDoesNotUpdateDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            List<Integration> integrations,
            InteroperabilityService service,
            Guid invalidIntegrationId)
        {
            solution.Integrations = JsonSerializer.Serialize(integrations);
            context.Solutions.Add(solution);
            await context.SaveChangesAsync();

            await service.DeleteIntegration(solution.CatalogueItemId, invalidIntegrationId);
            var updatedSolution = await context.Solutions.SingleAsync(s => s.CatalogueItemId == solution.CatalogueItemId);
            updatedSolution.GetIntegrations().Should().BeEquivalentTo(integrations);
        }
    }
}
