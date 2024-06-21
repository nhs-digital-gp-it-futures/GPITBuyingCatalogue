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

    public async Task<IEnumerable<IntegrationType>> GetIntegrationTypesByIntegration(SupportedIntegrations integration)
        => await dbContext.IntegrationTypes.Where(x => x.IntegrationId == integration).ToListAsync();
}
