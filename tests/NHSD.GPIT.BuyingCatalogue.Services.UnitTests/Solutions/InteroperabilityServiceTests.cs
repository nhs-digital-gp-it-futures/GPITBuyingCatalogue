using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Services.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
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
        public static async Task AddIntegration_UpdatesDatabase(
           [Frozen] BuyingCatalogueDbContext context,
           Solution solution,
           List<Integration> currentIntegrations,
           Integration newIntegration,
           InteroperabilityService service)
        {
            solution.Integrations = JsonConvert.SerializeObject(currentIntegrations);
            solution.AdditionalServices.Clear();

            context.Solutions.Add(solution);
            await context.SaveChangesAsync();

            await service.AddIntegration(solution.CatalogueItemId, newIntegration);
            var updatedSolution = await context.Solutions.SingleAsync();
            updatedSolution.GetIntegrations().Should().Contain(i => i.Description == newIntegration.Description);
        }
    }
}
