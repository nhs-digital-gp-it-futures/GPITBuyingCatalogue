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
        => await dbContext.Integrations
            .AsNoTracking()
            .ToListAsync();

    public async Task<IEnumerable<Integration>> GetIntegrationsWithTypes()
        => await dbContext.Integrations.Include(x => x.IntegrationTypes)
            .AsNoTracking()
            .ToListAsync();

    public async Task<Integration> GetIntegrationWithTypes(SupportedIntegrations integrationId)
        => await dbContext.Integrations.Include(x => x.IntegrationTypes)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == integrationId);

    public async Task<Dictionary<string, IOrderedEnumerable<string>>> GetIntegrationAndTypeNames(
        Dictionary<SupportedIntegrations, int[]> integrationAndTypeIds)
        => await dbContext.Integrations.Include(x => x.IntegrationTypes)
            .Where(x => integrationAndTypeIds.Keys.Contains(x.Id))
            .OrderBy(x => x.Id)
            .AsNoTracking()
            .ToDictionaryAsync(
                x => x.Name,
                x => x.IntegrationTypes.Where(y => integrationAndTypeIds[x.Id].Contains(y.Id))
                    .Select(y => y.Name)
                    .Order());

    public async Task<IEnumerable<IntegrationType>> GetIntegrationTypesByIntegration(SupportedIntegrations integration)
        => await dbContext.IntegrationTypes.Where(x => x.IntegrationId == integration)
            .AsNoTracking()
            .ToListAsync();

    public async Task<IntegrationType> GetIntegrationTypeById(
        SupportedIntegrations integrationId,
        int integrationTypeId)
        => await dbContext.IntegrationTypes.Include(x => x.Integration)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == integrationTypeId && x.IntegrationId == integrationId);

    public async Task<bool> IntegrationTypeExists(
        SupportedIntegrations integrationId,
        string integrationTypeName,
        int? integrationTypeId)
        => await dbContext.IntegrationTypes.AnyAsync(
            x => x.IntegrationId == integrationId && x.Id != integrationTypeId && string.Equals(
                x.Name,
                integrationTypeName.Trim()));

    public async Task AddIntegrationType(SupportedIntegrations integrationId, string name, string description)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        var integration = await dbContext.Integrations.Include(x => x.IntegrationTypes)
            .FirstOrDefaultAsync(x => x.Id == integrationId);

        integration.IntegrationTypes.Add(new IntegrationType { Name = name, Description = description });

        await dbContext.SaveChangesAsync();
    }

    public async Task EditIntegrationType(
        SupportedIntegrations integrationId,
        int integrationTypeId,
        string name,
        string description)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        var integrationType = await dbContext.IntegrationTypes.FirstOrDefaultAsync(
            x => x.IntegrationId == integrationId && x.Id == integrationTypeId);

        if (integrationType is null) return;

        integrationType.Name = name.Trim();
        integrationType.Description = description?.Trim();

        await dbContext.SaveChangesAsync();
    }
}
