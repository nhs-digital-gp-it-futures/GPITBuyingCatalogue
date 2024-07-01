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
using NHSD.GPIT.BuyingCatalogue.Services.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Solutions
{
    public static class InteroperabilityServiceTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(InteroperabilityService).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task SaveIntegrationLink_UpdatesDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            string integrationLink,
            InteroperabilityService service)
        {
            context.Solutions.Add(solution);
            context.SaveChanges();
            context.ChangeTracker.Clear();

            await service.SaveIntegrationLink(solution.CatalogueItemId, integrationLink);

            var updatedSolution = await context.Solutions.FirstAsync();
            updatedSolution.IntegrationsUrl.Should().Be(integrationLink);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static Task AddIntegration_NullIntegration_ThrowsException(InteroperabilityService service)
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() => service.AddIntegration(default, null));
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task AddIntegration_UpdatesDatabase(
            Integration integration,
            IntegrationType integrationType,
            Solution solution,
            [Frozen] BuyingCatalogueDbContext context,
            InteroperabilityService service)
        {
            integration.IntegrationTypes = [integrationType];
            integrationType.IntegrationId = integration.Id;

            solution.Integrations = [];

            context.Add(integration);
            context.Add(solution);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            await service.AddIntegration(
                solution.CatalogueItemId,
                new SolutionIntegration { IntegrationTypeId = integrationType.Id });

            var updatedSolution = await context.Solutions.Include(x => x.Integrations)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CatalogueItemId == solution.CatalogueItemId);

            updatedSolution.Integrations.Should().HaveCount(1);
            updatedSolution.Integrations.Should().Contain(x => x.IntegrationTypeId == integrationType.Id);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task EditIntegration_UpdatesDatabase(
            string integrationDescription,
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            List<SolutionIntegration> integrations,
            InteroperabilityService service)
        {
            solution.Integrations = integrations;

            context.Solutions.Add(solution);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var integration = integrations.First();
            integration.Description = integrationDescription;

            await service.EditIntegration(solution.CatalogueItemId, integration.Id, integration);

            var updatedSolution =
                await context.Solutions.Include(x => x.Integrations)
                    .FirstAsync(s => s.CatalogueItemId == solution.CatalogueItemId);

            updatedSolution.Integrations.Should().Contain(x => x.Description == integration.Description);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static Task EditIntegration_NullIntegrationThrowsException(InteroperabilityService service)
        {
            return Assert.ThrowsAsync<ArgumentNullException>(() => service.EditIntegration(default, default, null));
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task DeleteIntegration_UpdatesDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            List<SolutionIntegration> integrations,
            InteroperabilityService service)
        {
            solution.Integrations = integrations;
            var deletedIntegration = integrations[0];

            context.Solutions.Add(solution);
            await context.SaveChangesAsync();

            await service.DeleteIntegration(solution.CatalogueItemId, deletedIntegration.Id);
            var updatedSolution =
                await context.Solutions.Include(x => x.Integrations)
                    .FirstAsync(s => s.CatalogueItemId == solution.CatalogueItemId);

            updatedSolution.Integrations.Should().NotContain(x => x.Id == deletedIntegration.Id);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task DeleteIntegration_NullIntegrationDoesNotUpdateDatabase(
            [Frozen] BuyingCatalogueDbContext context,
            Solution solution,
            List<SolutionIntegration> integrations,
            InteroperabilityService service,
            int invalidIntegrationId)
        {
            solution.Integrations = integrations;
            context.Solutions.Add(solution);
            await context.SaveChangesAsync();

            await service.DeleteIntegration(solution.CatalogueItemId, invalidIntegrationId);
            var updatedSolution =
                await context.Solutions.Include(x => x.Integrations)
                    .FirstAsync(s => s.CatalogueItemId == solution.CatalogueItemId);
            updatedSolution.Integrations.Should().BeEquivalentTo(integrations);
        }

        [Theory]
        [MockAutoData]
        public static Task SetNhsAppIntegrations_NullIntegrations_ThrowsArgumentNullException(
            CatalogueItemId solutionId,
            InteroperabilityService service) => FluentActions
            .Awaiting(() => service.SetNhsAppIntegrations(solutionId, null))
            .Should()
            .ThrowAsync<ArgumentNullException>();

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task SetNhsAppIntegrations_WithNewIntegrations_AddsIntegrations(
            Solution solution,
            Integration integration,
            List<IntegrationType> integrationTypes,
            [Frozen] BuyingCatalogueDbContext context,
            InteroperabilityService service)
        {
            integration.Id = SupportedIntegrations.NhsApp;
            integrationTypes.ForEach(x => x.IntegrationId = integration.Id);
            solution.Integrations = new List<SolutionIntegration>();

            context.Solutions.Add(solution);
            context.IntegrationTypes.AddRange(integrationTypes);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            await service.SetNhsAppIntegrations(
                solution.CatalogueItemId,
                integrationTypes.Select(x => x.Id));

            var updatedSolution = await context.Solutions.Include(x => x.Integrations).AsNoTracking()
                .FirstOrDefaultAsync(x => x.CatalogueItemId == solution.CatalogueItemId);

            var updatedIntegrations = updatedSolution.Integrations;

            updatedIntegrations.Should().HaveCount(integrationTypes.Count);
        }

        [Theory]
        [MockInMemoryDbAutoData]
        public static async Task SetNhsAppIntegrations_WithStaleIntegrations_RemovesStaleIntegrations(
            Solution solution,
            Integration integration,
            List<IntegrationType> integrationTypes,
            [Frozen] BuyingCatalogueDbContext context,
            InteroperabilityService service)
        {
            integration.Id = SupportedIntegrations.NhsApp;
            integrationTypes.ForEach(x => x.IntegrationId = integration.Id);
            solution.Integrations = new List<SolutionIntegration>();

            context.Solutions.Add(solution);
            context.IntegrationTypes.AddRange(integrationTypes);

            var staleIntegration = integrationTypes.First();

            context.Solutions.Add(solution);

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            await service.SetNhsAppIntegrations(
                solution.CatalogueItemId,
                integrationTypes.Skip(1).Select(x => x.Id));

            var updatedSolution = await context.Solutions.AsNoTracking()
                .Include(x => x.Integrations).FirstOrDefaultAsync(x => x.CatalogueItemId == solution.CatalogueItemId);

            var updatedIntegrations = updatedSolution.Integrations;

            updatedIntegrations.Should().NotContain(x => x.Id == staleIntegration.Id);
            updatedIntegrations.Should().HaveCount(integrationTypes.Skip(1).Count());
        }
    }
}
