using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Integrations;

namespace NHSD.GPIT.BuyingCatalogue.Services.Integrations;

public sealed class IntegrationsService(BuyingCatalogueDbContext dbContext) : IIntegrationsService
{
    private readonly BuyingCatalogueDbContext dbContext =
        dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<IEnumerable<Integration>> GetIntegrations()
        => await dbContext.Integrations.ToListAsync();

    public async Task<IEnumerable<Integration>> GetIntegrationsWithTypes()
        => await dbContext.Integrations.Include(x => x.IntegrationTypes).ToListAsync();

    public async Task<Dictionary<string, IOrderedEnumerable<string>>> GetIntegrationAndTypeNames(
        Dictionary<SupportedIntegrations, int[]> integrationAndTypeIds)
        => await dbContext.Integrations.Include(x => x.IntegrationTypes)
            .Where(x => integrationAndTypeIds.Keys.Contains(x.Id))
            .OrderBy(x => x.Id)
            .ToDictionaryAsync(
                x => x.Name,
                x => x.IntegrationTypes.Where(y => integrationAndTypeIds[x.Id].Contains(y.Id))
                    .Select(y => y.Name)
                    .Order());

    public async Task<IEnumerable<IntegrationType>> GetIntegrationTypesByIntegration(SupportedIntegrations integration)
        => await dbContext.IntegrationTypes.Where(x => x.IntegrationId == integration).ToListAsync();
}
