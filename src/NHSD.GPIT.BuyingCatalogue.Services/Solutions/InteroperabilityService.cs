using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Services.Solutions
{
    public sealed class InteroperabilityService : IInteroperabilityService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public InteroperabilityService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task SaveIntegrationLink(CatalogueItemId solutionId, string integrationLink)
        {
            var solution = await dbContext.Solutions.FirstAsync(s => s.CatalogueItemId == solutionId);
            solution.IntegrationsUrl = integrationLink;
            await dbContext.SaveChangesAsync();
        }

        public async Task AddIntegration(CatalogueItemId catalogueItemId, Integration integration)
        {
            if (integration is null)
                throw new ArgumentNullException(nameof(integration));

            integration.Id = Guid.NewGuid();

            var solution = await dbContext.Solutions.FirstAsync(s => s.CatalogueItemId == catalogueItemId);

            var integrations = solution.GetIntegrations() ?? new List<Integration>();

            integrations.Add(integration);

            solution.Integrations = JsonSerializer.Serialize(integrations);

            await dbContext.SaveChangesAsync();
        }

        public async Task<Integration> GetIntegrationById(CatalogueItemId solutionId, Guid integrationId)
        {
            var solution = await dbContext.Solutions.FirstOrDefaultAsync(s => s.CatalogueItemId == solutionId);

            var integration = solution?.GetIntegrations().FirstOrDefault(i => i.Id == integrationId);

            return integration;
        }

        public async Task EditIntegration(CatalogueItemId solutionId, Guid integrationId, Integration integration)
        {
            if (integration is null)
                throw new ArgumentNullException(nameof(integration));

            var solution = await dbContext.Solutions.FirstAsync(s => s.CatalogueItemId == solutionId);

            var integrations = solution.GetIntegrations().ToList();

            var index = integrations.IndexOf(integrations.First(i => i.Id == integrationId));

            integrations.RemoveAt(index);

            integrations.Insert(index, integration);

            solution.Integrations = JsonSerializer.Serialize(integrations);

            await dbContext.SaveChangesAsync();
        }

        public async Task DeleteIntegration(CatalogueItemId solutionId, Guid integrationId)
        {
            var solution = await dbContext.Solutions.FirstAsync(s => s.CatalogueItemId == solutionId);

            var integrations = solution.GetIntegrations().ToList();

            var index = integrations.IndexOf(integrations.FirstOrDefault(i => i.Id == integrationId));

            if (index > -1)
            {
                integrations.RemoveAt(index);

                solution.Integrations = JsonSerializer.Serialize(integrations);

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
