using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
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

    public async Task SetSolutionRecipientQuantity(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        IEnumerable<ServiceRecipientQuantityDto> serviceRecipients)
    {
        var solution = await GetSolution(internalOrgId, competitionId, solutionId);
        if (solution is null) return;

        var quantitiesDict = solution.Quantities.ToDictionary(x => x.RecipientOdsCode);

        serviceRecipients.ForEach(recipient => UpdateRecipientQuantity(recipient, solution, competitionId, quantitiesDict));

        if (dbContext.ChangeTracker.HasChanges())
            await dbContext.SaveChangesAsync();
    }

    public async Task SetServiceRecipientQuantity(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId serviceId,
        IEnumerable<ServiceRecipientQuantityDto> serviceRecipients)
    {
        var service = await GetSolutionService(internalOrgId, competitionId, solutionId, serviceId);
        if (service is null) return;

        var quantitiesDict = service.Quantities.ToDictionary(x => x.RecipientOdsCode);

        serviceRecipients.ForEach(recipient => UpdateRecipientQuantity(recipient, service, competitionId, quantitiesDict));

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

    private static void UpdateRecipientQuantity(
        ServiceRecipientQuantityDto recipient,
        CompetitionCatalogueItem item,
        int competitionId,
        Dictionary<string, CompetitionItemQuantity> quantitiesDict)
    {
        var competitionItemQuantity = new CompetitionItemQuantity
        {
            CompetitionId = competitionId,
            ParentSublocationOdsCode = recipient.ParentSublocationOdsCode,
            RecipientOdsCode = recipient.RecipientOdsCode,
            Quantity = recipient.Quantity,
        };
        var recipientQuantityExists = quantitiesDict.ContainsKey(recipient.RecipientOdsCode);
        if (!recipientQuantityExists)
        {
            item.Quantities.Add(competitionItemQuantity);
        }
        else
        {
            quantitiesDict[recipient.RecipientOdsCode] = competitionItemQuantity;
            item.Quantities = quantitiesDict.Values.ToList();
        }
    }

    private async Task<CompetitionSolution> GetSolution(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId)
    {
        var competition = await dbContext.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Quantities)
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        var solution = competition.CompetitionSolutions.FirstOrDefault(x => x.CatalogueItemId == solutionId);

        return solution;
    }

    private async Task<CompetitionCatalogueItem> GetSolutionService(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId serviceId)
    {
        var competition = await dbContext.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Services)
            .ThenInclude(x => x.Quantities)
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        var solution = competition.CompetitionSolutions.FirstOrDefault(x => x.CatalogueItemId == solutionId);

        var service = solution?.Services.FirstOrDefault(x => x.CatalogueItemId == serviceId);

        return service;
    }
}
