using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.Services.Competitions;

public class CompetitionsQuantityService : ICompetitionsQuantityService
{
    private readonly BuyingCatalogueDbContext dbContext;

    public CompetitionsQuantityService(BuyingCatalogueDbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task SetSolutionGlobalQuantity(string internalOrgId, int competitionId, CatalogueItemId solutionId, int quantity)
    {
        var competition = await dbContext.Competitions.Include(x => x.CompetitionSolutions)
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        var solution = competition.CompetitionSolutions.FirstOrDefault(x => x.SolutionId == solutionId);
        if (solution is null) return;

        solution.Quantity = quantity;

        await dbContext.SaveChangesAsync();
    }

    public async Task SetServiceGlobalQuantity(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId serviceId,
        int quantity)
    {
        var competition = await dbContext.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.SolutionServices)
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        var solution = competition.CompetitionSolutions.FirstOrDefault(x => x.SolutionId == solutionId);

        var service = solution?.SolutionServices.FirstOrDefault(x => x.ServiceId == serviceId);
        if (service is null) return;

        service.Quantity = quantity;

        await dbContext.SaveChangesAsync();
    }

    public async Task SetSolutionRecipientQuantity(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        IEnumerable<ServiceRecipientDto> serviceRecipients)
    {
        var solution = await GetSolution(internalOrgId, competitionId, solutionId);
        if (solution is null) return;

        solution.Quantities = serviceRecipients
            .Select(x => new SolutionQuantity { OdsCode = x.OdsCode, Quantity = x.Quantity!.Value })
            .ToList();

        if (dbContext.ChangeTracker.HasChanges())
            await dbContext.SaveChangesAsync();
    }

    public async Task SetServiceRecipientQuantity(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId serviceId,
        IEnumerable<ServiceRecipientDto> serviceRecipients)
    {
        var service = await GetSolutionService(internalOrgId, competitionId, solutionId, serviceId);
        if (service is null) return;

        service.Quantities = serviceRecipients
            .Select(x => new ServiceQuantity { OdsCode = x.OdsCode, Quantity = x.Quantity!.Value })
            .ToList();

        await dbContext.SaveChangesAsync();
    }

    public async Task ResetSolutionQuantities(string internalOrgId, int competitionId, CatalogueItemId solutionId)
    {
        var solution = await GetSolution(internalOrgId, competitionId, solutionId);
        if (solution is null) return;

        solution.Quantity = null;
        solution.Quantities.Clear();

        await dbContext.SaveChangesAsync();
    }

    public async Task ResetServiceQuantities(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId serviceId)
    {
        var service = await GetSolutionService(internalOrgId, competitionId, solutionId, serviceId);
        if (service is null) return;

        service.Quantity = null;
        service.Quantities.Clear();

        await dbContext.SaveChangesAsync();
    }

    private async Task<CompetitionSolution> GetSolution(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId)
    {
        var competition = await dbContext.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Quantities)
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        var solution = competition.CompetitionSolutions.FirstOrDefault(x => x.SolutionId == solutionId);

        return solution;
    }

    private async Task<SolutionService> GetSolutionService(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId serviceId)
    {
        var competition = await dbContext.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.SolutionServices)
            .ThenInclude(x => x.Quantities)
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        var solution = competition.CompetitionSolutions.FirstOrDefault(x => x.SolutionId == solutionId);

        var service = solution?.SolutionServices.FirstOrDefault(x => x.ServiceId == serviceId);

        return service;
    }
}
