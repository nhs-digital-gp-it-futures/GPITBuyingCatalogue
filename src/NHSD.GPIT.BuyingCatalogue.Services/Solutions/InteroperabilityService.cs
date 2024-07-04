using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Services.Solutions
{
    public sealed class InteroperabilityService(BuyingCatalogueDbContext dbContext) : IInteroperabilityService
    {
        private readonly BuyingCatalogueDbContext dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

        public async Task SaveIntegrationLink(CatalogueItemId solutionId, string integrationLink)
        {
            var solution = await dbContext.Solutions.FirstAsync(s => s.CatalogueItemId == solutionId);
            solution.IntegrationsUrl = integrationLink;
            await dbContext.SaveChangesAsync();
        }

        public async Task AddIntegration(CatalogueItemId catalogueItemId, SolutionIntegration integration)
        {
            ArgumentNullException.ThrowIfNull(integration);

            var solution = await dbContext.Solutions.Include(x => x.Integrations).FirstAsync(s => s.CatalogueItemId == catalogueItemId);

            solution.Integrations.Add(integration);

            await dbContext.SaveChangesAsync();
        }

        public async Task EditIntegration(CatalogueItemId solutionId, int integrationId, SolutionIntegration integration)
        {
            ArgumentNullException.ThrowIfNull(integration);

            var solution = await dbContext.Solutions.Include(x => x.Integrations).FirstAsync(s => s.CatalogueItemId == solutionId);
            var solutionIntegration = solution.Integrations.FirstOrDefault(x => x.Id == integrationId);
            if (solutionIntegration is null) return;

            solutionIntegration.IntegrationTypeId = integration.IntegrationTypeId;
            solutionIntegration.IntegratesWith = integration.IntegratesWith;
            solutionIntegration.Description = integration.Description;
            solutionIntegration.IsConsumer = integration.IsConsumer;

            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteIntegration(CatalogueItemId solutionId, int integrationId)
        {
            var solution = await dbContext.Solutions.Include(x => x.Integrations).FirstAsync(s => s.CatalogueItemId == solutionId);

            var integration = solution.Integrations.FirstOrDefault(x => x.Id == integrationId);
            if (integration is null) return;

            solution.Integrations.Remove(integration);
            await dbContext.SaveChangesAsync();
        }

        public async Task SetNhsAppIntegrations(CatalogueItemId solutionId, IEnumerable<int> integrations)
        {
            ArgumentNullException.ThrowIfNull(integrations);

            var solution = await dbContext.Solutions.Include(x => x.Integrations).ThenInclude(x => x.IntegrationType).FirstAsync(s => s.CatalogueItemId == solutionId);

            var nhsAppIntegrations =
                solution.Integrations.Where(x => x.IntegrationType.IntegrationId == SupportedIntegrations.NhsApp)
                    .ToList();

            var newIntegrations = integrations.Select(x => new SolutionIntegration { IntegrationTypeId = x })
                .ToList();

            var toRemove =
                nhsAppIntegrations.Where(
                    x => newIntegrations.All(y => x.IntegrationTypeId != y.IntegrationTypeId));

            var toAdd =
                newIntegrations.Where(
                    x => nhsAppIntegrations.All(y => x.IntegrationTypeId != y.IntegrationTypeId));

            toRemove.ToList().ForEach(x => solution.Integrations.Remove(x));
            toAdd.ToList().ForEach(x => solution.Integrations.Add(x));

            await dbContext.SaveChangesAsync();
        }
    }
}
