using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Services.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Solutions
{
    public static class InteroperabilityServiceTests
    {
        [Theory]
        [CommonAutoData]
        public static async Task SaveIntegrationLink_UpdatesDatabase(
            Mock<IIdentityService> identityService,
            Solution solution,
            string integrationLink)
        {
            var options = new DbContextOptionsBuilder<BuyingCatalogueDbContext>()
                .UseInMemoryDatabase("BuyingCatalogue", new InMemoryDatabaseRoot())
                .Options;

            using (var context = new BuyingCatalogueDbContext(options, identityService.Object))
            {
                context.Solutions.Add(solution);
                context.SaveChanges();
            }

            using (var context = new BuyingCatalogueDbContext(options, identityService.Object))
            {
                var service = new InteroperabilityService(context);
                await service.SaveIntegrationLink(solution.Id, integrationLink);
            }

            using (var context = new BuyingCatalogueDbContext(options, identityService.Object))
            {
                var updatedSolution = await context.Solutions.SingleAsync();
                updatedSolution.IntegrationsUrl.Should().Be(integrationLink);
            }
        }

        [Theory]
        [CommonAutoData]
        public static async Task AddIntegration_UpdatesDatabase(
           Mock<IIdentityService> identityService,
           Solution solution,
           List<Integration> currentIntegrations,
           Integration newIntegration)
        {
            solution.Integrations = JsonConvert.SerializeObject(currentIntegrations);

            var options = new DbContextOptionsBuilder<BuyingCatalogueDbContext>()
                .UseInMemoryDatabase("BuyingCatalogue", new InMemoryDatabaseRoot())
                .Options;

            using (var context = new BuyingCatalogueDbContext(options, identityService.Object))
            {
                context.Solutions.Add(solution);
                context.SaveChanges();
            }

            using (var context = new BuyingCatalogueDbContext(options, identityService.Object))
            {
                var service = new InteroperabilityService(context);
                await service.AddIntegration(solution.Id, newIntegration);
            }

            using (var context = new BuyingCatalogueDbContext(options, identityService.Object))
            {
                var updatedSolution = await context.Solutions.SingleAsync();
                updatedSolution.GetIntegrations().Should().ContainSingle(i => i.Description == newIntegration.Description);
            }
        }
    }
}
