using System.Collections.Generic;
using System.Threading.Tasks;
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

            context.Solutions.Add(solution);
            context.SaveChanges();

            await service.AddIntegration(solution.CatalogueItemId, newIntegration);
            var updatedSolution = await context.Solutions.SingleAsync();
            updatedSolution.GetIntegrations().Should().Contain(i => i.Description == newIntegration.Description);
        }
    }
}
